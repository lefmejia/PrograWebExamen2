using System.Collections.Generic;

namespace PlataformJuegoTorneo.DTOs
{
    public class TendenciasDto
    {
        public IEnumerable<JuegoPopularDto> JuegosPopulares { get; set; } = new List<JuegoPopularDto>();
        public IEnumerable<GeneroTorneoDto> GenerosConMasTorneos { get; set; } = new List<GeneroTorneoDto>();
    }

    public class JuegoPopularDto
    {
        public string Titulo { get; set; } = string.Empty;
        public int JugadoresActivos { get; set; }
    }

    public class GeneroTorneoDto
    {
        public string Genero { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}
