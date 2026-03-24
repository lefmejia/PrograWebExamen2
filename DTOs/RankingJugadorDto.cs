namespace PlataformJuegoTorneo.DTOs
{
    public class RankingJugadorDto
    {
        public int Posicion { get; set; }
        public string NombreJugador { get; set; } = string.Empty;
        public int Puntos { get; set; }
        public int Nivel { get; set; }
        public double RatioVictoria { get; set; }
        public int TotalPartidas { get; set; }
        public int RachaActual { get; set; }
    }
}