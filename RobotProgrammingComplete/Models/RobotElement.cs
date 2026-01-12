namespace RobotProgrammingComplete.Models
{
    public enum ElementType
    {
        Obstacle,
        Wall,
        Letter,
        Robot
    }

    public class RobotElement
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public string Letter { get; set; } = "";
        public ElementType Type { get; set; } = ElementType.Wall;
    }
}
