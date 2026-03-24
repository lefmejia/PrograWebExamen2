using PlataformJuegoTorneo.Models;
using PlataformJuegoTorneo.DTOs;
using Google.Cloud.Firestore;

namespace PlataformJuegoTorneo.Services
{
    public class ClasificacionesService: IClasificacionesService
    {
        private readonly FirebaseService _firebaseService;
        private readonly ILogger<ClasificacionesService> _logger;

        public ClasificacionesService(FirebaseService firebaseService, ILogger<ClasificacionesService> logger)
        {
            _firebaseService = firebaseService;
            _logger = logger;
        }

        public async Task<IEnumerable<RankingJugadorDto>> GetRanking(string juegoId, int page, int pageSize, int? minNivel, int? maxNivel)
        {
            if (pageSize > 50) pageSize = 50;
            var clasificacionesRef = _firebaseService.GetCollection("clasificaciones");
            var query = clasificacionesRef.WhereEqualTo("JuegoId", juegoId);
            if (minNivel.HasValue)
                query = query.WhereGreaterThanOrEqualTo("NivelJuego", minNivel.Value);
            if (maxNivel.HasValue)
                query = query.WhereLessThanOrEqualTo("NivelJuego", maxNivel.Value);
            var snapshot = await query.GetSnapshotAsync();
            var list = snapshot.Documents
                .Select(d => d.ConvertTo<Clasificacion>())
                .OrderBy(c => c.Posicion)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            // Obtener nombres de jugadores
            var jugadorIds = list.Select(c => c.JugadorId).ToList();
            var jugadoresRef = _firebaseService.GetCollection("jugadores");
            var jugadoresSnapshot = await jugadoresRef.WhereIn("Id", jugadorIds).GetSnapshotAsync();
            var jugadoresDict = jugadoresSnapshot.Documents.ToDictionary(d => d.GetValue<string>("Id"), d => d.GetValue<string>("Nombre"));
            return list.Select(c => new RankingJugadorDto
            {
                Posicion = c.Posicion,
                NombreJugador = jugadoresDict.ContainsKey(c.JugadorId) ? jugadoresDict[c.JugadorId] : string.Empty,
                Puntos = c.PuntosJuego,
                Nivel = c.NivelJuego,
                RatioVictoria = c.RatioVictoria,
                TotalPartidas = c.TotalPartidas,
                RachaActual = c.Racha
            });
        }

        public async Task<MiClasificacionDto?> GetMiClasificacion(string juegoId, string userId)
        {
            var clasificacionesRef = _firebaseService.GetCollection("clasificaciones");
            var snapshot = await clasificacionesRef.WhereEqualTo("JuegoId", juegoId).GetSnapshotAsync();
            var lista = snapshot.Documents.Select(d => d.ConvertTo<Clasificacion>()).OrderBy(c => c.Posicion).ToList();
            var miClasificacion = lista.FirstOrDefault(c => c.JugadorId == userId);
            if (miClasificacion == null)
                return null;
            return new MiClasificacionDto
            {
                Rank = miClasificacion.Posicion,
                Puntos = miClasificacion.PuntosJuego,
                Nivel = miClasificacion.NivelJuego,
                Medallas = new MedallasDto
                {
                    Oro = miClasificacion.MedallasOro,
                    Plata = miClasificacion.MedallaPlata,
                    Bronce = miClasificacion.MedallaBronce
                },
                Logros = miClasificacion.Logros ?? new List<string>()
            };
        }
    }
}
