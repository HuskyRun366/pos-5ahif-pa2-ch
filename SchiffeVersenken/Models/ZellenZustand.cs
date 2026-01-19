namespace SchiffeVersenken.Models
{
    /// <summary>
    /// Mögliche Zustände einer Zelle auf dem Spielfeld.
    /// </summary>
    public enum ZellenZustand
    {
        /// <summary>Unbekannt - Zelle wurde noch nicht beschossen (Gegnerfeld-Ansicht)</summary>
        Unbekannt,

        /// <summary>Wasser - Leere Zelle ohne Schiff</summary>
        Wasser,

        /// <summary>Schiff - Zelle enthält ein Schiffsteil</summary>
        Schiff,

        /// <summary>Treffer - Schiffsteil wurde getroffen</summary>
        Treffer,

        /// <summary>Daneben - Schuss ins Wasser</summary>
        Daneben
    }
}
