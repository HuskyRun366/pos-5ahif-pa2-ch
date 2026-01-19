namespace SchiffeVersenken.Models
{
    /// <summary>
    /// Ergebnis eines Schusses.
    /// </summary>
    public enum SchussErgebnis
    {
        /// <summary>Ungültiger Schuss (bereits beschossen oder außerhalb)</summary>
        Ungueltig,

        /// <summary>Warte auf Antwort vom Gegner</summary>
        Warten,

        /// <summary>Schuss ins Wasser</summary>
        Daneben,

        /// <summary>Schiffsteil getroffen</summary>
        Treffer,

        /// <summary>Schiff komplett versenkt</summary>
        Versenkt
    }
}
