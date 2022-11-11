using RyanTechno.AzureApps.Client.ExchangeMobile.Helpers;
using System.Globalization;

namespace RyanTechno.AzureApps.Client.ExchangeMobile.Converters
{
    public class CurrencyImageConverter : IValueConverter
    {
        /// <summary>
        /// Get image path from given currency abbreviation.
        /// </summary>
        /// <param name="value">Currency abbreviation.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Image base path.</param>
        /// <param name="culture">Not used</param>
        /// <returns>Image path.</returns>
        /// <exception cref="ArgumentException"></exception>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not null && value is string abbr)
            {
                return parameter ?? parameter + ImageConversionHelper.GetCurrencyImageFileName(abbr);
            }

            throw new ArgumentException("The value is not a string.", nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
