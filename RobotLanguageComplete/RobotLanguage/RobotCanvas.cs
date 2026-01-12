using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RobotLanguage
{
    /// <summary>
    /// Aufgabe 1: Custom Widget für das Zeichnen von Kreisen und Linien
    /// Die Schüler müssen OnRender vervollständigen, um Kreise und Linien darzustellen.
    /// </summary>
    public class RobotCanvas : Control
    {
        // Listen für die zu zeichnenden Elemente
        private List<CircleData> _circles = new List<CircleData>();
        private List<LineData> _lines = new List<LineData>();

        // Aktuelle Position und Richtung des Roboters
        public Point CurrentPosition { get; set; } = new Point(50, 300);
        public double CurrentAngle { get; set; } = 0; // 0 = nach rechts, 90 = nach unten

        // Farben für die Darstellung
        private static readonly Brush ObstacleBrush = Brushes.Red;
        private static readonly Brush GoalBrush = Brushes.Green;
        private static readonly Brush LineBrush = Brushes.Blue;
        private static readonly Brush RobotBrush = Brushes.Orange;

        public RobotCanvas()
        {
            // Standardwerte setzen
        }

        #region Aufgabe 1: OnRender vervollständigen

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // Hintergrund zeichnen
            drawingContext.DrawRectangle(Background ?? Brushes.White, null, new Rect(0, 0, ActualWidth, ActualHeight));

            // TODO: Aufgabe 1 - Kreise zeichnen (siehe Aufgabe 3)
            // Hinweis: Durchlaufe _circles und zeichne jeden Kreis
            // Verwende drawingContext.DrawEllipse(brush, pen, center, radiusX, radiusY)
            // Unterscheide zwischen Hindernissen (rot) und Ziel (grün) anhand von IsGoal
            
            foreach (var circle in _circles)
            {
                Brush brush = circle.IsGoal ? GoalBrush : ObstacleBrush;
                Pen pen = new Pen(Brushes.Black, 2);
                drawingContext.DrawEllipse(brush, pen, new Point(circle.X, circle.Y), circle.Radius, circle.Radius);
            }

            // TODO: Aufgabe 1 - Linien zeichnen (siehe Aufgabe 6)
            // Hinweis: Durchlaufe _lines und zeichne jede Linie
            // Verwende drawingContext.DrawLine(pen, startPoint, endPoint)
            
            foreach (var line in _lines)
            {
                Pen pen = new Pen(LineBrush, 2);
                drawingContext.DrawLine(pen, line.Start, line.End);
            }

            // Roboter-Position zeichnen (kleiner Pfeil/Dreieck)
            DrawRobot(drawingContext);
        }

        /// <summary>
        /// Zeichnet den Roboter als kleines Dreieck an der aktuellen Position
        /// Diese Funktion ist bereits vorbereitet.
        /// </summary>
        private void DrawRobot(DrawingContext dc)
        {
            double size = 15;
            double angleRad = CurrentAngle * Math.PI / 180;

            // Dreieck-Punkte berechnen
            Point tip = new Point(
                CurrentPosition.X + size * Math.Cos(angleRad),
                CurrentPosition.Y + size * Math.Sin(angleRad)
            );
            Point left = new Point(
                CurrentPosition.X + size * 0.7 * Math.Cos(angleRad + 2.5),
                CurrentPosition.Y + size * 0.7 * Math.Sin(angleRad + 2.5)
            );
            Point right = new Point(
                CurrentPosition.X + size * 0.7 * Math.Cos(angleRad - 2.5),
                CurrentPosition.Y + size * 0.7 * Math.Sin(angleRad - 2.5)
            );

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(tip, true, true);
                ctx.LineTo(left, true, false);
                ctx.LineTo(right, true, false);
            }

            dc.DrawGeometry(RobotBrush, new Pen(Brushes.Black, 1), geometry);
        }

        #endregion

        #region Hilfsmethoden für Aufgabe 3 (XML) und Aufgabe 6 (Run)

        /// <summary>
        /// Fügt einen Kreis (Hindernis oder Ziel) hinzu
        /// </summary>
        public void AddCircle(double x, double y, double radius, bool isGoal)
        {
            _circles.Add(new CircleData { X = x, Y = y, Radius = radius, IsGoal = isGoal });
            InvalidateVisual();
        }

        /// <summary>
        /// Fügt eine Linie hinzu (wird beim Ausführen von FORWARD erstellt)
        /// </summary>
        public void AddLine(Point start, Point end)
        {
            _lines.Add(new LineData { Start = start, End = end });
            InvalidateVisual();
        }

        /// <summary>
        /// Aufgabe 3: Entfernt alle Inhalte aus dem Widget
        /// </summary>
        public void Clear()
        {
            _circles.Clear();
            _lines.Clear();
            CurrentPosition = new Point(50, 300);
            CurrentAngle = 0;
            InvalidateVisual();
        }

        /// <summary>
        /// Nur die Linien löschen (für neuen Run)
        /// </summary>
        public void ClearLines()
        {
            _lines.Clear();
            CurrentPosition = new Point(50, 300);
            CurrentAngle = 0;
            InvalidateVisual();
        }

        #endregion

        #region Aufgabe 7: Kollisionserkennung

        /// <summary>
        /// Aufgabe 7: Prüft ob eine Linie ein Hindernis (nicht-Ziel-Kreis) schneidet
        /// </summary>
        public bool CheckObstacleCollision()
        {
            foreach (var line in _lines)
            {
                foreach (var circle in _circles)
                {
                    if (!circle.IsGoal && LineIntersectsCircle(line, circle))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Aufgabe 7: Prüft ob der Roboter das Ziel erreicht hat
        /// </summary>
        public bool CheckGoalReached()
        {
            foreach (var circle in _circles)
            {
                if (circle.IsGoal && PointInCircle(CurrentPosition, circle))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Hilfsmethode: Prüft ob eine Linie einen Kreis schneidet
        /// Diese Funktion ist bereits vorbereitet.
        /// </summary>
        private bool LineIntersectsCircle(LineData line, CircleData circle)
        {
            // Vektor von Start zu End
            double dx = line.End.X - line.Start.X;
            double dy = line.End.Y - line.Start.Y;

            // Vektor von Start zu Kreismittelpunkt
            double fx = line.Start.X - circle.X;
            double fy = line.Start.Y - circle.Y;

            double a = dx * dx + dy * dy;
            double b = 2 * (fx * dx + fy * dy);
            double c = fx * fx + fy * fy - circle.Radius * circle.Radius;

            double discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
                return false;

            discriminant = Math.Sqrt(discriminant);

            double t1 = (-b - discriminant) / (2 * a);
            double t2 = (-b + discriminant) / (2 * a);

            return (t1 >= 0 && t1 <= 1) || (t2 >= 0 && t2 <= 1);
        }

        /// <summary>
        /// Hilfsmethode: Prüft ob ein Punkt innerhalb eines Kreises liegt
        /// Diese Funktion ist bereits vorbereitet.
        /// </summary>
        private bool PointInCircle(Point point, CircleData circle)
        {
            double dx = point.X - circle.X;
            double dy = point.Y - circle.Y;
            return Math.Sqrt(dx * dx + dy * dy) <= circle.Radius;
        }

        #endregion
    }

    #region Datenklassen

    /// <summary>
    /// Datenklasse für Kreise (Hindernisse und Ziel)
    /// </summary>
    public class CircleData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Radius { get; set; }
        public bool IsGoal { get; set; }
    }

    /// <summary>
    /// Datenklasse für Linien
    /// </summary>
    public class LineData
    {
        public Point Start { get; set; }
        public Point End { get; set; }
    }

    #endregion
}
