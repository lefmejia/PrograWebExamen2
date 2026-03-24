using PlataformJuegoTorneo.DTOs;

namespace PlataformJuegoTorneo.Services
{
    public interface IClasificacionesService
    {
        Task<IEnumerable<RankingJugadorDto>> GetRanking(string juegoId, int page, int pageSize, int? minNivel, int? maxNivel);
        Task<MiClasificacionDto?> GetMiClasificacion(string juegoId, string userId);
    }
}
