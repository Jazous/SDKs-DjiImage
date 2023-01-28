#### SDKs.DjiImage

https://github.com/Jazous/SDKs-DjiImage

##### 1、Nuget 安装

.NET 6.0+

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

##### 2、下载 Dji TSDK 添加到项目中

https://www.dji.com/cn/downloads/softwares/dji-thermal-sdk

dji_thermal_sdk_v1.4_20220929.zip

Linux ：*libdirp.so、libv_cirp.so、libv_dirp.so、libv_girp.so、libv_iirp.so、libv_list.ini*

Windows：*libdirp.dll、libv_cirp.dll、libv_dirp.dll、ibv_girp.dll、libv_iirp.dll、libv_list.ini*

下载后解压，并将对应 `tsdk-core\lib` 目录下的 `libv_list.ini`、`*.dll` 和 `*.so` 文件拷贝到执行程序根目录下

##### 3、使用例子

```c#
byte[] data = System.IO.File.ReadAllBytes("1.JPG");
using (var img = RJPEG.FromBytes(data))
{
    var at = img.GetRectTemp(0, 0, 100, 100);
    Console.WriteLine($"MinTemp={at.MinTemp} ,MaxTemp={at.MaxTemp}");
}
Console.ReadKey();
```

注：运行报错 `System.BadImageFormatException` 时，需要项目属性设置相应的目标平台， `release_x64` 目标平台选择 `x64`，`release_x86` 目标平台选择 `x86`（`.NET Framework` 中 `AnyCPU` 默认是 `x86`）