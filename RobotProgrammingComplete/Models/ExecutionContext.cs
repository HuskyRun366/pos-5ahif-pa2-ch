using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotProgrammingComplete.Models
{
    public class ExecutionContext
    {
        public int RobotX { get; set; }
        public int RobotY { get; set; }
        public int FieldWidth { get; set; }
        public int FieldHeight { get; set; }
        public List<RobotElement> Elements { get; set; } = new();
        public List<string> CollectedLetters { get; set; } = new();
        public Action? OnUpdate { get; set; }
        public int DelayMs { get; set; } = 1000;

        public RobotElement? GetElementAt(int x, int y)
        {
            return Elements.FirstOrDefault(e => e.X == x && e.Y == y);
        }

        public bool IsObstacle(int x, int y)
        {
            // Wall boundary check
            if (x <= 0 || x >= FieldWidth - 1 || y <= 0 || y >= FieldHeight - 1)
                return true;
            var element = GetElementAt(x, y);
            return element?.Type == ElementType.Obstacle;
        }

        public bool IsLetter(int x, int y, string letter)
        {
            var element = GetElementAt(x, y);
            return element?.Type == ElementType.Letter && element.Letter == letter;
        }

        public (int dx, int dy) GetDirectionOffset(Direction direction)
        {
            return direction switch
            {
                Direction.Up => (0, -1),
                Direction.Down => (0, 1),
                Direction.Left => (-1, 0),
                Direction.Right => (1, 0),
                _ => (0, 0)
            };
        }

        public (int x, int y) GetPositionInDirection(Direction direction)
        {
            var (dx, dy) = GetDirectionOffset(direction);
            return (RobotX + dx, RobotY + dy);
        }
    }
}
