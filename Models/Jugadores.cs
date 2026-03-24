namespace PlataformJuegoTorneo.Models
{
    public class Jugadores
    {
        public string Id { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Apellido { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string NombreUsuario { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; }

        public int Edad { get; set; }

        public string Pais { get; set; } = string.Empty;

        public string Rol { get; set; } = string.Empty;

        public int PuntosGlobales { get; set; }

        public int TorneosGanados { get; set; }

        public DateTime UltimaConexion { get; set; }

        public bool Activo { get; set; } = true;

        public bool Conectado { get; set; } = false;
    }
}
