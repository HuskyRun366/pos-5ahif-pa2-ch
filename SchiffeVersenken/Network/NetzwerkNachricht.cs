using System.Text.Json;
using SchiffeVersenken.Models;

namespace SchiffeVersenken.Network
{
    /// <summary>
    /// Nachrichtentypen für das Netzwerkprotokoll.
    /// </summary>
    public enum NachrichtTyp
    {
        /// <summary>Verbindungsanfrage mit Spielername</summary>
        Verbinden,

        /// <summary>Verbindung akzeptiert</summary>
        VerbindungAkzeptiert,

        /// <summary>Spieler hat Schiffe platziert und ist bereit</summary>
        Bereit,

        /// <summary>Schuss auf eine Koordinate</summary>
        Schuss,

        /// <summary>Ergebnis eines Schusses</summary>
        SchussErgebnis,

        /// <summary>Ein Schiff wurde versenkt (inkl. Schiffstyp)</summary>
        SchiffVersenkt,

        /// <summary>Spiel gewonnen/verloren</summary>
        SpielEnde,

        /// <summary>Chat-Nachricht</summary>
        Chat,

        /// <summary>Ping für Verbindungstest</summary>
        Ping,

        /// <summary>Pong-Antwort</summary>
        Pong
    }

    /// <summary>
    /// Netzwerk-Nachricht für Kommunikation zwischen Spielern.
    /// </summary>
    public class NetzwerkNachricht
    {
        /// <summary>Typ der Nachricht</summary>
        public NachrichtTyp Typ { get; set; }

        /// <summary>X-Koordinate (für Schuss)</summary>
        public int X { get; set; }

        /// <summary>Y-Koordinate (für Schuss)</summary>
        public int Y { get; set; }

        /// <summary>Schussergebnis</summary>
        public SchussErgebnis Ergebnis { get; set; }

        /// <summary>Spielername</summary>
        public string SpielerName { get; set; } = string.Empty;

        /// <summary>Zusätzliche Daten (z.B. Chat-Text, Schiffstyp)</summary>
        public string Daten { get; set; } = string.Empty;

        /// <summary>
        /// Serialisiert die Nachricht zu JSON.
        /// </summary>
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        /// <summary>
        /// Deserialisiert eine Nachricht aus JSON.
        /// </summary>
        public static NetzwerkNachricht? FromJson(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<NetzwerkNachricht>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}
