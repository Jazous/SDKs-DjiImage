//======================================================================
//
//        Copyright (C) 2022 jazous
//        All rights reserved
//
//        filename :RJPEG
//        description :
//
//        created by jazous at  03/09/2008 18:41:28
//
//====================================================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// 大疆无人机 R-JPEG 热红外照片
    /// </summary>
    /// <remarks>支持：禅思 H20N、禅思 Zenmuse XT S、禅思 Zenmuse H20 系列、经纬 M30 系列、御 2 行业进阶版、DJI Mavic 3 行业系列</remarks>
    public sealed class RJPEG : IJPEG, IAreaTemperature
    {
        System.IntPtr _ph = System.IntPtr.Zero;
        float[,] _mData = null;
        float _mintemp;
        float _maxtemp;
        float _avgtemp;
        int _width;
        int _height;
        Location[] _maxtemploc;
        RdfDroneDji _droneDji;
        MeasureParam _params;

        /// <summary>
        /// 获取指定位置的温度
        /// </summary>
        /// <param name="left">距离图片左边的位置，范围：0 ~ Width - 1。</param>
        /// <param name="top">距离图片上边的位置，范围：0 ~ Height - 1。</param>
        /// <returns>该位置的温度。</returns>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public float this[int left, int top]
        {
            get { return _mData[left, top]; }
        }

        /// <summary>
        /// 图像宽度
        /// </summary>
        public int Width => _width;
        /// <summary>
        /// 图像高度
        /// </summary>
        public int Height => _height;
        /// <summary>
        /// 红外测温参数
        /// </summary>
        public MeasureParam Params => _params;
        /// <summary>
        /// 图片最低温度
        /// </summary>
        public float MinTemp => _mintemp;
        /// <summary>
        /// 图片最高温度
        /// </summary>
        public float MaxTemp => _maxtemp;
        /// <summary>
        /// 图片平均温度
        /// </summary>
        public float AvgTemp => _avgtemp;
        /// <summary>
        /// XMP Meta drone-dji 信息
        /// </summary>
        public RdfDroneDji DroneDji => _droneDji;
        /// <summary>
        /// 图片最高温度位置
        /// </summary>
        public Location[] MaxTempLocs => _maxtemploc;

        private RJPEG()
        {
        }
        /// <summary>
        /// 从指定文件创建大疆热红外 R-JPEG 图片
        /// </summary>
        /// <param name="path">JPEG 文件路径</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.DllNotFoundException"></exception>
        public static RJPEG FromFile(string path)
        {
            return FromStream(System.IO.File.OpenRead(path), false);
        }
        /// <summary>
        /// 从指定文件流创建大疆热红外 R-JPEG 图片
        /// </summary>
        /// <param name="stream">图片字节流</param>
        /// <param name="leaveOpen">使用完后是否关闭流</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.DllNotFoundException"></exception>
        public static RJPEG FromStream(System.IO.Stream stream, bool leaveOpen = false)
        {
            if (stream == null || stream == System.IO.Stream.Null)
                throw new System.ArgumentNullException(nameof(stream));
            if (stream.Length == 0)
                throw new System.ArgumentException("stream's length can not be zero.", nameof(stream));

            int len = (int)stream.Length;
            byte[] buffer = new byte[len];
            stream.Read(buffer, 0, buffer.Length);
            if (!leaveOpen)
                stream.Close();

            var img = new RJPEG();
            int code = img.Load(buffer);
            if (code == 0)
            {
                img._droneDji = Rdf.GetDroneDji(buffer) ?? RdfDroneDji.Empty;
                return img;
            }
            img.Dispose();
            throw new System.ArgumentException(((dirp_ret_code_e)code).ToString(), nameof(stream));
        }
        /// <summary>
        /// 从指定字节数组创建大疆热红外 R-JPEG 图片
        /// </summary>
        /// <param name="bytes">文件字节数组</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.DllNotFoundException"></exception>
        public static RJPEG FromBytes(byte[] bytes)
        {
            if (bytes == null)
                throw new System.ArgumentNullException(nameof(bytes));
            if (bytes.Length == 0)
                throw new System.ArgumentException("bytes's length can not be zero.", nameof(bytes));

            var img = new RJPEG();
            int code = img.Load(bytes);
            if (code == 0)
            {
                img._droneDji = Rdf.GetDroneDji(bytes) ?? RdfDroneDji.Empty;
                return img;
            }
            img.Dispose();
            throw new System.ArgumentException(((dirp_ret_code_e)code).ToString(), nameof(bytes));
        }
        /// <summary>
        /// 尝试从指定文件流创建大疆热红外 R-JPEG 图片
        /// </summary>
        /// <param name="stream">图片字节流</param>
        /// <param name="leaveOpen">使用完后是否关闭流</param>
        /// <returns></returns>
        /// <exception cref="System.DllNotFoundException"></exception>
        /// <remarks>解析失败返回 null</remarks>
        public static RJPEG TryParse(System.IO.Stream stream, bool leaveOpen = false)
        {
            if (stream == null || stream == System.IO.Stream.Null)
                return null;

            int len = (int)stream.Length;
            if (len == 0)
                return null;

            byte[] buffer = new byte[len];
            stream.Read(buffer, 0, buffer.Length);
            if (!leaveOpen)
                stream.Close();

            var img = new RJPEG();
            int code = img.Load(buffer);
            if (code == 0)
            {
                img._droneDji = Rdf.GetDroneDji(buffer) ?? RdfDroneDji.Empty;
                return img;
            }
            img.Dispose();
            return null;
        }
        /// <summary>
        /// 尝试从指定字节数组创建大疆热红外 R-JPEG 图片
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        /// <exception cref="System.DllNotFoundException"></exception>
        /// <remarks>解析失败返回 null</remarks>
        public static RJPEG TryParse(byte[] bytes)
        {
            if (bytes == null)
                return null;
            if (bytes.Length == 0)
                return null;

            var img = new RJPEG();
            int code = img.Load(bytes);
            if (code == 0)
            {
                img._droneDji = Rdf.GetDroneDji(bytes) ?? RdfDroneDji.Empty;
                return img;
            }
            img.Dispose();
            return null;
        }
        /// <summary>
        /// 验证指定字节数组是否是大疆热红外照片
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static bool IsRJPEG(byte[] bytes)
        {
            IntPtr ph = IntPtr.Zero;
            int code = _tsdk.dirp_create_from_rjpeg(bytes, bytes.Length, ref ph);
            if (code == 0)
            {
                _tsdk.dirp_destroy(ph);
                ph = System.IntPtr.Zero;
                return true;
            }
            return false;
        }
        int Load(byte[] bytes)
        {
            int code = _tsdk.dirp_create_from_rjpeg(bytes, bytes.Length, ref _ph);
            if (code == 0)
            {
                var res = new dirp_resolution_t();
                _tsdk.dirp_get_rjpeg_resolution(_ph, ref res);
                _width = res.width;
                _height = res.height;

                var mp = new MeasureParam();
                _tsdk.dirp_get_measurement_params(_ph, ref mp);
                _params = mp;

                int rawsize = _width * _height * 2;
                byte[] buffer = new byte[rawsize];
                _tsdk.dirp_measure(_ph, buffer, rawsize);
                _tsdk.dirp_destroy(_ph);
                _ph = System.IntPtr.Zero;
                _mData = Cast(buffer, _width, _height);
            }
            return code;
        }
        float[,] Cast(byte[] rawData, int width, int height)
        {
            var result = new float[width, height];
            int index = 0;
            int i, j;
            byte[] arr = new byte[2] { rawData[0], rawData[1] };
            float temp = float.Parse((System.BitConverter.ToInt16(arr, 0) * 0.100000f).ToString("f1"));
            float mintemp = temp;
            float maxtemp = temp;
            float sumTemp = 0;
            var maxtemploc = new List<Location>();
            for (j = 0; j < height; j++)
            {
                for (i = 0; i < width; i++)
                {
                    arr[0] = rawData[index];
                    arr[1] = rawData[index + 1];
                    index += 2;
                    temp = float.Parse((System.BitConverter.ToInt16(arr, 0) * 0.100000f).ToString("f1"));
                    result[i, j] = temp;
                    sumTemp += temp;
                   
                    if (maxtemp < temp)
                    {
                        maxtemp = temp;
                        maxtemploc.Clear();
                        maxtemploc.Add(new Location(i, j));
                    }
                    else if (maxtemp == temp)
                    {
                        maxtemploc.Add(new Location(i, j));
                    }
                }
            }
            this._mintemp = mintemp;
            this._maxtemp = maxtemp;
            this._maxtemploc = maxtemploc.Distinct().ToArray();
            this._avgtemp = float.Parse((sumTemp / result.Length).ToString("f1"));
            return result;
        }

        /// <summary>
        /// 获取满足指定温度条件的位置温度集合
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public LTCollection GetEntries(System.Predicate<float> predicate)
        {
            int i, j;
            int width = _width;
            int height = _height;
            float temp;
            var result = new LTCollection();
            for (j = 0; j < height; j++)
            {
                for (i = 0; i < width; i++)
                {
                    temp = _mData[i, j];
                    if (predicate.Invoke(temp))
                        result.Add(new LTEntry(i, j, temp));
                }
            }
            return result;
        }
        /// <summary>
        /// 获取满足指定条件的位置温度集合
        /// </summary>
        /// <param name="predicate">参数依次对应 Left、Top、Temp</param>
        /// <returns></returns>
        public LTCollection GetEntries(System.Func<int, int, float, bool> predicate)
        {
            int i, j;
            int width = _width;
            int height = _height;
            float temp;
            var result = new LTCollection();
            for (j = 0; j < height; j++)
            {
                for (i = 0; i < width; i++)
                {
                    temp = _mData[i, j];
                    if (predicate.Invoke(i, j, temp))
                        result.Add(new LTEntry(i, j, temp));
                }
            }
            return result;
        }
        /// <summary>
        /// 获取缓冲区图片温度矩阵
        /// </summary>
        /// <returns></returns>
        public float[,] GetTemp()
        {
            return _mData;
        }
        /// <summary>
        /// 获取图片指定位置的温度，超出图像范围返回 float.NaN
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public float GetTemp(Location p)
        {
            return GetTemp(p.Left, p.Top);
        }
        /// <summary>
        /// 获取图片指定位置的温度，超出图像范围返回 float.NaN
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public float GetTemp(int left, int top)
        {
            if (left < 0 || left > Width - 1)
                return float.NaN;

            if (top < 0 || top > Height - 1)
                return float.NaN;

            return _mData[left, top];
        }
        /// <summary>
        ///获取图像指定线段上的位置温度集合
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public LTCollection GetLine(Location p1, Location p2)
        {
            return GetLine(p1.Left, p1.Top, p2.Left, p2.Top);
        }
        /// <summary>
        /// 获取图像指定线段上的位置温度集合
        /// </summary>
        /// <param name="leftA"></param>
        /// <param name="topA"></param>
        /// <param name="leftB"></param>
        /// <param name="topB"></param>
        /// <returns></returns>
        public LTCollection GetLine(int leftA, int topA, int leftB, int topB)
        {
            var result = new LTCollection();
            int w = _width - 1;
            int h = _height - 1;

            if (leftA == leftB)
            {
                if (leftA < 0 && leftA > w)
                    return result;

                int miny = System.Math.Min(topA, topB);
                if (miny < 0) miny = 0;
                int maxy = System.Math.Max(topA, topB);
                if (maxy > h) maxy = h;

                for (int j = miny; j <= maxy; j++)
                    result.Add(leftA, j, _mData[leftA, j]);
                return result;
            }
            else
            {
                int minx = System.Math.Min(leftA, leftB);
                int maxx = System.Math.Max(leftA, leftB);
                if (minx > w || maxx < 0)
                    return result;

                if (minx < 0) minx = 0;
                if (maxx > w) maxx = w;

                int j1, j2;
                decimal k = decimal.Divide(topB - topA, leftB - leftA);
                for (int i = minx; i <= maxx; i++)
                {
                    j1 = System.Convert.ToInt32(System.Math.Floor(k * i));
                    if (j1 >= 0 && j1 <= h)
                        result.Add(i, j1, _mData[i, j1]);

                    j2 = System.Convert.ToInt32(System.Math.Ceiling(k * i));
                    if (j1 != j2 && j2 >= 0 && j2 <= h)
                        result.Add(i, j2, _mData[i, j2]);
                }
                return result;
            }
        }
        /// <summary>
        /// 获取图像指定线段上的温度
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public AreaTemperature GetLineTemp(Location p1, Location p2)
        {
            return GetLineTemp(p1.Left, p1.Top, p2.Left, p2.Top);
        }
        /// <summary>
        /// 获取图像指定线段上的温度
        /// </summary>
        /// <param name="leftA"></param>
        /// <param name="topA"></param>
        /// <param name="leftB"></param>
        /// <param name="topB"></param>
        /// <returns></returns>
        public AreaTemperature GetLineTemp(int leftA, int topA, int leftB, int topB)
        {
            int w = _width - 1;
            int h = _height - 1;

            float temp;
            float mintemp;
            float maxtemp;
            float sumtemp = 0;
            if (leftA == leftB)
            {
                if (leftA < 0 && leftA > w)
                    return AreaTemperature.Empty;

                int miny = System.Math.Min(topA, topB);
                if (miny < 0) miny = 0;
                int maxy = System.Math.Max(topA, topB);
                if (maxy > h) maxy = h;

                mintemp = _maxtemp;
                maxtemp = _mintemp;
                sumtemp = 0;

                for (int j = miny; j <= maxy; j++)
                {
                    temp = _mData[leftA, j];
                    if (mintemp > temp) mintemp = temp;
                    if (maxtemp < temp) maxtemp = temp;
                    sumtemp += temp;
                }
                return new AreaTemperature(mintemp, maxtemp, float.Parse((sumtemp / (maxy - miny + 1)).ToString("f1")));
            }
            else
            {
                int minx = System.Math.Min(leftA, leftB);
                int maxx = System.Math.Max(leftA, leftB);
                if (minx > w || maxx < 0)
                    return AreaTemperature.Empty;

                if (minx < 0) minx = 0;
                if (maxx > w) maxx = w;
                mintemp = _maxtemp;
                maxtemp = _mintemp;

                int j1, j2;
                float sumcount = 0;
                decimal k = decimal.Divide(topB - topA, leftB - leftA);
                for (int i = minx; i <= maxx; i++)
                {
                    j1 = System.Convert.ToInt32(System.Math.Floor(k * i));
                    if (j1 >= 0 && j1 <= h)
                    {
                        temp = _mData[i, j1];
                        if (mintemp > temp) mintemp = temp;
                        if (maxtemp < temp) maxtemp = temp;
                        sumtemp += temp;
                        sumcount++;
                    }
                    j2 = System.Convert.ToInt32(System.Math.Ceiling(k * i));
                    if (j1 != j2 && j2 >= 0 && j2 <= h)
                    {
                        temp = _mData[i, j2];
                        if (mintemp > temp) mintemp = temp;
                        if (maxtemp < temp) maxtemp = temp;
                        sumtemp += temp;
                        sumcount++;
                    }
                }
                return new AreaTemperature(mintemp, maxtemp, float.Parse((sumtemp / sumcount).ToString("f1")));
            }
        }
        /// <summary>
        /// 获取图像指定矩形范围的位置温度集合
        /// </summary>
        /// <param name="p"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public LTCollection GetRect(Location p, int width, int height)
        {
            return GetRect(p.Left, p.Top, p.Left + width, p.Top + height);
        }
        /// <summary>
        /// 获取图像指定矩形范围的位置温度集合
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <returns></returns>
        public LTCollection GetRect(int left, int top, int right, int bottom)
        {
            var result = new LTCollection();
            int w = _width - 1;
            int h = _height - 1;
            if (left > right || left > w || right < 0 || top > bottom || bottom < 0 || top > h)
                return result;

            int minx = left;
            if (minx < 0) minx = 0;
            int maxx = right;
            if (maxx > w) maxx = w;

            int miny = top;
            if (miny < 0) miny = 0;
            int maxy = bottom;
            if (maxy > h) maxy = h;


            for (int i = minx; i <= maxx; i++)
                for (int j = miny; j <= maxy; j++)
                    result.Add(i, j, _mData[i, j]);

            return result;
        }
        /// <summary>
        /// 获取图像指定矩形范围的温度
        /// </summary>
        /// <param name="p"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public AreaTemperature GetRectTemp(Location p, int width, int height)
        {
            return GetRectTemp(p.Left, p.Top, p.Left + width, p.Top + height);
        }
        /// <summary>
        /// 获取图像指定矩形范围的温度
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <returns></returns>
        public AreaTemperature GetRectTemp(int left, int top, int right, int bottom)
        {
            int w = _width - 1;
            int h = _height - 1;
            if (left > right || left > w || right < 0 || top > bottom || bottom < 0 || top > h)
                return AreaTemperature.Empty;

            int minx = left;
            if (minx < 0) minx = 0;
            int maxx = right;
            if (maxx > w) maxx = w;

            int miny = top;
            if (miny < 0) miny = 0;
            int maxy = bottom;
            if (maxy > h) maxy = h;

            float temp;
            float mintemp = _maxtemp;
            float maxtemp = _mintemp;
            float sumtemp = 0;
            for (int i = minx; i <= maxx; i++)
            {
                for (int j = miny; j <= maxy; j++)
                {
                    temp = _mData[i, j];
                    sumtemp += temp;
                    if (mintemp > temp) mintemp = temp;
                    if (maxtemp < temp) maxtemp = temp;
                }
            }
            float avgtemp = float.Parse((sumtemp / ((maxx - minx + 1) * (maxy - miny + 1))).ToString("f1"));
            return new AreaTemperature(mintemp, maxtemp, avgtemp);
        }
        /// <summary>
        /// 获取图像指定圆内的位置温度集合
        /// </summary>
        /// <param name="p">圆心位置</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        public LTCollection GetCircle(Location p, int radius)
        {
            return GetEllipse(p.Left, p.Top, radius, radius);
        }
        /// <summary>
        /// 获取图像指定圆内的位置温度集合
        /// </summary>
        /// <param name="left">圆心水平方向位置</param>
        /// <param name="top">圆心垂直方向位置</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        public LTCollection GetCircle(int left, int top, int radius)
        {
            return GetEllipse(left, top, radius, radius);
        }
        /// <summary>
        /// 获取图像指定圆内的温度
        /// </summary>
        /// <param name="p">圆心位置</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        public AreaTemperature GetCircleTemp(Location p, int radius)
        {
            return GetEllipseTemp(p.Left, p.Top, radius, radius);
        }
        /// <summary>
        /// 获取图像指定圆内的温度
        /// </summary>
        /// <param name="left">圆心水平方向位置</param>
        /// <param name="top">圆心垂直方向位置</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        public AreaTemperature GetCircleTemp(int left, int top, int radius)
        {
            return GetEllipseTemp(left, top, radius, radius);
        }
        /// <summary>
        /// 获取图像指定椭圆内的位置温度集合
        /// </summary>
        /// <param name="p">椭圆中心点位置</param>
        /// <param name="a">水平方向半轴长度，椭圆宽度 = 2*a</param>
        /// <param name="b">垂直方向半轴长度，椭圆高度 = 2*b</param>
        /// <returns></returns>
        public LTCollection GetEllipse(Location p, int a, int b)
        {
            return GetEllipse(p.Left, p.Top, a, b);
        }
        /// <summary>
        /// 获取图像指定椭圆内的位置温度集合
        /// </summary>
        /// <param name="left">椭圆中心点水平方向位置</param>
        /// <param name="top">椭圆中心点垂直方向位置</param>
        /// <param name="a">水平方向半轴长度，椭圆宽度 = 2*a</param>
        /// <param name="b">垂直方向半轴长度，椭圆高度 = 2*b</param>
        /// <returns></returns>
        public LTCollection GetEllipse(int left, int top, int a, int b)
        {
            var result = new LTCollection();
            int h = _height - 1;
            int w = _width - 1;

            if (left < -a || left > w + a || top < -b || top > h + b || a <= 0 || b <= 0)
                return result;

            int minx = left - a;
            if (minx < 0) minx = 0;
            int maxx = left + a;
            if (maxx > w) maxx = w;

            int miny = top - b;
            if (miny < 0) miny = 0;
            int maxy = top + b;
            if (maxy > h) maxy =h;

            float temp;
            float aa = a * a;
            float bb = b * b;
            float aabb = aa * bb;
            int dx, dy;
            for (int i = minx; i <= maxx; i++)
            {
                for (int j = miny; j <= maxy; j++)
                {
                    dx = left - i;
                    dy = top - j;
                    if (dx * dx * bb + dy * dy * aa < aabb)
                    {
                        temp = _mData[i, j];
                        result.Add(i, j, temp);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 获取图像指定椭圆内的温度
        /// </summary>
        /// <param name="p">椭圆中心点位置</param>
        /// <param name="a">水平方向半轴长度，椭圆宽度 = 2*a</param>
        /// <param name="b">垂直方向半轴长度，椭圆高度 = 2*b</param>
        /// <returns></returns>
        public AreaTemperature GetEllipseTemp(Location p, int a, int b)
        {
            return GetEllipseTemp(p.Left, p.Top, a, b);
        }
        /// <summary>
        /// 获取图像指定椭圆内的温度
        /// </summary>
        /// <param name="left">椭圆中心点水平方向位置</param>
        /// <param name="top">椭圆中心点垂直方向位置</param>
        /// <param name="a">水平方向半轴长度，椭圆宽度 = 2*a</param>
        /// <param name="b">垂直方向半轴长度，椭圆高度 = 2*b</param>
        /// <returns></returns>
        public AreaTemperature GetEllipseTemp(int left, int top, int a, int b)
        {
            int h = _height - 1;
            int w = _width - 1;

            if (left < -a || left > w + a || top < -b || top > h + b || a <= 0 || b <= 0)
                return AreaTemperature.Empty;

            int minx = left - a;
            if (minx < 0) minx = 0;
            int maxx = left + a;
            if (maxx > w) maxx = w;

            int miny = top - b;
            if (miny < 0) miny = 0;
            int maxy = top + b;
            if (maxy > h) maxy = h;

            float aa = a * a;
            float bb = b * b;
            float aabb = aa * bb;
            int dx, dy;

            float temp;
            float mintemp = _maxtemp;
            float maxtemp = _mintemp;
            float sumtemp = 0;
            int sumcount = 0;
            for (int i = minx; i <= maxx; i++)
            {
                for (int j = miny; j <= maxy; j++)
                {
                    dx = left - i;
                    dy = top - j;
                    if (dx * dx * bb + dy * dy * aa < aabb)
                    {
                        temp = _mData[i, j];
                        if (mintemp > temp) mintemp = temp;
                        if (maxtemp < temp) maxtemp = temp;
                        sumtemp += temp;
                        sumcount++;
                    }
                }
            }
            if (sumcount == 0)
                return AreaTemperature.Empty;

            return new AreaTemperature(mintemp, maxtemp, float.Parse((sumtemp / sumcount).ToString("f1")));
        }
        /// <summary>
        /// 过滤指定集合中满足指定温差阈值设定的温度点集合。
        /// </summary>
        /// <param name="entries">需要对其过滤的集合。</param>
        /// <param name="radius">温差限定半径。</param>
        /// <param name="dtemp">温差阈值。</param>
        /// <returns></returns>
        /// <remarks>
        /// 以集合中每个点为圆心，过滤出满足指定像素半径范围内温差大于指定温度的点。
        /// </remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public List<LTEntry> Filter(IEnumerable<LTEntry> entries, int radius, float dtemp)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));
            if (radius <= 0)
                throw new ArgumentException(nameof(radius), "The radius must be a positive integer.");
            if (dtemp <= 0)
                throw new ArgumentException(nameof(dtemp), "The dtemp must be a positive float.");

            if (!entries.Any())
                return Enumerable.Empty<LTEntry>().ToList();

            var result = new List<LTEntry>();
            Parallel.ForEach(entries, entry =>
            {
                var circle = GetEllipseTemp(entry.Left, entry.Top, radius, radius);
                if ((circle.MaxTemp - circle.MinTemp) > dtemp)
                    result.Add(entry);
            });
            return result;
        }

        /// <summary>
        /// 设置调色板风格
        /// </summary>
        /// <param name="color">色阶风格</param>
        /// <returns></returns>
        public bool SetPseudoColor(PseudoColor color)
        {
            return _tsdk.dirp_set_pseudo_color(_ph, color) == 0;
        }
        /// <summary>
        /// 设置等温线
        /// </summary>
        /// <param name="low">最低温度</param>
        /// <param name="high">最高温度</param>
        /// <returns></returns>
        bool SetIsotherm(float low, float high)
        {
            var isotherm = new dirp_isotherm_t() { enable = true, high = high, low = low };
            return _tsdk.dirp_set_isotherm(_ph, isotherm) == 0;
        }
        /// <summary>
        /// 设置亮度
        /// </summary>
        /// <param name="brightness">亮度，0~100，默认为 50</param>
        /// <returns></returns>
        public bool SetBrightness(byte brightness)
        {
            if (brightness > 100) brightness = 100;
            var enhancement_params_t = new dirp_enhancement_params_t() { brightness = brightness };
            return _tsdk.dirp_set_enhancement_params(_ph, ref enhancement_params_t) == 0;
        }
        /// <summary>
        /// 保存 RGB 伪彩色 Jpeg 图片到指定的流。
        /// </summary>
        /// <returns></returns>
        public void SaveTo(Stream stream)
        {
            byte[] bytes = new byte[_width * _height * 3];
            if (0 == _tsdk.dirp_process(_ph, bytes, bytes.Length))
            {
                using (var bitmap = new Bitmap(_width, _height))
                {
                    byte r, g, b;
                    int idx = 0;
                    for (int y = 0; y < _height; y++)
                    {
                        for (int x = 0; x < _width; x++)
                        {
                            r = bytes[idx];
                            g = bytes[idx + 1];
                            b = bytes[idx + 2];
                            idx = idx + 3;
                            bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(r, g, b));
                        }
                    }
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }
        /// <summary>
        ///  Get original RAW binary data from R-JPEG.
        /// </summary>
        /// <returns></returns>
        byte[] GetOriginalRaw()
        {
            byte[] buffer = new byte[_width * _height * 2];
            if (0 == _tsdk.dirp_get_original_raw(_ph, buffer, buffer.Length))
                return buffer;
            return new byte[0];
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_ph != System.IntPtr.Zero)
            {
                try
                {
                    _tsdk.dirp_destroy(_ph);
                    _ph = System.IntPtr.Zero;
                }
                finally
                {
                    _ph = System.IntPtr.Zero;
                }
            }
            _mData = null;
        }
    }
}