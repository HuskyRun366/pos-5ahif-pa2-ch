namespace SchiffeVersenken.Models
{
    /// <summary>
    /// Aktuelle Phase des Spiels.
    /// </summary>
    public enum SpielPhase
    {
        /// <summary>Schiffe werden platziert</summary>
        Platzierung,

        /// <summary>Warten auf Gegner (Verbindung oder dessen Platzierung)</summary>
        Warten,

        /// <summary>Eigener Zug - Spieler kann schießen</summary>
        AmZug,

        /// <summary>Gegner ist am Zug</summary>
        GegnerAmZug,

        /// <summary>Spiel beendet</summary>
        Ende
    }
}
