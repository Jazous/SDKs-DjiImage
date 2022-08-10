using SDKs.DjiImage.Thermals;

byte[] data = System.IO.File.ReadAllBytes("1.JPG");
using (var img = RJPEG.FromBytes(data))
{
    Console.WriteLine($"MinTemp={img.MinTemp} ,MaxTemp={img.MaxTemp} ,AvgTemp={img.AvgTemp}");

    var coll = img.GetCircle(86, 85, 85);
    Console.WriteLine($"MinTemp={coll.MinTemp} ,MaxTemp={coll.MaxTemp} ,AvgTemp={coll.AvgTemp}");
}
Console.ReadKey();