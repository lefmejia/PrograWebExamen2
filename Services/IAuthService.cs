using PlataformJuegoTorneo.Models;
using PlataformJuegoTorneo.DTOs;

namespace PlataformJuegoTorneo.Services
{
    public interface IAuthService
    {
        Task<Jugadores> Registro(RegistroDto registroDto);

        Task<(Jugadores jugador, string token)> Login(LoginDto loginDto);

        Task<bool> ValidateToken(string token);

        Task<Jugadores?> GetJugadorById(string userId);

        Task<Jugadores> ActualizarPerfil(string jugadorId, Jugadores jugador);

        string GenerateJwtToken(Jugadores jugador);

        Task<List<Jugadores>?> GetAllJugadores();
    }
}
