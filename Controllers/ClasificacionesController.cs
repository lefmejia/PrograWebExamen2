using Microsoft.AspNetCore.Mvc;
using PlataformJuegoTorneo.Services;
using Google.Cloud.Firestore;
using PlataformJuegoTorneo.Models;
using Microsoft.AspNetCore.Authorization;

namespace PlataformJuegoTorneo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClasificacionesController : ControllerBase
    {
        private readonly ClasificacionesService _clasificacionesService;
        private readonly ILogger<ClasificacionesController> _logger;

        public ClasificacionesController(ClasificacionesService clasificacionesService, ILogger<ClasificacionesController> logger)
        {
            _clasificacionesService = clasificacionesService;
            _logger = logger;
        }

        // GET: /api/clasificaciones/{juegoId}?page=1&pageSize=50&minNivel=1&maxNivel=100
        [HttpGet("{juegoId}")]
        [Authorize]
        public async Task<IActionResult> GetRanking(string juegoId, int page = 1, int pageSize = 50, int? minNivel = null, int? maxNivel = null)
        {
            var result = await _clasificacionesService.GetRanking(juegoId, page, pageSize, minNivel, maxNivel);
            return Ok(result);
        }
    }
}
