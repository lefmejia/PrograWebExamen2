namespace PlataformJuegoTorneo.DTOs
{
    public class Games
    {
        public string Id { get; set; }
        public string Titulo { get; set; }
        public string Desarrollador { get; set; }
        public string Genero { get; set; }
        public List<string> Plataformas { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public string Descripcion { get; set; }
        public int JugadoresActivos { get; set; }
        public int TorneoActivos { get; set; }
        public string Estado { get; set; }
        public double PuntuacionPromedio { get; set; }
        public DateTime FechaAgreg { get; set; }
    }
}
