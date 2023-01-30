using SDKs.DjiImage.Thermals;
using SkiaSharp;
using System.Diagnostics;
using System.Drawing;

byte[] data = System.IO.File.ReadAllBytes("2.JPG");
using (var rjpg = RJPEG.FromBytes(data))
{
    Console.WriteLine($"rjpeg：MinTemp={rjpg.MinTemp} ,MaxTemp={rjpg.MaxTemp} ,AvgTemp={rjpg.AvgTemp}");


    var rect = rjpg.GetRect(20, 20, rjpg.Width - 20, rjpg.Height - 20);
    Console.WriteLine($"rect：MinTemp={rect.MinTemp} ,MaxTemp={rect.MaxTemp} ,AvgTemp={rect.AvgTemp}");


    var ltc = rjpg.Filter(rjpg.GetEntries(c => c >= 80).ToArray(), 30, 20);
    var lt = new LTCollection(ltc.Count);
    lt.AddRange(ltc);
    var ltList = lt.Split(50);
    Console.WriteLine($"points={ltc.Count}，split count={ltList.Count}个");

    SKPaint p1 = new SKPaint();
    p1.Color = SKColors.Cyan;
    p1.StrokeWidth = 2;
    p1.Typeface = SKTypeface.Default;
    p1.Style = SKPaintStyle.Stroke;

    SKPaint p2 = new SKPaint();
    p2.Color = SKColors.Black;
    p2.StrokeWidth = 1;
    p2.Typeface = SKTypeface.FromFamilyName("Calibri");
    p2.Style = SKPaintStyle.Stroke;
    p2.TextSize = 10;

    SKPaint p3 = new SKPaint();
    p3.Color = SKColors.Red;
    p3.StrokeWidth = 2;
    p3.Typeface = SKTypeface.Default;
    p3.Style = SKPaintStyle.Stroke;

    SKImageInfo imageInfo = new SKImageInfo(rjpg.Width, rjpg.Height);
    using (var bitmap = SKBitmap.Decode(data, imageInfo))
    using (var canvas = new SKCanvas(bitmap))
    {
        for (int i = 0; i < ltList.Count; i++)
        {
            var item = ltList[i];
            int w = item.Right - item.Left;
            int h = item.Bottom - item.Top;

            canvas.DrawRect(item.Left, item.Top, w, h, p1);
            canvas.DrawText(item.MaxTemp.ToString(), item.Left + 2, item.Top + 11, p2);

            //标记最高温度点
            var ps = item.MaxTempLocs;
            for (int j = 0; j < ps.Length; j++)
                canvas.DrawPoint(ps[j].Left, ps[j].Top, p3);
        }

        canvas.Flush();
        using (var pic = SKImage.FromBitmap(bitmap))
        {
            var skData = pic.Encode(SKEncodedImageFormat.Jpeg, 100);
            using (var fs = File.Open("2_mark.JPG", FileMode.OpenOrCreate, FileAccess.Write))
            {
                skData.SaveTo(fs);
                fs.Flush();
            }
        }

    }
}
Console.ReadKey();