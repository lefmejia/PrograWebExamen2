using System.Collections.Generic;

namespace PlataformJuegoTorneo.DTOs
{
    public class MiClasificacionDto
    {
        public int Rank { get; set; }
        public int Puntos { get; set; }
        public int Nivel { get; set; }
        public MedallasDto Medallas { get; set; } = new();
        public IEnumerable<string> Logros { get; set; } = new List<string>();
    }
}
