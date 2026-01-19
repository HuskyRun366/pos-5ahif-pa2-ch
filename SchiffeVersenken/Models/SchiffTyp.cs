namespace SchiffeVersenken.Models
{
    /// <summary>
    /// Schiffstypen mit ihren Längen gemäß offiziellen Regeln.
    /// </summary>
    public enum SchiffTyp
    {
        /// <summary>Schlachtschiff - 5 Felder</summary>
        Schlachtschiff = 5,

        /// <summary>Kreuzer - 4 Felder</summary>
        Kreuzer = 4,

        /// <summary>Zerstoerer - 3 Felder</summary>
        Zerstoerer = 3,

        /// <summary>U-Boot - 2 Felder</summary>
        UBoot = 2
    }
}
