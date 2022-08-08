using SDKs.DjiImage.Thermals;

byte[] data = System.IO.File.ReadAllBytes("1.JPG");
using (var img = RJPEG.FromBytes(data))
{
    Console.WriteLine($"MinTemp={img.MinTemp} ,MaxTemp={img.MaxTemp} ,AvgTemp={img.AvgTemp}");

    AreaTemperature at = img.GetTempCircle(86, 85, 85);
    Console.WriteLine($"MinTemp={at.MinTemp} ,MaxTemp={at.MaxTemp} ,AvgTemp={at.AvgTemp}");
}
Console.ReadKey();