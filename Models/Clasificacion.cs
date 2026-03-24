using System;
using System.Collections.Generic;

namespace PlataformJuegoTorneo.Models
{
    public class Clasificacion
    {
        public int Id { get; set; }
        public string JugadorId { get; set; }
        public string JuegoId { get; set; }
        public int Posicion { get; set; }
        public int PuntosJuego { get; set; }
        public int NivelJuego { get; set; } // 1-100
        public int TorneoGanados { get; set; }
        public double RatioVictoria { get; set; } // 0-100
        public int TotalPartidas { get; set; }
        public int Racha { get; set; }
        public int RachaMaxima { get; set; }
        public int MedallasOro { get; set; }
        public int MedallaPlata { get; set; }
        public int MedallaBronce { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string EstiloJuego { get; set; }
        public List<string> Logros { get; set; }
    }
}
