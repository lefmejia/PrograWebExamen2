using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using PlataformJuegoTorneo.Models;
using System.Security.Claims;

namespace Examen2PrograWeb.Controllers
{
    [Route("api/torneos")]
    [ApiController]
    [Authorize] 
    public class ParticipacionesController : ControllerBase
    {
        private readonly FirestoreDb _db;

        public ParticipacionesController(FirestoreDb db)
        {
            _db = db;
        }

        [HttpPost("{id}/inscribirse")]
        public async Task<IActionResult> Inscribirse(string id, [FromBody] bool confirmarPago)
        {
            var jugadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            DocumentReference torneoRef = _db.Collection("torneos").Document(id);
            DocumentSnapshot snapshot = await torneoRef.GetSnapshotAsync();

            if (!snapshot.Exists) return NotFound("Torneo no existe.");

            // Validaciones
            if (snapshot.GetValue<string>("Estado") != "próximo") 
                return BadRequest("El torneo no está en fase de inscripción.");

            if (snapshot.GetValue<int>("ParticipantesActuales") >= snapshot.GetValue<int>("MaxParticipantes"))
                return BadRequest("Torneo lleno.");

            // Crear el registro
            var participacion = new Participacion 
            { 
                jugadorId = jugadorId, 
                torneoId = id, 
                pagado = confirmarPago 
            };

            await _db.Collection("participaciones").AddAsync(participacion);

            // Actualizar contador en la colección de Torneos
            await torneoRef.UpdateAsync("ParticipantesActuales", FieldValue.Increment(1));

            return Ok(new { mensaje = "Te has inscrito correctamente" });
        }
    }
}