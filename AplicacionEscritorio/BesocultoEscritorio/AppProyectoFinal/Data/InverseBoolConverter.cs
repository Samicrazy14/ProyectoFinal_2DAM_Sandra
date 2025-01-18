using System.Globalization;

namespace AppProyectoFinal.Data
{
    /// <summary>
    /// Convertidor de valores booleanos para invertir el valor original.
    /// </summary>
    public class InverseBoolConverter : IValueConverter
    {
        /// <summary>
        /// Método para convertir un valor booleano a su inverso.
        /// <list type="number">
        /// <param name="value">Valor booleano de entrada que se va a invertir.</param>
        /// <param name="targetType">Tipo de destino al que se desea convertir el valor.</param>
        /// <param name="parameter">Parámetro opcional para la conversión.</param>
        /// <param name="culture">Información cultural que se debe utilizar en la conversión.</param>
        /// </list>
        /// </summary>
        /// <returns>Devuelve el valor booleano invertido, o el valor original si no es booleano.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue ? !boolValue : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue ? !boolValue : value;
        }
    }
}