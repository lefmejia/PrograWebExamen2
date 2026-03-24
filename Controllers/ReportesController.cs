using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PlataformJuegoTorneo.Services;
using Google.Cloud.Firestore;
using PlataformJuegoTorneo.Models;
using System.Security.Claims;

namespace PlataformJuegoTorneo.Controllers
{
    [ApiController]
    [Route("api/reportes")]
    public class ReportesController : ControllerBase
    {
        private readonly IReportesService _reportesService;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(IReportesService reportesService, ILogger<ReportesController> logger)
        {
            _reportesService = reportesService;
            _logger = logger;
        }

        // GET: /api/reportes/torneos-populares
        [HttpGet("torneos-populares")]
        [Authorize(Roles = "organizador,admin")]
        public async Task<IActionResult> GetTorneosPopulares()
        {
            var result = await _reportesService.GetTorneosPopulares();
            return Ok(result);
        }

        // GET: /api/reportes/jugadores-destacados
        [HttpGet("jugadores-destacados")]
        [Authorize]
        public async Task<IActionResult> GetJugadoresDestacados()
        {
            var result = await _reportesService.GetJugadoresDestacados();
            return Ok(result);
        }

        // GET: /api/reportes/mi-desempeno/{juegoId}
        [HttpGet("mi-desempeno/{juegoId}")]
        [Authorize]
        public async Task<IActionResult> GetMiDesempeno(string juegoId)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var result = await _reportesService.GetMiDesempeno(userId, juegoId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        // GET: /api/reportes/tendencias
        [HttpGet("tendencias")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetTendencias()
        {
            var result = await _reportesService.GetTendencias();
            return Ok(result);
        }
    }
}
