using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformJuegoTorneo.DTOs;
using PlataformJuegoTorneo.Interfaces;
using PlataformJuegoTorneo.Models;
using System.Security.Claims;
namespace PlataformJuegoTorneo.Controllers
{
    /// <summary>
    /// VideojuegosController: Gestión de catálogo de juegos
    /// Responsable: Integrante 2
    /// </summary>
    [Route("api/games")]
    [ApiController]
    [Authorize] // Todos los endpoints requieren autenticación base
    public class GamesController : ControllerBase
    {
        private readonly IGamesService _service;
        public GamesController(IGamesService service)
        {
            _service = service;
        }
        /// <summary>
        /// GET: api/games
        /// Lista todos los juegos con estado "disponible"
        /// Permite filtrar por género, plataforma o desarrollador
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetJuegos([FromQuery] string? genero, [FromQuery] string? plataforma, [FromQuery] string? desarrollador)
        {
            try
            {
                var juegos = await _service.GetJuegosDisponibles(genero, plataforma, desarrollador);
                return Ok(juegos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al obtener la lista de juegos", error = ex.Message });
            }
        }
        /// <summary>
        /// GET: api/games/{id}
        /// Obtiene un juego por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var juego = await _service.GetJuegoById(id);
            if (juego == null) return NotFound(new { message = $"El juego con ID {id} no existe." });

            return Ok(juego);
        }
        /// <summary>
        /// POST: api/juegos
        /// Agrega un nuevo juego al sistema
        /// Restringido a rol "admin"
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateJuego([FromBody] GamesDTO dto)
        {
            try
            {
                // Obtener ID del administrador que realiza la acción
                var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";

                var nuevoJuego = await _service.CreateJuego(dto, adminId);

                // Devolvemos 201 Created con la ruta para obtener el recurso
                return CreatedAtAction(nameof(GetById), new { id = nuevoJuego.Id }, nuevoJuego);
            }
            catch (ArgumentException ex)
            {
                // Errores de validación (título único, plataformas, descripción corta)
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno al crear el juego", error = ex.Message });
            }
        }
        /// <summary>
        /// PUT: api/games/{id}
        /// Actualiza descripción, puntuación o estado
        /// Restringido a rol "admin"
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateJuego(string id, [FromBody] GamesDTO dto)
        {
            // Validación de estados permitidos según requerimiento
            var estadosPermitidos = new List<string> { "disponible", "mantenimiento", "descontinuado" };
            if (!estadosPermitidos.Contains(dto.Estado?.ToLower()))
            {
                return BadRequest(new { message = "Estado no permitido. Use: disponible, mantenimiento o descontinuado." });
            }

            try
            {
                await _service.UpdateJuego(id, dto);
                return NoContent(); // Respuesta 204
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Juego no encontrado." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al actualizar", error = ex.Message });
            }
        }
        /// <summary>
        /// GET: api/games/{id}/estadisticas
        /// Retorna jugadores activos, torneos y calificación
        /// Accesible para todos los usuarios autenticados
        /// </summary>
        [HttpGet("{id}/estadisticas")]
        public async Task<IActionResult> GetEstadisticas(string id)
        {
            try
            {
                var stats = await _service.GetEstadisticas(id);
                if (stats == null) return NotFound(new { message = "No se encontraron estadísticas para este juego." });

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al obtener estadísticas", error = ex.Message });
            }
        }

    }
}
