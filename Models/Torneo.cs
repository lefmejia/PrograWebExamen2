using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
namespace PlataformJuegoTorneo.Models

{
    public class Torneo
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string JuegoId { get; set; } // ID del documento del juego
        public string OrganizadorId { get; set; } // ID del organizador
        public string Descripcion { get; set; }
        public string Estado { get; set; } = "próximo";
        public string Formato { get; set; } // "individual", "equipos", "royale"
        public int MaxParticipantes { get; set; }
        public int ParticipantesActuales { get; set; } = 0; // Inicializar en 0 ya que no hay nadie inscrito
        public decimal PrecioInscripcion { get; set; }
        public decimal PremioTotal { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaLimiteInscripcion { get; set; }
        public bool ReglasModificadas { get; set; } = false; // Inicializar en false 
    }
}
