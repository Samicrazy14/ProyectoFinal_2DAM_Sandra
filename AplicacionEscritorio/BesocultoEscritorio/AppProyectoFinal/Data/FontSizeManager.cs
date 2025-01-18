
using System.ComponentModel;

namespace AppProyectoFinal.Data
{
    /// <summary>
    /// Manejador del tamaño de fuente que permite cambiar entre los valores definidos.
    /// </summary>
    public class FontSizeManager : INotifyPropertyChanged
    {
        private static FontSizeManager _instance;
        public static FontSizeManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FontSizeManager();
                return _instance;
            }
        }

        // Eventos para notificar cambios
        public event PropertyChangedEventHandler PropertyChanged;

        // Propiedades para los tamaños
        private double _sizeTitulo = 28; // valor predeterminado
        private double _sizeTexto = 15;  // valor predeterminado

        //GETS AND SETS
        public double SizeTitulo
        {
            get => _sizeTitulo;
            set
            {
                if (_sizeTitulo != value)
                {
                    _sizeTitulo = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SizeTitulo)));
                }
            }
        }
        public double SizeTexto
        {
            get => _sizeTexto;
            set
            {
                if (_sizeTexto != value)
                {
                    _sizeTexto = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SizeTexto)));
                }
            }
        }

        /// <summary>
        /// Método para cambiar el tamaño de fuente entre las 3 opciones posibles.
        /// <list type="number">
        /// <param name="selectedSize">Opción a la que se desea convertir (0: Pequeña, 1: Mediana, 2: Grande).</param>
        /// </list>
        /// </summary>
        public void UpdateFontSizes(int selectedSize)
        {
            switch (selectedSize)
            {
                case 0: // Pequeña
                    SizeTitulo = 24;
                    SizeTexto = 11;
                    break;
                case 1:  // Mediana
                    SizeTitulo = 28;
                    SizeTexto = 15;
                    break;
                case 2: // Grande
                    SizeTitulo = 36;
                    SizeTexto = 19;
                    break;
            }
        }
    }
}
