using System.Collections.Generic;
using System.Xml.Linq;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Services
{
    public class XmlFieldLoader
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public List<RobotElement> Elements { get; private set; } = new();

        public void Load(string filePath)
        {
            Elements.Clear();

            var doc = XDocument.Load(filePath);
            var root = doc.Root;

            if (root == null) return;

            Width = int.Parse(root.Element("Width")?.Value ?? "10");
            Height = int.Parse(root.Element("Height")?.Value ?? "10");

            var fields = root.Element("Fields");
            if (fields == null) return;

            foreach (var cell in fields.Elements("XML_Cell"))
            {
                int x = int.Parse(cell.Element("X")?.Value ?? "0");
                int y = int.Parse(cell.Element("Y")?.Value ?? "0");
                string typeStr = cell.Element("Type")?.Value ?? "";

                var element = new RobotElement { X = x, Y = y };

                switch (typeStr.ToLower())
                {
                    case "robot":
                        element.Type = ElementType.Robot;
                        break;
                    case "stone":
                        element.Type = ElementType.Obstacle;
                        break;
                    default:
                        if (typeStr.Length == 1 && char.IsLetter(typeStr[0]))
                        {
                            element.Type = ElementType.Letter;
                            element.Letter = typeStr.ToUpper();
                        }
                        else
                        {
                            element.Type = ElementType.Wall;
                        }
                        break;
                }

                Elements.Add(element);
            }
        }
    }
}
