using System.Text.Json;

namespace RyanTechno.AzureApps.Infrastructure.Helpers
{
    public class IOHelper
    {
        public static T DeserializeJsonFile<T>(string jsonFile)
        {
            string jsonString = File.ReadAllText(jsonFile);
            T jsonObj = JsonSerializer.Deserialize<T>(jsonString)!;

            return jsonObj;
        }
    }
}
