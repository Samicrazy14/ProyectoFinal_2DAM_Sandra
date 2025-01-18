using AppProyectoFinal.Resources.Languages;
using System.ComponentModel;
using System.Globalization;

namespace AppProyectoFinal
{
    /// <summary>
    /// Clase para la gestión de los recursos de idiomas.
    /// </summary>
    public class LocalizationResourceManager : INotifyPropertyChanged
    {
        /// <summary>
        /// Constructor para la gestión de los recursos de idiomas.
        /// </summary>
        private LocalizationResourceManager()
        {
            AppResources.Culture = CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Obtiene o establece la instacia del idioma actual.
        /// </summary>
        public static LocalizationResourceManager Instance { get; } = new();

        /// <summary>
        /// Variable para obtener el idioma actual de la aplicación.
        /// </summary>
        public object this[string resourceKey]
            => AppResources.ResourceManager.GetObject(resourceKey, AppResources.Culture) ?? Array.Empty<byte>();

        /// <summary>
        /// Variable para detectar cambios en el idioma de la aplicación.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Método para cambiar el idioma de la aplicación.
        /// <list type="number">
        /// <item><param name="culture">Objeto del tipo <em>CultureInfo</em> con el nuevo idioma de la aplicación.</param></item>
        /// </list>
        /// </summary>
        public void SetCulture(CultureInfo culture)
        {
            AppResources.Culture = culture;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
