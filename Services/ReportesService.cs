using PlataformJuegoTorneo.Models;
using PlataformJuegoTorneo.DTOs;
using Google.Cloud.Firestore;

namespace PlataformJuegoTorneo.Services
{
    public class ReportesService
    {
        private readonly FirebaseService _firebaseService;
        private readonly ILogger<ReportesService> _logger;

        public ReportesService(FirebaseService firebaseService, ILogger<ReportesService> logger)
        {
            _firebaseService = firebaseService;
            _logger = logger;
        }

        public async Task<IEnumerable<TorneoPopularDto>> GetTorneosPopulares()
        {
            var torneosRef = _firebaseService.GetCollection("torneos");
            var participacionesRef = _firebaseService.GetCollection("participaciones");
            var fechaLimite = DateTime.UtcNow.AddDays(-30);
            var participacionesSnap = await participacionesRef.WhereGreaterThanOrEqualTo("FechaInscripcion", fechaLimite).GetSnapshotAsync();
            var inscripcionesPorTorneo = participacionesSnap.Documents
                .GroupBy(d => d.GetValue<string>("TorneoId"))
                .Select(g => new { TorneoId = g.Key, Cantidad = g.Count() })
                .OrderByDescending(x => x.Cantidad)
                .Take(10)
                .ToList();
            var torneoIds = inscripcionesPorTorneo.Select(x => x.TorneoId).ToList();
            if (torneoIds.Count == 0)
                return new List<TorneoPopularDto>();
            var torneosSnap = await torneosRef.WhereIn("Id", torneoIds).GetSnapshotAsync();
            var torneosDict = torneosSnap.Documents.ToDictionary(d => d.GetValue<string>("Id"), d => d);
            return inscripcionesPorTorneo.Select(x => new TorneoPopularDto
            {
                Nombre = torneosDict.ContainsKey(x.TorneoId) ? torneosDict[x.TorneoId].GetValue<string>("Nombre") : string.Empty,
                Juego = torneosDict.ContainsKey(x.TorneoId) ? torneosDict[x.TorneoId].GetValue<string>("Juego") : string.Empty,
                CantidadInscripciones = x.Cantidad,
                PremioTotal = torneosDict.ContainsKey(x.TorneoId) ? torneosDict[x.TorneoId].GetValue<decimal>("PremioTotal") : 0,
                Estado = torneosDict.ContainsKey(x.TorneoId) ? torneosDict[x.TorneoId].GetValue<string>("Estado") : string.Empty
            });
        }


        public async Task<IEnumerable<JugadorDestacadoDto>> GetJugadoresDestacados()
        {
            var jugadoresRef = _firebaseService.GetCollection("jugadores");
            var snapshot = await jugadoresRef.GetSnapshotAsync();
            var jugadores = snapshot.Documents
                .Select(d => d.ConvertTo<Jugador>())
                .OrderByDescending(j => j.PuntosGlobales)
                .Take(20)
                .ToList();
            var participacionesRef = _firebaseService.GetCollection("participaciones");
            var participacionesSnap = await participacionesRef.GetSnapshotAsync();
            var juegosPorJugador = participacionesSnap.Documents
                .GroupBy(d => d.GetValue<string>("JugadorId"))
                .ToDictionary(g => g.Key, g => g.Select(x => x.GetValue<string>("TorneoId")).Distinct().Count());
            return jugadores.Select(j => new JugadorDestacadoDto
            {
                Nombre = j.Nombre,
                PuntosGlobales = j.PuntosGlobales,
                TorneosGanados = j.TorneoGanados,
                CantidadJuegos = juegosPorJugador.ContainsKey(j.Id.ToString()) ? juegosPorJugador[j.Id.ToString()] : 0
            });
        }


        public async Task<DesempenoDto?> GetMiDesempeno(string userId, string juegoId)
        {
            var clasificacionesRef = _firebaseService.GetCollection("clasificaciones");
            var snapshot = await clasificacionesRef.WhereEqualTo("JuegoId", juegoId).GetSnapshotAsync();
            var lista = snapshot.Documents.Select(d => d.ConvertTo<Clasificacion>()).OrderBy(c => c.Posicion).ToList();
            var miClasificacion = lista.FirstOrDefault(c => c.JugadorId == userId);
            if (miClasificacion == null)
                return null;
            int posicion = miClasificacion.Posicion;
            int nivel = miClasificacion.NivelJuego;
            double progreso = (nivel < 100) ? (miClasificacion.PuntosJuego % 100) / 100.0 : 1.0;
            var mejoresTorneos = lista.Where(c => c.JugadorId == userId)
                .OrderByDescending(c => c.PuntosJuego)
                .Take(3)
                .Select(c => new MejorTorneoDto { PuntosJuego = c.PuntosJuego, Posicion = c.Posicion });
            return new DesempenoDto
            {
                NivelActual = nivel,
                PosicionRanking = posicion,
                ProgresoSiguienteNivel = progreso,
                RatioVictoria = miClasificacion.RatioVictoria,
                RachaActual = miClasificacion.Racha,
                Medallas = new MedallasDto
                {
                    Oro = miClasificacion.MedallasOro,
                    Plata = miClasificacion.MedallaPlata,
                    Bronce = miClasificacion.MedallaBronce
                },
                MejoresTorneos = mejoresTorneos,
                Logros = miClasificacion.Logros ?? new List<string>()
            };
        }

        public async Task<TendenciasDto> GetTendencias()
        {
            var juegosRef = _firebaseService.GetCollection("juegos");
            var torneosRef = _firebaseService.GetCollection("torneos");
            var juegosSnap = await juegosRef.GetSnapshotAsync();
            var torneosSnap = await torneosRef.GetSnapshotAsync();
            // Juegos más populares (top 5 por jugadoresActivos)
            var juegosPopulares = juegosSnap.Documents
                .Select(d => d.ConvertTo<Juego>())
                .OrderByDescending(j => j.JugadoresActivos)
                .Take(5)
                .Select(j => new JuegoPopularDto { Titulo = j.Titulo, JugadoresActivos = j.JugadoresActivos });
            // Géneros con más torneos activos
            var torneosActivos = torneosSnap.Documents
                .Where(d => d.GetValue<string>("Estado") == "en progreso")
                .Select(d => d.GetValue<string>("Juego"));
            var generos = juegosSnap.Documents
                .Where(d => torneosActivos.Contains(d.GetValue<string>("Id")))
                .GroupBy(d => d.GetValue<string>("Genero"))
                .Select(g => new GeneroTorneoDto { Genero = g.Key, Cantidad = g.Count() })
                .OrderByDescending(g => g.Cantidad)
                .ToList();
            return new TendenciasDto
            {
                JuegosPopulares = juegosPopulares,
                GenerosConMasTorneos = generos
            };
        }
    }
}
