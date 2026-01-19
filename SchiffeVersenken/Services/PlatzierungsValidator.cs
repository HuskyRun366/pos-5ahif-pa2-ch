using SchiffeVersenken.Models;

namespace SchiffeVersenken.Services
{
    /// <summary>
    /// Validiert die Platzierung von Schiffen gemäß offiziellen Regeln.
    /// </summary>
    public class PlatzierungsValidator
    {
        /// <summary>
        /// Prüft ob ein Schiff an der angegebenen Position gültig platziert werden kann.
        /// </summary>
        /// <param name="spielfeld">Das Spielfeld</param>
        /// <param name="schiff">Das zu platzierende Schiff</param>
        /// <returns>True wenn die Platzierung gültig ist</returns>
        public bool IstGueltigePlatzierung(Spielfeld spielfeld, Schiff schiff)
        {
            // 1. Prüfen ob alle Positionen innerhalb des Spielfelds liegen
            foreach (var (x, y) in schiff.GetPositionen())
            {
                if (x < 0 || x >= Spielfeld.Groesse || y < 0 || y >= Spielfeld.Groesse)
                {
                    return false; // Außerhalb des Spielfelds
                }
            }

            // 2. Prüfen ob Schiff mit existierenden Schiffen kollidiert
            //    (auch diagonal - Schiffe dürfen sich nicht berühren!)
            var gesperrteZellen = GetGesperrteZellen(spielfeld);

            foreach (var (x, y) in schiff.GetPositionen())
            {
                if (gesperrteZellen.Contains((x, y)))
                {
                    return false; // Kollidiert mit Sperrzone
                }
            }

            return true;
        }

        /// <summary>
        /// Ermittelt alle Zellen die durch existierende Schiffe gesperrt sind.
        /// Inkludiert die Schiffspositionen selbst PLUS alle benachbarten Felder (auch diagonal).
        /// </summary>
        private HashSet<(int, int)> GetGesperrteZellen(Spielfeld spielfeld)
        {
            var gesperrt = new HashSet<(int, int)>();

            foreach (var schiff in spielfeld.Schiffe)
            {
                // Alle Zellen in der Sperrzone des Schiffs
                foreach (var pos in schiff.GetSperrzone())
                {
                    gesperrt.Add(pos);
                }
            }

            return gesperrt;
        }

        /// <summary>
        /// Prüft ob alle erforderlichen Schiffe platziert wurden.
        /// </summary>
        public bool SindAlleSchiffePlatziert(Spielfeld spielfeld)
        {
            // Erforderlich: 1x5, 1x4, 2x3, 1x2
            var schiffTypen = spielfeld.Schiffe.GroupBy(s => s.Typ)
                                               .ToDictionary(g => g.Key, g => g.Count());

            return schiffTypen.GetValueOrDefault(SchiffTyp.Schlachtschiff) >= 1 &&
                   schiffTypen.GetValueOrDefault(SchiffTyp.Kreuzer) >= 1 &&
                   schiffTypen.GetValueOrDefault(SchiffTyp.Zerstoerer) >= 2 &&
                   schiffTypen.GetValueOrDefault(SchiffTyp.UBoot) >= 1;
        }

        /// <summary>
        /// Gibt eine Vorschau welche Zellen ein Schiff belegen würde.
        /// Für Hover-Effekte während der Platzierung.
        /// </summary>
        public (List<(int X, int Y)> Positionen, bool IstGueltig) GetPlatzierungsVorschau(
            Spielfeld spielfeld, SchiffTyp typ, int startX, int startY, bool horizontal)
        {
            var schiff = new Schiff
            {
                Typ = typ,
                StartX = startX,
                StartY = startY,
                Horizontal = horizontal
            };

            var positionen = schiff.GetPositionen().ToList();
            var istGueltig = IstGueltigePlatzierung(spielfeld, schiff);

            return (positionen, istGueltig);
        }
    }
}
