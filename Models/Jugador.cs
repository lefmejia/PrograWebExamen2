using System;

namespace PlataformJuegoTorneo.Models
{
    public class Jugador
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; } // Email único
        public string Contraseña { get; set; } // Contraseña hasheada con BCrypt
        public string NombreUsuario { get; set; } // Nombre de usuario único
        public int Edad { get; set; }
        public string Pais { get; set; }
        public string Rol { get; set; } // "jugador", "organizador" o "admin"
        public bool Activo { get; set; }
        public int PuntosGlobales { get; set; }
        public int TorneoGanados { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Conectado { get; set; }
        public DateTime? UltimaConexion { get; set; }
    }
}
