using PlataformJuegoTorneo.DTOs;

namespace PlataformJuegoTorneo.Services
{
    public interface IReportesService
    {
        Task<IEnumerable<TorneoPopularDto>> GetTorneosPopulares();
        Task<IEnumerable<JugadorDestacadoDto>> GetJugadoresDestacados();
        Task<DesempenoDto?> GetMiDesempeno(string userId, string juegoId);
        Task<TendenciasDto> GetTendencias();
    }
}
