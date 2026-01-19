using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SchiffeVersenken.ViewModels;

namespace SchiffeVersenken.Controls
{
    /// <summary>
    /// Benutzerdefiniertes Control zur Darstellung des 10x10 Spielfelds.
    /// </summary>
    public partial class SpielfeldControl : UserControl
    {
        /// <summary>
        /// Event wenn eine Zelle geklickt wird.
        /// </summary>
        public event EventHandler<ZellenKlickEventArgs>? ZellenGeklickt;

        /// <summary>
        /// Event wenn die Maus über eine Zelle fährt (für Vorschau).
        /// </summary>
        public event EventHandler<ZellenKlickEventArgs>? ZellenHover;

        /// <summary>
        /// Event wenn die Maus die Zelle verlässt.
        /// </summary>
        public event EventHandler? ZellenHoverEnde;

        public SpielfeldControl()
        {
            InitializeComponent();
        }

        private void Zelle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ZellenViewModel zelle)
            {
                ZellenGeklickt?.Invoke(this, new ZellenKlickEventArgs(zelle.X, zelle.Y));
            }
        }

        private void Zelle_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ZellenViewModel zelle)
            {
                ZellenHover?.Invoke(this, new ZellenKlickEventArgs(zelle.X, zelle.Y));
            }
        }

        private void Zelle_MouseLeave(object sender, MouseEventArgs e)
        {
            ZellenHoverEnde?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// EventArgs für Zellen-Klick-Events.
    /// </summary>
    public class ZellenKlickEventArgs : EventArgs
    {
        public int X { get; }
        public int Y { get; }

        public ZellenKlickEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
