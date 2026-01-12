using System.Windows;

namespace RobotLanguage
{
    /// <summary>
    /// Aufgabe 5: Basisklasse für alle Ausdrücke im Syntax-Baum
    /// Diese Klasse ist bereits vorbereitet.
    /// </summary>
    public abstract class Expression
    {
        /// <summary>
        /// Führt den Ausdruck aus (wird vom Interpreter aufgerufen)
        /// </summary>
        public abstract void Execute(RobotCanvas canvas);
    }

    /// <summary>
    /// Aufgabe 5: Programm - enthält eine Liste von Anweisungen
    /// </summary>
    public class ProgramExpression : Expression
    {
        public List<Expression> Statements { get; } = new List<Expression>();

        public override void Execute(RobotCanvas canvas)
        {
            foreach (var statement in Statements)
            {
                statement.Execute(canvas);
            }
        }
    }

    /// <summary>
    /// Aufgabe 5: FORWARD-Befehl
    /// </summary>
    public class ForwardExpression : Expression
    {
        public double Distance { get; set; }

        public ForwardExpression(double distance)
        {
            Distance = distance;
        }

        public override void Execute(RobotCanvas canvas)
        {
            double angleRad = canvas.CurrentAngle * Math.PI / 180;
            double newX = canvas.CurrentPosition.X + Distance * Math.Cos(angleRad);
            double newY = canvas.CurrentPosition.Y + Distance * Math.Sin(angleRad);
            Point newPos = new Point(newX, newY);
            canvas.AddLine(canvas.CurrentPosition, newPos);
            canvas.CurrentPosition = newPos;
        }
    }

    /// <summary>
    /// Aufgabe 5: BACKWARD-Befehl
    /// </summary>
    public class BackwardExpression : Expression
    {
        public double Distance { get; set; }

        public BackwardExpression(double distance)
        {
            Distance = distance;
        }

        public override void Execute(RobotCanvas canvas)
        {
            double angleRad = canvas.CurrentAngle * Math.PI / 180;
            double newX = canvas.CurrentPosition.X - Distance * Math.Cos(angleRad);
            double newY = canvas.CurrentPosition.Y - Distance * Math.Sin(angleRad);
            Point newPos = new Point(newX, newY);
            canvas.AddLine(canvas.CurrentPosition, newPos);
            canvas.CurrentPosition = newPos;
        }
    }

    /// <summary>
    /// Aufgabe 5: LEFT-Befehl (nach links drehen)
    /// </summary>
    public class LeftExpression : Expression
    {
        public double Angle { get; set; }

        public LeftExpression(double angle)
        {
            Angle = angle;
        }

        public override void Execute(RobotCanvas canvas)
        {
            canvas.CurrentAngle -= Angle;
        }
    }

    /// <summary>
    /// Aufgabe 5: RIGHT-Befehl (nach rechts drehen)
    /// </summary>
    public class RightExpression : Expression
    {
        public double Angle { get; set; }

        public RightExpression(double angle)
        {
            Angle = angle;
        }

        public override void Execute(RobotCanvas canvas)
        {
            canvas.CurrentAngle += Angle;
        }
    }
}
