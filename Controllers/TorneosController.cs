using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using PlataformJuegoTorneo.Models;

namespace Examen2PrograWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TorneosController : ControllerBase
    {
        private readonly FirestoreDb _db;

        public TorneosController(FirestoreDb db)
        {
            _db = db;
        }

        // 1. ENDPOINT: CREAR TORNEO (POST)
        [HttpPost]
        // [Authorize(Roles = "organizador,admin")] // Esto se borra al estar el JWT configurado
        public async Task<IActionResult> CrearTorneo([FromBody] Torneo nuevo)
        {
            // Validaciones
            if (nuevo.FechaInicio <= DateTime.UtcNow)
                return BadRequest("Error: La fecha de inicio debe ser futura.");

            if (nuevo.FechaLimiteInscripcion >= nuevo.FechaInicio)
                return BadRequest("Error: La fecha límite debe ser antes del inicio.");

            if (nuevo.MaxParticipantes <= 2)
                return BadRequest("Error: El torneo debe tener más de 2 participantes.");

            // Tipos de Inscripciones Permitidas
            var formatos = new List<string> { "individual", "equipos", "royale" };
            if (!formatos.Contains(nuevo.Formato.ToLower()))
                return BadRequest("Error: Formato no válido (usar individual, equipos o royale).");

            try
            {
                CollectionReference colRef = _db.Collection("torneos");

                // Convertimos el objeto a un diccionario para Firestore
                var datos = new Dictionary<string, object>
                {
                    { "nombre", nuevo.Nombre },
                    { "juegoId", nuevo.JuegoId },
                    { "organizadorId", nuevo.OrganizadorId },
                    { "descripcion", nuevo.Descripcion },
                    { "estado", "próximo" },
                    { "formato", nuevo.Formato },
                    { "maxParticipantes", nuevo.MaxParticipantes },
                    { "participantesActuales", 0 }, // Requisito: inicia en 0
                    { "precioInscripcion", nuevo.PrecioInscripcion },
                    { "premioTotal", nuevo.PremioTotal },
                    { "fechaInicio", Timestamp.FromDateTime(nuevo.FechaInicio.ToUniversalTime()) },
                    { "fechaLimiteInscripcion", Timestamp.FromDateTime(nuevo.FechaLimiteInscripcion.ToUniversalTime()) },
                    { "reglasModificadas", false }
                };

                DocumentReference doc = await colRef.AddAsync(datos);
                return Ok(new { id = doc.Id, mensaje = "Torneo creado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // 2. Filtrar torneos
        [HttpGet]
        public async Task<IActionResult> ListarTorneos([FromQuery] string? juegoId, [FromQuery] string? estado)
        {
            Query query = _db.Collection("torneos");

            if (!string.IsNullOrEmpty(juegoId))
                query = query.WhereEqualTo("juegoId", juegoId);

            if (!string.IsNullOrEmpty(estado))
                query = query.WhereEqualTo("estado", estado);

            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            var resultados = snapshot.Documents.Select(d => d.ToDictionary()).ToList();

            return Ok(resultados);
        }
    
    // 3. ENDPOINT: ACTUALIZAR ESTADO (PATCH)
// Requisito: Los torneos inician en "próximo" y deben poder cambiar de fase
[HttpPatch("{id}/estado")]
        public async Task<IActionResult> ActualizarEstado(string id, [FromBody] string nuevoEstado)
        {
            // Validamos que el estado sea uno de los permitidos en las reglas del negocio
            var estadosValidos = new List<string> { "próximo", "en progreso", "finalizado", "cancelado" };

            if (string.IsNullOrEmpty(nuevoEstado) || !estadosValidos.Contains(nuevoEstado.ToLower()))
            {
                return BadRequest("Estado no válido. Use: próximo, en progreso, finalizado o cancelado.");
            }

            try
            {

                DocumentReference docRef = _db.Collection("torneos").Document(id);

                // Verificar que el torneo existe
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (!snapshot.Exists)
                {
                    return NotFound($"No se encontró el torneo con ID: {id}");
                }

                // Actualizar estado del torneo
                await docRef.UpdateAsync("estado", nuevoEstado.ToLower());

                return Ok(new { mensaje = $"Estado del torneo {id} actualizado a {nuevoEstado}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar en Firebase: {ex.Message}");
            }
        }
    }
}