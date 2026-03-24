namespace PlataformJuegoTorneo.DTOs
{
    public class JugadorDestacadoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public int PuntosGlobales { get; set; }
        public int TorneosGanados { get; set; }
        public int CantidadJuegos { get; set; }
    }
}