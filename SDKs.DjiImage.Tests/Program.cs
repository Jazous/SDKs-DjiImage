using SDKs.DjiImage.Thermals;
using SkiaSharp;
using System.Diagnostics;
using System.Drawing;

byte[] data = System.IO.File.ReadAllBytes("D:\\pc\\Downloads\\dji_thermal_sdk_v1.5_20240507\\dataset\\M3T\\DJI_0005_R.JPG");

var rjpg = RJPEG.FromBytes(data);
if (rjpg != null)
{
    Console.WriteLine($"rjpeg：MinTemp={rjpg.MinTemp} ,MaxTemp={rjpg.MaxTemp} ,AvgTemp={rjpg.AvgTemp}");


    var rect = rjpg.GetRect(20, 20, rjpg.Width - 20, rjpg.Height - 20);
    Console.WriteLine($"rect：MinTemp={rect.MinTemp} ,MaxTemp={rect.MaxTemp} ,AvgTemp={rect.AvgTemp}");

    rjpg.SetPseudoColor(PseudoColor.DIRP_PSEUDO_COLOR_HOTIRON);
    rjpg.SetIsotherm(26.0f, 28.5f);
    rjpg.SetBrightness(10);
    System.IO.File.WriteAllBytes("D:\\c.raw", rjpg.GetProcessedRaw());
}
Console.ReadKey();