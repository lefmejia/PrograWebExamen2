using Google.Cloud.Firestore;

namespace PlataformJuegoTorneo.Models
{
    [FirestoreData]
    public class Participacion
    {
        [FirestoreDocumentId] public string Id { get; set; }
        [FirestoreProperty] public string jugadorId { get; set; }
        [FirestoreProperty] public string torneoId { get; set; }
        [FirestoreProperty] public string estado { get; set; } = "inscrito";
        [FirestoreProperty] public int victorias { get; set; } = 0;
        [FirestoreProperty] public int derrotas { get; set; } = 0;
        [FirestoreProperty] public int puntosObtenidos { get; set; } = 0;
        [FirestoreProperty] public int partidasJugadas { get; set; } = 0;
        [FirestoreProperty] public bool pagado { get; set; }
        [FirestoreProperty] public DateTime fechaInscripcion { get; set; } = DateTime.UtcNow;
    }
}