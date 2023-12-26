

using RyanTechno.AzureApps.Client.Console;


string? accessToken = await StockUtil.RetrieveAccessToken();

int startCode = -1;
string folderPath = Path.Combine(Environment.CurrentDirectory, "csv");
if (!Directory.Exists(folderPath))
    Directory.CreateDirectory(folderPath);
else
{
    // Get last market code from existing files.
    string[] existingFiles = Directory.GetFiles(folderPath);
    startCode = existingFiles.Max(f => Convert.ToInt32(f.Substring(f.Length - 8, 4)));
}

var shStock = new { StockMarket = "SHH", StockCodePrefix = "60" };
var szStock = new { StockMarket = "SHZ", StockCodePrefix = "00" };

string stockMarket, stockCode, filePath;
for (int i = startCode + 1; i <= startCode + 100; i++)
{
    stockMarket = shStock.StockMarket;
    stockCode = shStock.StockCodePrefix + i.ToString().PadLeft(4, '0');

    Console.WriteLine("Pause request process for 15 seconds...");
    await Task.Delay(15 * 1000);
    Console.WriteLine($"Start to request daily csv for {stockMarket} - {stockCode}");

    filePath = Path.Combine(folderPath, $"{stockMarket}-{stockCode}.csv");
    await StockUtil.RequestDailyCsv(stockMarket, stockCode, filePath, accessToken);
}



