using PlataformJuegoTorneo.DTOs;

namespace PlataformJuegoTorneo.Interfaces
{
    public interface IGamesService
    {
        Task<List<GamesDTO>> GetJuegosDisponibles(string? genero = null, string? plataforma = null, string? desarrollador = null);

        Task<GamesDTO?> GetJuegoById(string id);

        Task<GamesDTO> CreateJuego(GamesDTO dto, string adminId);

        Task UpdateJuego(string id, GamesDTO dto);

        Task<object?> GetEstadisticas(string id);

    }
}
