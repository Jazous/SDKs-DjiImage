using SDKs.DjiImage;
using SDKs.DjiImage.Thermals;

byte[] data = System.IO.File.ReadAllBytes("img/H20T.JPG");

using (var rjpg = RJPEG.TryParse(data))
{
    if (rjpg != null)
    {
        //获取整张图片的最高温、最低温和平均温度
        Console.WriteLine($"rjpeg：MinTemp={rjpg.MinTemp} ,MaxTemp={rjpg.MaxTemp} ,AvgTemp={rjpg.AvgTemp}");

        //获取指定矩形范围内的最高温、最低温和平均温度
        var rect = rjpg.GetRect(new SDKs.DjiImage.Location(260, 240), 200, 110);
        Console.WriteLine($"rect：MinTemp={rect.MinTemp} ,MaxTemp={rect.MaxTemp} ,AvgTemp={rect.AvgTemp}");

        //设置调色板风格
        rjpg.SetPseudoColor(PseudoColor.DIRP_PSEUDO_COLOR_HOTIRON);

        //设置亮度
        rjpg.SetBrightness(60);

        //设置小于60摄氏度以下的像素点为黑色
        using (var fs = System.IO.File.OpenWrite("h20t_modify.JPG"))
        {
            rjpg.SaveTo(fs, t => t <= 60 ? Rgb.Black : null);
            fs.Flush();
        }
    }
}
Console.ReadKey();