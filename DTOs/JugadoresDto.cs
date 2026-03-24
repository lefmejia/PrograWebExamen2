namespace PlataformJuegoTorneo.DTOs
{
    public class JugadoresDto
    {
        public string Id { get; set; } = string.Empty;

        public string NombreUsuario { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; }

        public string Rol { get; set; } = string.Empty;

        public int PuntosGlobales { get; set; }

        public int TorneosGanados { get; set; }

        public DateTime UltimaConexion { get; set; }

        public bool Activo { get; set; } = true;

        public bool Conectado { get; set; } = false;
    }

    public class RegistroDto
    {

        public string Nombre { get; set; } = string.Empty;

        public string Apellido { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string NombreUsuario { get; set; } = string.Empty;

        public int Edad { get; set; }

        public string Pais { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        public string Correo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
        public JugadoresDto Jugador { get; set; } = new JugadoresDto();
    }
}
