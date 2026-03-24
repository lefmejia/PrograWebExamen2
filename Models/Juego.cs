using System;
using System.Collections.Generic;

namespace PlataformJuegoTorneo.Models
{
    public class Juego
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Desarrollador { get; set; }
        public string Genero { get; set; }
        public List<string> Plataformas { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public string Descripcion { get; set; }
        public int JugadoresActivos { get; set; }
        public int TorneoActivos { get; set; }
        public string Estado { get; set; } // "disponible", "mantenimiento" o "descontinuado"
        public double PuntuacionPromedio { get; set; } // 1-5
        public DateTime FechaAgreg { get; set; }
    }
}
