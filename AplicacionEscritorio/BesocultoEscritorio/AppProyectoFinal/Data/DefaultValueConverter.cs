using System.Globalization;

namespace AppProyectoFinal.Data
{
    /// <summary>
    /// Convertidor de valores para proporcionar un valor predeterminado cuando el valor de entrada es nulo o vacío.
    /// </summary>
    public class DefaultValueConverter : IValueConverter
    {

        public LocalizationResourceManager LocalizationResourceManager
            => LocalizationResourceManager.Instance;
        /// <summary>
        /// Método para convertir un valor, devolviendo un parámetro específico si el valor es nulo o vacío.
        /// <list type="number">
        /// <param name="value">Valor de entrada a convertir, puede ser nulo o vacío.</param>
        /// <param name="targetType">Tipo de destino al que se desea convertir el valor.</param>
        /// <param name="parameter">Valor predeterminado a devolver si el valor de entrada es nulo o vacío.</param>
        /// <param name="culture">Información cultural que se debe utilizar en la conversión.</param>
        /// </list>
        /// </summary>
        /// <returns>Devuelve el valor de entrada si no es nulo o vacío; de lo contrario, devuelve el parámetro predeterminado.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string baseValue = value?.ToString() ?? string.Empty;
            if (parameter is string param && param == "AddPag")
            {
                // Si el valor es vacío o nulo, usa un valor por defecto
                if (string.IsNullOrWhiteSpace(baseValue))
                {
                    baseValue = "0";
                }

                // Devuelve el valor con "pag"
                return $"{baseValue} pag";
            }

            if (value is string stringValue)
            {
                
                return string.IsNullOrEmpty(stringValue) ? LocalizationResourceManager[parameter.ToString()] : stringValue;
            }
            return value ?? LocalizationResourceManager[parameter.ToString()];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
