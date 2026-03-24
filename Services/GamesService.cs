using Google.Cloud.Firestore;
using PlataformJuegoTorneo.DTOs;
using PlataformJuegoTorneo.Interfaces;
using PlataformJuegoTorneo.Models;

namespace PlataformJuegoTorneo.Services
{
    public class GamesService : IGamesService
    {
        private readonly FirebaseService _firebaseService;
        private readonly string _collectionName = "Games";

        public GamesService(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }
        public async Task<GamesDTO> CreateJuego(GamesDTO dto, string adminId)
        {
            try
            {
                // Validaciones de negocio
                if (dto.Descripcion.Length < 20)
                    throw new ArgumentException("La descripción debe tener mínimo 20 caracteres.");

                var validas = new List<string> { "PC", "PS5", "Xbox", "Switch" };
                if (dto.Plataformas.Any(p => !validas.Contains(p)))
                    throw new ArgumentException("Plataformas válidas: PC, PS5, Xbox, Switch.");

                var collection = _firebaseService.GetCollection(_collectionName);

                // Validar título único
                var existing = await collection.WhereEqualTo("Titulo", dto.Titulo).GetSnapshotAsync();
                if (existing.Count > 0) throw new ArgumentException("El título ya existe en el sistema.");

                var juego = new Games
                {
                    Id = Guid.NewGuid().ToString(),
                    Titulo = dto.Titulo,
                    Desarrollador = dto.Desarrollador,
                    Genero = dto.Genero,
                    Plataformas = dto.Plataformas,
                    FechaLanzamiento = dto.FechaLanzamiento.ToUniversalTime(),
                    Descripcion = dto.Descripcion,
                    JugadoresActivos = 0, // Requerimiento: inicializar en 0
                    TorneoActivos = 0,    // Requerimiento: inicializar en 0
                    Estado = "disponible",
                    PuntuacionPromedio = dto.PuntuacionPromedio,
                    FechaAgreg = DateTime.UtcNow
                };

                await collection.Document(juego.Id).SetAsync(juego);
                return ConvertToDto(juego);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear juego: {ex.Message}");
                throw;
            }
        }
        public async Task<List<GamesDTO>> GetJuegosDisponibles(string? genero = null, string? plataforma = null, string? desarrollador = null)
        {
            try
            {
                var collection = _firebaseService.GetCollection(_collectionName);
                Query query = collection.WhereEqualTo("Estado", "disponible");

                if (!string.IsNullOrWhiteSpace(genero)) query = query.WhereEqualTo("Genero", genero);
                if (!string.IsNullOrWhiteSpace(desarrollador)) query = query.WhereEqualTo("Desarrollador", desarrollador);
                if (!string.IsNullOrWhiteSpace(plataforma)) query = query.WhereArrayContains("Plataformas", plataforma);

                var snapshot = await query.GetSnapshotAsync();
                return snapshot.Documents.Select(doc => ConvertToDto(doc.ConvertTo<Games>())).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al listar juegos: {ex.Message}");
                throw;
            }
        }
        public async Task UpdateJuego(string id, GamesDTO dto)
        {
            try
            {
                var docRef = _firebaseService.GetCollection(_collectionName).Document(id);
                var snapshot = await docRef.GetSnapshotAsync();
                if (!snapshot.Exists) throw new KeyNotFoundException("Juego no encontrado.");

                // Solo actualizar campos permitidos según requerimiento
                var updates = new Dictionary<string, object>
                {
                    { "Descripcion", dto.Descripcion },
                    { "PuntuacionPromedio", dto.PuntuacionPromedio },
                    { "Estado", dto.Estado }
                };

                await docRef.UpdateAsync(updates);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar juego {id}: {ex.Message}");
                throw;
            }
        }
        public async Task<object?> GetEstadisticas(string id)
        {
            try
            {
                var doc = await _firebaseService.GetCollection(_collectionName).Document(id).GetSnapshotAsync();
                if (!doc.Exists) return null;

                var j = doc.ConvertTo<Games>();
                return new { j.JugadoresActivos, j.TorneoActivos, j.PuntuacionPromedio };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en estadísticas: {ex.Message}");
                throw;
            }
        }
        public async Task<GamesDTO?> GetJuegoById(string id)
        {
            try
            {
                // 1. Acceder a la colección y al documento por ID
                var collection = _firebaseService.GetCollection(_collectionName);
                var docSnapshot = await collection.Document(id).GetSnapshotAsync();

                // 2. Verificar si el documento existe
                if (!docSnapshot.Exists)
                {
                    Console.WriteLine($"Juego con ID {id} no encontrado.");
                    return null;
                }

                // 3. Convertir el documento al modelo y luego al DTO
                var juego = docSnapshot.ConvertTo<Games>();
                return ConvertToDto(juego);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el juego por ID ({id}): {ex.Message}");
                throw;
            }
        }

        private GamesDTO ConvertToDto(Games j) => new GamesDTO
        {
            Id = j.Id,
            Titulo = j.Titulo,
            Desarrollador = j.Desarrollador,
            Genero = j.Genero,
            Plataformas = j.Plataformas ?? new List<string>(),
            FechaLanzamiento = j.FechaLanzamiento,
            Descripcion = j.Descripcion,
            Estado = j.Estado,
            PuntuacionPromedio = j.PuntuacionPromedio,
            FechaAgreg = j.FechaAgreg
        };
    }
}
