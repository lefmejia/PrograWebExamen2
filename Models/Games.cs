using Google.Cloud.Firestore;
namespace PlataformJuegoTorneo.Models
{
    [FirestoreData]
    public class Games
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Titulo { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Desarrollador { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Genero { get; set; } = string.Empty;

        [FirestoreProperty]
        public List<string> Plataformas { get; set; } = new List<string>();

        [FirestoreProperty]
        public DateTime FechaLanzamiento { get; set; }

        [FirestoreProperty]
        public string Descripcion { get; set; } = string.Empty;

        [FirestoreProperty]
        public int JugadoresActivos { get; set; }

        [FirestoreProperty]
        public int TorneoActivos { get; set; }

        [FirestoreProperty]
        public string Estado { get; set; } = string.Empty;

        [FirestoreProperty]
        public double PuntuacionPromedio { get; set; }

        [FirestoreProperty]
        public DateTime FechaAgreg { get; set; } = DateTime.UtcNow;
    }
}
