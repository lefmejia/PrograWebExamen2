using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformJuegoTorneo.DTOs;
using PlataformJuegoTorneo.Models;
using PlataformJuegoTorneo.Services;

namespace PlataformJuegoTorneo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JugadoresController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public JugadoresController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet("{jugadorId}")]
        public async Task<IActionResult> GetGuestById(string jugadorId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jugadorId))
                {
                    return BadRequest(new { message = "El ID de huésped es requerido" });
                }

                var jugador = await _authService.GetJugadorById(jugadorId);

                if (jugador == null)
                {
                    return NotFound(new { message = "Jugador no encontrado" });
                }

                var jugadorDto = new JugadoresDto
                {
                    Id = jugador.Id,
                    NombreUsuario = jugador.NombreUsuario,
                    Rol = jugador.Rol,
                    FechaRegistro = jugador.FechaRegistro,
                    PuntosGlobales = jugador.PuntosGlobales,
                    TorneosGanados = jugador.TorneosGanados,
                    UltimaConexion = jugador.UltimaConexion,
                    Activo = jugador.Activo,
                    Conectado = jugador.Conectado,
                };

                var response = new
                {
                    jugador = jugadorDto
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener información del jugador: {ex.Message}");
                return StatusCode(500, new { message = "Error al obtener información del jugador" });
            }
        }

        [HttpPut("{jugadorId}/perfil")]
        public async Task<IActionResult> ActualizarPerfil(string jugadorId, [FromBody] Jugadores jugador)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jugadorId))
                {
                    return BadRequest(new { message = "El ID de jugador es requerido" });
                }

                if (jugador == null)
                {
                    return BadRequest(new { message = "El cuerpo de la petición es requerido" });
                }

                var updatedJugador = await _authService.ActualizarPerfil(jugadorId, jugador);

                _logger.LogInformation($"Jugador actualizada: {jugadorId}");

                return Ok(updatedJugador);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar película: {ex.Message}");
                return StatusCode(500, new { message = "Error al actualizar película" });
            }
        }
    }
}
