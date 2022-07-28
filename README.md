#### SDKs.DjiImage

https://github.com/Jazous/SDKs-DjiImage

##### 1、Nuget 安装：

```shell
PM> Install-Package SDKs.DjiImage -Version 1.0.0
```

##### 2、下载 Dji TSDK 添加到项目中：

https://www.dji.com/cn/downloads/softwares/dji-thermal-sdk

dji_thermal_sdk_v1.3_20220517.zip

Windows 环境

libdirp.dll、libv_dirp.dll、ibv_girp.dll、libv_iirp.dll、libv_list.ini

Linux 环境

libdirp.so、libv_dirp.so、libv_girp.so、libv_iirp.so、libv_list.ini

下载后将对应的 .dll 或 .so 文件拷贝到执行程序根目录下

##### 3、使用方法：

```c#
byte[] data = System.IO.File.ReadAllBytes("1.JPG");
using (var img = RJPEG.FromBytes(data))
{
    AreaTemperature at = img.GetTempRect(0, 0, 100, 100);
    Console.WriteLine($"MinTemp={at.MinTemp} ,MaxTemp={at.MaxTemp}");
}
Console.ReadKey();
```

#### License 说明

https://github.com/Jazous/SDKs-DjiImage/blob/main/LICENSE.txt