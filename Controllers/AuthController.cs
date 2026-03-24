using Microsoft.AspNetCore.Mvc;
using PlataformJuegoTorneo.DTOs;
using PlataformJuegoTorneo.Models;
using PlataformJuegoTorneo.Services;

namespace PlataformJuegoTorneo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Register([FromBody] RegistroDto registerDto)
        {
            try
            {
                // Validar que el DTO no es nulo
                if (registerDto == null)
                {
                    return BadRequest(new { message = "El cuerpo de la petición es requerido" });
                }

                // Validar que email y password no están vacíos
                if (string.IsNullOrWhiteSpace(registerDto.Correo) ||
                    string.IsNullOrWhiteSpace(registerDto.Password))
                {
                    return BadRequest(new { message = "Email y contraseña son requeridos" });
                }

                // Llamar al servicio para registrar
                var user = await _authService.Registro(registerDto);

                _logger.LogInformation($"Usuario registrado: {user.Correo}");

                // Devolver 201 (Created) con el usuario creado
                return Created($"/api/auth/users/{user.Id}", user);
            }
            catch (ArgumentException ex)
            {
                // Errores de validación
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Email ya existe, etc.
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en registro: {ex.Message}");
                return StatusCode(500, new { message = "Error al registrar jugador" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                // Validar entrada
                if (loginDto == null)
                {
                    return BadRequest(new { message = "El cuerpo de la petición es requerido" });
                }

                if (string.IsNullOrWhiteSpace(loginDto.Correo) ||
                    string.IsNullOrWhiteSpace(loginDto.Password))
                {
                    return BadRequest(new { message = "Email y contraseña son requeridos" });
                }

                // Llamar al servicio para hacer login
                var (user, token) = await _authService.Login(loginDto);

                _logger.LogInformation($"Usuario inició sesión: {user.Correo}");

                // Devolver el token y datos del usuario
                var response = new AuthResponseDto
                {
                    Success = true,
                    Message = "Login exitoso",
                    Token = token,
                    Jugador = new JugadoresDto
                    {
                        Id = user.Id,
                        NombreUsuario = user.NombreUsuario,
                        Rol = user.Rol,
                        FechaRegistro = user.FechaRegistro,
                        PuntosGlobales = user.PuntosGlobales,
                        TorneosGanados = user.TorneosGanados,
                        UltimaConexion = user.UltimaConexion,
                        Activo = user.Activo,
                        Conectado = user.Conectado,
                    }
                };

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                // Email no existe o contraseña incorrecta
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en login: {ex.Message}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al iniciar sesión"
                });
            }
        }
    }
}
