using SDKs.DjiImage.Thermals;
using SkiaSharp;
using System.Diagnostics;
using System.Drawing;

byte[] data = System.IO.File.ReadAllBytes("1.JPG");
using (var rjpg = RJPEG.FromBytes(data))
{
    Console.WriteLine($"MinTemp={rjpg.MinTemp} ,MaxTemp={rjpg.MaxTemp} ,AvgTemp={rjpg.AvgTemp}");


    var rect = rjpg.GetRect(20, 20, rjpg.Width - 20, rjpg.Height - 20);
    Console.WriteLine($"MinTemp={rect.MinTemp} ,MaxTemp={rect.MaxTemp} ,AvgTemp={rect.AvgTemp}");


    var ltc = rjpg.Filter(rjpg.GetEntries(c => c >= 62).ToArray(), 30, 20);
    var ltList = ltc.Split(100);
    Console.WriteLine($"MinTemp={ltc.MinTemp} ,MaxTemp={ltc.MaxTemp} ,AvgTemp={ltc.AvgTemp}");

    SKPaint p1 = new SKPaint();
    p1.Color = SKColors.Cyan;
    p1.StrokeWidth = 1;
    p1.Typeface = SKTypeface.Default;
    p1.Style = SKPaintStyle.Stroke;

    SKPaint p2 = new SKPaint();
    p2.Color = SKColors.Black;
    p2.StrokeWidth = 1;
    p2.Typeface = SKTypeface.FromFamilyName("Times New Roman");
    p2.Style = SKPaintStyle.Stroke;
    p2.TextSize = 11;

    SKImageInfo imageInfo = new SKImageInfo(rjpg.Width, rjpg.Height);
    using (var bitmap = SKBitmap.Decode(data, imageInfo))
    using (var canvas = new SKCanvas(bitmap))
    {
        for (int i = 0; i < ltList.Count; i++)
        {
            var item = ltList[i];
            int w = item.Right - item.Left;
            int h = item.Bottom - item.Top;
            if (w < 25) w = 25;
            if (h < 25) h = 25;
            canvas.DrawRect(item.Left, item.Top, w, h, p1);
            canvas.DrawText(item.MaxTemp.ToString(), item.Left + 2, item.Top + 11, p2);
            canvas.Flush();
        }
        using (var pic = SKImage.FromBitmap(bitmap))
        {
            var skData = pic.Encode(SKEncodedImageFormat.Jpeg, 90);
            using (var fs = File.Open("1_mark.JPG", FileMode.OpenOrCreate, FileAccess.Write))
            {
                skData.SaveTo(fs);
                fs.Flush();
            }
        }
    }
}
Console.ReadKey();