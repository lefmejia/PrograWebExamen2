using System;

namespace PlataformJuegoTorneo.Models
{
    public class Torneo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Juego { get; set; } // ID del documento del juego
        public string Organizador { get; set; } // ID del documento del organizador
        public string Descripcion { get; set; } // Descripción y reglas del torneo
        public string Estado { get; set; } // "próximo", "en progreso", "finalizado" o "cancelado"
        public string Formato { get; set; } // "individual", "equipos" o "royale"
        public int MaxParticipantes { get; set; }
        public int ParticipantesActuales { get; set; }
        public decimal PrecioInscripcion { get; set; } // Costo en moneda local
        public decimal PremioTotal { get; set; } // Monto total en premios
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaLimiteInscripcion { get; set; }
        public int MinNivel { get; set; } // Nivel mínimo requerido (1-100)
        public int MaxNivel { get; set; } // Nivel máximo permitido (1-100, 0 = sin límite)
        public bool RequiereEquipo { get; set; }
        public int TamanioEquipo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool ReglasModificadas { get; set; }
    }
}
