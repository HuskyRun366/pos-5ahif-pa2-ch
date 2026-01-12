using System.Collections.Generic;
using System.Xml.Linq;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Services
{
    /// <summary>
    /// Lädt Spielfeld-Definitionen aus XML-Dateien.
    /// Das XML-Format enthält Breite, Höhe und eine Liste von Zellen.
    /// </summary>
    public class XmlFieldLoader
    {
        /// <summary>Breite des geladenen Spielfelds in Zellen</summary>
        public int Width { get; private set; }

        /// <summary>Höhe des geladenen Spielfelds in Zellen</summary>
        public int Height { get; private set; }

        /// <summary>Liste aller geladenen Elemente</summary>
        public List<RobotElement> Elements { get; private set; } = new();

        /// <summary>
        /// Lädt ein Spielfeld aus einer XML-Datei.
        /// </summary>
        /// <param name="filePath">Pfad zur XML-Datei</param>
        /// <remarks>
        /// Das erwartete XML-Format ist:
        /// <code>
        /// &lt;Root&gt;
        ///   &lt;Width&gt;10&lt;/Width&gt;
        ///   &lt;Height&gt;10&lt;/Height&gt;
        ///   &lt;Fields&gt;
        ///     &lt;XML_Cell&gt;
        ///       &lt;X&gt;1&lt;/X&gt;
        ///       &lt;Y&gt;1&lt;/Y&gt;
        ///       &lt;Type&gt;robot&lt;/Type&gt;
        ///     &lt;/XML_Cell&gt;
        ///   &lt;/Fields&gt;
        /// &lt;/Root&gt;
        /// </code>
        /// </remarks>
        public void Load(string filePath)
        {
            // Vorherige Elemente löschen
            Elements.Clear();

            // XML-Datei laden und parsen
            var doc = XDocument.Load(filePath);
            var root = doc.Root;

            // Abbrechen falls kein Root-Element vorhanden
            if (root == null) return;

            // Spielfeld-Dimensionen auslesen (Standard: 10x10)
            Width = int.Parse(root.Element("Width")?.Value ?? "10");
            Height = int.Parse(root.Element("Height")?.Value ?? "10");

            // Fields-Container suchen
            var fields = root.Element("Fields");
            if (fields == null) return;

            // Alle Zellen durchlaufen und in Elemente umwandeln
            foreach (var cell in fields.Elements("XML_Cell"))
            {
                // Position aus XML auslesen
                int x = int.Parse(cell.Element("X")?.Value ?? "0");
                int y = int.Parse(cell.Element("Y")?.Value ?? "0");
                string typeStr = cell.Element("Type")?.Value ?? "";

                // Neues Element mit Position erstellen
                var element = new RobotElement { X = x, Y = y };

                // Typ-String in ElementType umwandeln
                switch (typeStr.ToLower())
                {
                    case "robot":
                        // Roboter-Element
                        element.Type = ElementType.Robot;
                        break;

                    case "stone":
                        // Hindernis (Stein)
                        element.Type = ElementType.Obstacle;
                        break;

                    default:
                        // Prüfe ob es ein einzelner Buchstabe ist
                        if (typeStr.Length == 1 && char.IsLetter(typeStr[0]))
                        {
                            // Buchstabe zum Sammeln
                            element.Type = ElementType.Letter;
                            element.Letter = typeStr.ToUpper();  // Großbuchstabe speichern
                        }
                        else
                        {
                            // Alles andere als Wand behandeln
                            element.Type = ElementType.Wall;
                        }
                        break;
                }

                // Element zur Liste hinzufügen
                Elements.Add(element);
            }
        }
    }
}
