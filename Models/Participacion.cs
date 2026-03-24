using System;

namespace PlataformJuegoTorneo.Models
{
    public class Participacion
    {
        public int Id { get; set; }
        public string JugadorId { get; set; }
        public string TorneoId { get; set; }
        public string? EquipoId { get; set; }
        public string Estado { get; set; } // "inscrito", "jugando", "eliminado" o "completo"
        public int? Posicion { get; set; }
        public int PuntosObtenidos { get; set; }
        public int PartidasJugadas { get; set; }
        public int Victorias { get; set; }
        public int Derrotas { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
        public EstadisticasJuego Estadisticas { get; set; }
        public int Penalizaciones { get; set; }
        public bool Pagado { get; set; }
    }

    public class EstadisticasJuego
    {
        public int Asesinatos { get; set; }
        public int Muertes { get; set; }
        public int Asistencias { get; set; }
        public int DañoCausado { get; set; }
    }
}
