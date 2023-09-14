using RyanTechno.AzureApps.Common.Models;
using RyanTechno.AzureApps.Common.Models.Exchange;
using System.Collections.Immutable;

namespace RyanTechno.AzureApps.Services.Extensions
{
    public static class AzureExchangeServiceExtensions
    {
        public static void TranslateCurrencySubject(this IDictionary<string, DailyExchangeRateBoardcastBenchmark> benchmarks, IImmutableDictionary<string, string> currencySubjects, string languagePrefix)
        {
            Dictionary<string, string?> subjectMapping = benchmarks.Select(b => new
            {
                OldKey = b.Key,
                NewKey = currencySubjects.ContainsKey(b.Key.Replace(languagePrefix, string.Empty)) ? currencySubjects[b.Key.Replace(languagePrefix, string.Empty)] : null,
            }).ToDictionary(m => m.OldKey, m => m.NewKey);

            string[] oldKeys = benchmarks.Keys.ToArray();

            foreach (string oldKey in oldKeys)
            {
                DailyExchangeRateBoardcastBenchmark benchmark = benchmarks[oldKey];

                benchmarks.Remove(oldKey);
                benchmarks.Add(subjectMapping[oldKey] ?? oldKey, benchmark);
            }
        }
    }
}
