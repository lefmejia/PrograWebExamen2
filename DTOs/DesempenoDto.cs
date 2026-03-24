using System.Collections.Generic;

namespace PlataformJuegoTorneo.DTOs
{
    public class DesempenoDto
    {
        public int NivelActual { get; set; }
        public int PosicionRanking { get; set; }
        public double ProgresoSiguienteNivel { get; set; }
        public double RatioVictoria { get; set; }
        public int RachaActual { get; set; }
        public MedallasDto Medallas { get; set; } = new();
        public IEnumerable<MejorTorneoDto> MejoresTorneos { get; set; } = new List<MejorTorneoDto>();
        public IEnumerable<string> Logros { get; set; } = new List<string>();
    }

    public class MedallasDto
    {
        public int Oro { get; set; }
        public int Plata { get; set; }
        public int Bronce { get; set; }
    }

    public class MejorTorneoDto
    {
        public int PuntosJuego { get; set; }
        public int Posicion { get; set; }
    }
}
