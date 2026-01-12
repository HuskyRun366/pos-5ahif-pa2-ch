using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RobotProgrammingComplete.Models;

namespace RobotProgrammingComplete.Controls
{
    public partial class RobotMap : UserControl
    {
        private int _width = 10;
        private int _height = 10;
        private List<RobotElement> _elements = new();

        private readonly BitmapImage _wallImage;
        private readonly BitmapImage _robotImage;
        private readonly BitmapImage _obstacleImage;

        public RobotMap()
        {
            InitializeComponent();

            _wallImage = new BitmapImage(new Uri("pack://application:,,,/Resources/wall.jpg"));
            _robotImage = new BitmapImage(new Uri("pack://application:,,,/Resources/robot.png"));
            _obstacleImage = new BitmapImage(new Uri("pack://application:,,,/Resources/stone.png"));
        }

        public void SetSize(int x, int y)
        {
            _width = x;
            _height = y;
            Redraw();
        }

        public void SetElements(List<RobotElement> list)
        {
            _elements = list;
            Redraw();
        }

        public void ClearLevel()
        {
            SetSize(5, 5);
            SetElements(new List<RobotElement>());
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Redraw();
        }

        public void Redraw()
        {
            canvas.Children.Clear();

            double pw = ActualWidth;
            double ph = ActualHeight;

            if (pw <= 0 || ph <= 0) return;

            // Aspect ratio calculation
            double pa = pw / ph;
            double a = (double)_width / _height;

            if (pa > a)
            {
                pw = ph * a;
            }
            else
            {
                ph = pw / a;
            }

            double d = pw / _width;

            // Draw border walls
            for (int i = 0; i < _width; i++)
            {
                DrawImage(_wallImage, i * d, 0, d, d);
                DrawImage(_wallImage, i * d, ph - d, d, d);
            }
            for (int j = 1; j < _height - 1; j++)
            {
                DrawImage(_wallImage, 0, j * d, d, d);
                DrawImage(_wallImage, pw - d, j * d, d, d);
            }

            // Draw elements
            foreach (var element in _elements)
            {
                switch (element.Type)
                {
                    case ElementType.Robot:
                        DrawImage(_robotImage, element.X * d, element.Y * d, d, d);
                        break;
                    case ElementType.Wall:
                        DrawImage(_wallImage, element.X * d, element.Y * d, d, d);
                        break;
                    case ElementType.Obstacle:
                        DrawImage(_obstacleImage, element.X * d, element.Y * d, d, d);
                        break;
                    case ElementType.Letter:
                        DrawLetter(element.Letter, element.X * d, element.Y * d, d, d);
                        break;
                }
            }

            // Draw grid lines
            var pen = new SolidColorBrush(Colors.Black);
            for (int i = 0; i <= _width; i++)
            {
                var line = new Line
                {
                    X1 = i * d, Y1 = 0,
                    X2 = i * d, Y2 = ph,
                    Stroke = pen,
                    StrokeThickness = 2
                };
                canvas.Children.Add(line);
            }
            for (int j = 0; j <= _height; j++)
            {
                var line = new Line
                {
                    X1 = 0, Y1 = j * d,
                    X2 = pw, Y2 = j * d,
                    Stroke = pen,
                    StrokeThickness = 2
                };
                canvas.Children.Add(line);
            }
        }

        private void DrawImage(BitmapImage image, double x, double y, double width, double height)
        {
            var img = new Image
            {
                Source = image,
                Width = width,
                Height = height
            };
            Canvas.SetLeft(img, x);
            Canvas.SetTop(img, y);
            canvas.Children.Add(img);
        }

        private void DrawLetter(string letter, double x, double y, double width, double height)
        {
            var text = new TextBlock
            {
                Text = letter,
                FontSize = Math.Max(10, height - 5),
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black,
                Width = width,
                Height = height,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var border = new Border
            {
                Width = width,
                Height = height,
                Child = text
            };

            Canvas.SetLeft(border, x);
            Canvas.SetTop(border, y);
            canvas.Children.Add(border);
        }
    }
}
