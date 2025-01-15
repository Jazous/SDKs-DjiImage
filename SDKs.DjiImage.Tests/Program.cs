using SDKs.DjiImage.Thermals;

byte[] data = System.IO.File.ReadAllBytes("3.JPG");

using (var rjpg = RJPEG.TryParse(data))
{
    if (rjpg != null)
    {
        //获取整张图片的最高温、最低温和平均温度
        Console.WriteLine($"rjpeg：MinTemp={rjpg.MinTemp} ,MaxTemp={rjpg.MaxTemp} ,AvgTemp={rjpg.AvgTemp}");

        //获取指定矩形范围内的最高温、最低温和平均温度
        var rect = rjpg.GetRect(100, 100, 200, 200);
        Console.WriteLine($"rect：MinTemp={rect.MinTemp} ,MaxTemp={rect.MaxTemp} ,AvgTemp={rect.AvgTemp}");

        //设置调色板风格
        rjpg.SetPseudoColor(PseudoColor.DIRP_PSEUDO_COLOR_HOTIRON);
        //设置亮度
        rjpg.SetBrightness(60);
        //保存设置后的伪彩色照片
        using (var fs = System.IO.File.OpenWrite("3_1.JPG"))
        {
            rjpg.SaveTo(fs);
            fs.Flush();
        }
    }
}
Console.ReadKey();