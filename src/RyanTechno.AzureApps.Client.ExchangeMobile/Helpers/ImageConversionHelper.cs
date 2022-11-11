using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RyanTechno.AzureApps.Client.ExchangeMobile.Helpers
{
    internal class ImageConversionHelper
    {
        public static string GetCurrencyImageFileName(string currencyAbbr) => $"{currencyAbbr.ToLower()}.png";

        public static string GetCurrencyImagePath(string currencyAbbr, string basePath) => $"{basePath}\\{currencyAbbr.ToLower()}.png";
    }
}
