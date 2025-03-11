#### SDKs.DjiImage

https://github.com/Jazous/SDKs-DjiImage

##### 1、Nuget 安装

.NET 6.0及以上

```shell
PM> Install-Package SDKs.DjiImage
```

.NET Framework 4.8

```shell
PM> Install-Package SDKs.DjiImage.Net48
```

.NET Framework 4.5.2

```shell
PM> Install-Package SDKs.DjiImage.Net45
```

##### 2、下载 DJI TSDK 添加到项目中

https://www.dji.com/cn/downloads/softwares/dji-thermal-sdk

dji_thermal_sdk_v1.7_20241205.zip

下载后解压，并将对应 `tsdk-core\lib` 目录下的 `libv_list.ini`、`*.dll` 和 `*.so` 文件拷贝到执行程序根目录下

##### 3、使用例子

```c#
byte[] data = System.IO.File.ReadAllBytes("img/H30T.JPG");

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
        rjpg.SetPseudoColor(PseudoColor.DIRP_PSEUDO_COLOR_IRONRED);
        
        //设置亮度
        rjpg.SetBrightness(60);

        //保存设置温宽后的伪彩色照片
        using (var fs = System.IO.File.OpenWrite("H30T_adjust.JPG"))
        {
            rjpg.SaveTo(fs, rjpg.MaxTemp - 2, rjpg.MaxTemp);
            fs.Flush();
        }
    }
}
```

注：运行报错 `System.BadImageFormatException` 时，需要项目属性设置相应的目标平台， `release_x64` 目标平台选择 `x64`，`release_x86` 目标平台选择 `x86`（`.NET Framework` 中 `AnyCPU` 默认是 `x86`）