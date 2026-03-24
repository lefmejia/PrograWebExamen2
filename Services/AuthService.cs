using BCrypt.Net;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PlataformJuegoTorneo.DTOs;
using PlataformJuegoTorneo.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PlataformJuegoTorneo.Services
{
    public class AuthService : IAuthService
    {
        private readonly FirebaseService _firebaseService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            FirebaseService firebaseService,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _firebaseService = firebaseService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Jugadores> Registro(RegistroDto registroDto)
        {
            try
            {
                // Validar que el DTO no es nulo
                if (registroDto == null)
                {
                    throw new ArgumentException("El cuerpo de la petición es requerido");
                }

                // Validar que email y password no están vacíos
                if (string.IsNullOrWhiteSpace(registroDto.Correo) ||
                    string.IsNullOrWhiteSpace(registroDto.Password))
                {
                    throw new ArgumentException("Email y contraseña son requeridos");
                }

                if (string.IsNullOrWhiteSpace(registroDto.Nombre) ||
                    string.IsNullOrWhiteSpace(registroDto.Apellido))
                {
                    throw new ArgumentException("Nombre y apellido son requeridos");
                }

                // Validar que la contraseña tenga longitud mínima
                if (registroDto.Password.Length < 6)
                {
                    throw new ArgumentException("La contraseña debe tener al menos 6 caracteres");
                }

                if (string.IsNullOrWhiteSpace(registroDto.NombreUsuario))
                {
                    throw new ArgumentException("Nombre de usuario requerido");
                }

                if (registroDto.Edad == null || registroDto.Edad < 12)
                {
                    throw new ArgumentException("Edad tiene que ser mayor a 12 para registrarse");
                }

                if (string.IsNullOrWhiteSpace(registroDto.Pais))
                {
                    throw new ArgumentException("Pais es requerido");
                }

                // Obtener la colección de usuarios desde Firestore
                var jugadoresCollection = _firebaseService.GetCollection("jugadores");

                if (jugadoresCollection == null)
                {
                    throw new InvalidOperationException("No se pudo obtener la colección de usuarios");
                }

                // Verificar que el email no está registrado
                var query = await jugadoresCollection
                    .WhereEqualTo("Email", registroDto.Correo)
                    .GetSnapshotAsync();

                if (query.Count > 0)
                {
                    throw new InvalidOperationException("El email ya está registrado");
                }

                query = await jugadoresCollection
                    .WhereEqualTo("NombreUsuario", registroDto.NombreUsuario)
                    .GetSnapshotAsync();

                if (query.Count > 0)
                {
                    throw new InvalidOperationException("Nombre de usuario ya esta en uso");
                }

                // Hashear la contraseña con BCrypt
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(registroDto.Password);

                // Crear nuevo usuario
                var newUser = new Jugadores
                {
                    Id = Guid.NewGuid().ToString(),
                    Correo = registroDto.Correo,
                    Nombre = registroDto.Nombre,
                    Apellido = registroDto.Apellido,
                    NombreUsuario = registroDto.NombreUsuario,
                    Edad = registroDto.Edad,
                    Pais = registroDto.Pais,
                    Rol = "jugador",
                    PuntosGlobales = 0,
                    TorneosGanados = 0,
                    FechaRegistro = DateTime.UtcNow,
                    UltimaConexion = DateTime.UtcNow,
                    Activo = true,
                    Conectado = false
                };

                // Guardar el usuario en Firestore usando Dictionary
                var userData = new Dictionary<string, object>
                {
                    { "Id", newUser.Id },
                    { "Correo", newUser.Correo },
                    { "Nombre", newUser.Nombre },
                    { "Rol", newUser.Rol },
                    { "Apellido", newUser.Apellido },
                    { "NombreUsuario", newUser.NombreUsuario },
                    { "Edad", newUser.Edad },
                    { "Pais", newUser.Pais },
                    { "PuntosGlobales", newUser.PuntosGlobales },
                    { "TorneosGanados", newUser.TorneosGanados },
                    { "FechaRegistro", newUser.FechaRegistro },
                    { "UltimaConexion", newUser.UltimaConexion },
                    { "Activo", newUser.Activo },
                    { "Conectado", newUser.Conectado },
                    { "PasswordHash", passwordHash }  // Guardar hash, NO la contraseña
                };

                await jugadoresCollection.Document(newUser.Id).SetAsync(userData);

                return newUser;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"Error de validación en Register: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Error lógico en Register: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inesperado en Register: {ex.Message}");
                throw;
            }
        }
        public async Task<(Jugadores jugador, string token)> Login(LoginDto loginDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginDto.Correo) ||
                    string.IsNullOrWhiteSpace(loginDto.Password))
                {
                    throw new ArgumentException("Email y contraseña son requeridos");
                }

                var jugadoresCollection = _firebaseService.GetCollection("jugadores");

                if (jugadoresCollection == null)
                {
                    throw new InvalidOperationException("No se pudo obtener la colección de juagadores");
                }

                var query = await jugadoresCollection
                    .WhereEqualTo("Correo", loginDto.Correo)
                    .GetSnapshotAsync();

                if (query.Count == 0)
                {
                    throw new InvalidOperationException("Email o contraseña incorrectos");
                }

                var userDoc = query.Documents[0];
                var userDict = userDoc.ToDictionary();

                // Obtener el hash de contraseña guardado
                var passwordHash = userDict["PasswordHash"].ToString();

                // Validar la contraseña contra el hash con BCrypt
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, passwordHash))
                {
                    throw new InvalidOperationException("Email o contraseña incorrectos");
                }

                // Convertir el diccionario a objeto User
                var jugador = new Jugadores
                {
                    Id = userDict["Id"].ToString(),
                    Correo = userDict["Correo"].ToString(),
                    Nombre = userDict["Nombre"].ToString(),
                    Rol = userDict["Rol"].ToString(),
                    Apellido = userDict["Apellido"].ToString(),
                    NombreUsuario = userDict["NombreUsuario"].ToString(),
                    Pais = userDict["Pais"].ToString(),
                    TorneosGanados = (int)(long)userDict["TorneosGanados"],
                    PuntosGlobales = (int)(long)userDict["PuntosGlobales"],
                    Edad = (int)(long)userDict["Edad"],
                    FechaRegistro = ((Timestamp)userDict["FechaRegistro"]).ToDateTime(),
                    Activo = (bool)userDict["Activo"],
                    Conectado = (bool)userDict["Conectado"]
                };

                var token = GenerateJwtToken(jugador);

                // Actualizar LastLogin
                await jugadoresCollection.Document(jugador.Id).UpdateAsync(
                    new Dictionary<string, object>
                    {
                        { "UltimaConexion", DateTime.UtcNow },
                        { "Conectado", true }
                    }
                );

                return (jugador, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Login: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Jugadores>?> GetAllJugadores()
        {
            try
            {
                var jugadoresCollection = _firebaseService.GetCollection("jugadores");
                var query = jugadoresCollection.WhereEqualTo("Rol", "jugador");
                var snapshot = await query.GetSnapshotAsync();

                var jugadores = new List<Jugadores>();

                foreach (var doc in snapshot.Documents)
                {
                    var userDict = doc.ToDictionary();
                    var jugador = new Jugadores
                    {
                        Id = userDict["Id"].ToString(),
                        Correo = userDict["Correo"].ToString(),
                        Nombre = userDict["Nombre"].ToString(),
                        Rol = userDict["Rol"].ToString(),
                        Apellido = userDict["Apellido"].ToString(),
                        NombreUsuario = userDict["NombreUsuario"].ToString(),
                        Pais = userDict["Pais"].ToString(),
                        TorneosGanados = (int)(long)userDict["TorneosGanados"],
                        PuntosGlobales = (int)(long)userDict["PuntosGlobales"],
                        Edad = (int)(long)userDict["Edad"],
                        FechaRegistro = ((Timestamp)userDict["FechaRegistro"]).ToDateTime(),
                        UltimaConexion = ((Timestamp)userDict["UltimaConexion"]).ToDateTime(),
                        Activo = (bool)userDict["Activo"],
                        Conectado = (bool)userDict["Conectado"]
                    };
                    jugadores.Add(jugador);
                }

                return jugadores;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener huéspedes: {ex.Message}");
                return null;
            }
        }

        public async Task<Jugadores?> GetJugadorById(string userId)
        {
            try
            {
                var usersCollection = _firebaseService.GetCollection("jugadores");
                var doc = await usersCollection.Document(userId).GetSnapshotAsync();

                if (!doc.Exists)
                {
                    return null;
                }

                var userDict = doc.ToDictionary();

                var jugador = new Jugadores
                {
                    Id = userDict["Id"].ToString(),
                    Correo = userDict["Correo"].ToString(),
                    Nombre = userDict["Nombre"].ToString(),
                    Rol = userDict["Rol"].ToString(),
                    Apellido = userDict["Apellido"].ToString(),
                    NombreUsuario = userDict["NombreUsuario"].ToString(),
                    Pais = userDict["Pais"].ToString(),
                    TorneosGanados = (int)(long)userDict["TorneosGanados"],
                    PuntosGlobales = (int)(long)userDict["PuntosGlobales"],
                    FechaRegistro = ((Timestamp)userDict["FechaRegistro"]).ToDateTime(),
                    UltimaConexion = ((Timestamp)userDict["UltimaConexion"]).ToDateTime(),
                    Activo = (bool)userDict["Activo"],
                    Conectado = (bool)userDict["Conectado"]
                };

                return jugador;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener jugador: {ex.Message}");
                return null;
            }
        }

        public async Task<Jugadores> ActualizarPerfil(string jugadorId, Jugadores jugador)
        {
            try
            {
                // Validar entrada
                if (string.IsNullOrWhiteSpace(jugadorId))
                {
                    throw new ArgumentException("El ID de jugador es requerido");
                }

                var jugadoresCollection = _firebaseService.GetCollection("jugadores");

                // Verificar que la película existe
                var existingDoc = await jugadoresCollection.Document(jugadorId).GetSnapshotAsync();
                if (!existingDoc.Exists)
                {
                    throw new InvalidOperationException($"Jugador con ID {jugadorId} no existe");
                }

                // Obtener la película existente para preservar campos de auditoría
                var existingDict = existingDoc.ToDictionary();
                var existingJugador = new Jugadores
                {
                    Id = existingDict["Id"].ToString(),
                    Correo = existingDict["Correo"].ToString(),
                    Nombre = existingDict["Nombre"].ToString(),
                    Rol = existingDict["Rol"].ToString(),
                    Apellido = existingDict["Apellido"].ToString(),
                    NombreUsuario = existingDict["NombreUsuario"].ToString(),
                    Pais = existingDict["Pais"].ToString(),
                    TorneosGanados = (int)(long)existingDict["TorneosGanados"],
                    PuntosGlobales = (int)(long)existingDict["PuntosGlobales"],
                    Edad = (int)(long)existingDict["Edad"],
                    FechaRegistro = ((Timestamp)existingDict["FechaRegistro"]).ToDateTime(),
                    UltimaConexion = ((Timestamp)existingDict["UltimaConexion"]).ToDateTime(),
                    Activo = (bool)existingDict["Activo"],
                    Conectado = (bool)existingDict["Conectado"]
                };

                // Actualizar solo los campos permitidos
                existingJugador.Nombre = jugador.Nombre ?? existingJugador.Nombre;
                existingJugador.Apellido = jugador.Apellido ?? existingJugador.Apellido;
                existingJugador.Edad = jugador.Edad > 0 ? jugador.Edad : existingJugador.Edad;
                existingJugador.Pais = jugador.Pais ?? existingJugador.Pais;

                // Guardar cambios usando Dictionary
                var jugadorData = new Dictionary<string, object>
                {
                    { "Id", existingJugador.Id },
                    { "Correo", existingJugador.Correo },
                    { "Nombre", existingJugador.Nombre },
                    { "Rol", existingJugador.Rol },
                    { "Apellido", existingJugador.Apellido },
                    { "NombreUsuario", existingJugador.NombreUsuario },
                    { "Edad", existingJugador.Edad },
                    { "Pais", existingJugador.Pais },
                    { "PuntosGlobales", existingJugador.PuntosGlobales },
                    { "TorneosGanados", existingJugador.TorneosGanados },
                    { "FechaRegistro", existingJugador.FechaRegistro },
                    { "UltimaConexion", existingJugador.UltimaConexion },
                    { "Activo", existingJugador.Activo },
                    { "Conectado", existingJugador.Conectado },
                    { "PasswordHash", existingDict["PasswordHash"].ToString() },
                };

                await jugadoresCollection.Document(jugadorId).SetAsync(jugadorData);

                Console.WriteLine($"Película actualizada: {jugadorId}");
                return existingJugador;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar película: {ex.Message}");
                throw;
            }
        }

        public string GenerateJwtToken(Jugadores jugador)
        {
            try
            {
                var secretKey = _configuration["Jwt:SecretKey"];
                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];

                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new InvalidOperationException("JWT SecretKey no configurado");
                }

                var key = Encoding.ASCII.GetBytes(secretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("sub", jugador.Id),
                        new Claim("email", jugador.Correo),
                        new Claim("name", jugador.Nombre),
                        new Claim("role", jugador.Rol)
                    }),
                    Expires = DateTime.UtcNow.AddHours(24),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al generar token: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ValidateToken(string token)
        {
            try
            {
                var secretKey = _configuration["Jwt:SecretKey"];
                if (string.IsNullOrEmpty(secretKey))
                {
                    return false;
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(secretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validando token: {ex.Message}");
                return false;
            }
        }
    }
}
