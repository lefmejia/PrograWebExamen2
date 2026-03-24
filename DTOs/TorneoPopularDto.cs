namespace PlataformJuegoTorneo.DTOs
{
    public class TorneoPopularDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Juego { get; set; } = string.Empty;
        public int CantidadInscripciones { get; set; }
        public decimal PremioTotal { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}