using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SchiffeVersenken.ViewModels
{
    /// <summary>
    /// Basisklasse für alle ViewModels mit INotifyPropertyChanged-Implementierung.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Benachrichtigt die UI über Property-Änderungen.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Setzt eine Property und löst bei Änderung das PropertyChanged-Event aus.
        /// </summary>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
