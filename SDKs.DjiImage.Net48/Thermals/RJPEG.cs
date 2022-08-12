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
namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// 大疆无人机 R-JPEG 热红外照片
    /// </summary>
    /// <remarks>支持：禅思 H20N、禅思 Zenmuse XT S、禅思 Zenmuse H20 系列、经纬 M30 系列、御 2 行业进阶版</remarks>
    public sealed class RJPEG : IJPEG, IAreaTemperature
    {
        System.IntPtr _ph = System.IntPtr.Zero;
        float[,] _mData = null;
        float _mintemp;
        float _maxtemp;
        float _avgtemp;
        int _width;
        int _height;
        RdfDroneDji _droneDji;
        MeasureParam _params;

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
                img._droneDji = Rdf.GetDroneDji(buffer);
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
                img._droneDji = Rdf.GetDroneDji(bytes);
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
                img._droneDji = Rdf.GetDroneDji(buffer);
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
                img._droneDji = Rdf.GetDroneDji(bytes);
                return img;
            }
            img.Dispose();
            return null;
        }
        int Load(byte[] bytes)
        {
            int code = _tsdk.dirp_create_from_rjpeg(bytes, bytes.Length, ref _ph);
            if (code == 0)
            {
                var res = new dirp_resolution_t();
                try
                {
                    _tsdk.dirp_get_rjpeg_resolution(_ph, ref res);
                    _width = res.width;
                    _height = res.height;

                    var mp = new MeasureParam();
                    _tsdk.dirp_get_measurement_params(_ph, ref mp);
                    _params = mp;

                    int rawsize = res.width * res.height * 2;
                    byte[] buffer = new byte[rawsize];
                    _tsdk.dirp_measure(_ph, buffer, rawsize);
                    _tsdk.dirp_destroy(_ph);
                    _ph = System.IntPtr.Zero;
                    _mData = Cast(buffer, res.width, res.height);
                }
                finally
                {
                    _tsdk.dirp_destroy(_ph);
                    _ph = System.IntPtr.Zero;
                }
            }
            return code;
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
        /// <param name="predicate">参数以此对应 Left、Top、Temp</param>
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
        /// 获取图片指定位置的温度
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public float GetTemp(Location p)
        {
            return GetTemp(p.Left, p.Top);
        }
        /// <summary>
        /// 获取图片指定位置的温度
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
        ///获取图像指定直线上的位置温度集合
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public LTCollection GetLine(Location p1, Location p2)
        {
            return GetLine(p1.Left, p1.Top, p2.Left, p2.Top);
        }
        /// <summary>
        /// 获取图像指定直线上的位置温度集合
        /// </summary>
        /// <param name="leftA"></param>
        /// <param name="topA"></param>
        /// <param name="leftB"></param>
        /// <param name="topB"></param>
        /// <returns></returns>
        public LTCollection GetLine(int leftA, int topA, int leftB, int topB)
        {
            var result = new LTCollection();
            int left, right;
            if (leftA > leftB)
            {
                left = leftA;
                right = topA;
                leftA = leftB;
                topA = topB;
                leftB = left;
                topB = right;
            }
            int w = _width - 1;
            int h = _height - 1;

            if (leftA > w || leftB < 0)
                return result;

            int miny = System.Math.Min(topA, topB);
            int maxy = System.Math.Max(topA, topB);
            if (miny > h || maxy < 0)
                return result;

            int xofffset = leftB - leftA;
            int yofffset = System.Math.Abs(topB - topA);

            if (xofffset == 0 && yofffset == 0)
            {
                if (leftA >= 0 && leftA < w && topA >= 0 && topA < h)
                    result.Add(leftA, topA, _mData[leftA, topA]);
                return result;
            }

            if (xofffset == 0)
            {
                if (leftA >= 0 && leftA <= w)
                {
                    if (miny < 0) miny = 0;
                    if (maxy > h) maxy = h;
                    for (int i = miny; i <= maxy; i++)
                        result.Add(leftA, i, _mData[leftA, i]);
                }
                return result;
            }
            if (yofffset == 0)
            {
                if (topA >= 0 && topA <= h)
                {
                    if (leftA < 0) leftA = 0;
                    if (leftB > w) leftB = w;
                    for (int i = leftA; i <= leftB; i++)
                        result.Add(i, topA, _mData[i, topA]);
                }
                return result;
            }
            else
            {
                decimal k = decimal.Divide(yofffset + 1, xofffset + 1);
                bool up = topA > topB;
                int j;
                int xmin = leftA;
                int xmax = leftB;
                if (xmin < 0) xmin = 0;
                if (xmax > 0) xmax = w;
                for (int i = xmin; i <= xmax; i++)
                {
                    j = System.Convert.ToInt32(up ? (maxy - (i - leftA + 1) * k + 1) : (i - leftA + 1) * k + miny - 1);
                    if (j >= 0 && j <= h)
                        result.Add(i, j, _mData[i, j]);
                }
                return result;
            }
        }
        /// <summary>
        /// 获取图像指定直线上的温度
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public AreaTemperature GetLineTemp(Location p1, Location p2)
        {
            return GetLineTemp(p1.Left, p1.Top, p2.Left, p2.Top);
        }
        /// <summary>
        /// 获取图像指定直线上的温度
        /// </summary>
        /// <param name="leftA"></param>
        /// <param name="topA"></param>
        /// <param name="leftB"></param>
        /// <param name="topB"></param>
        /// <returns></returns>
        public AreaTemperature GetLineTemp(int leftA, int topA, int leftB, int topB)
        {
            int left, right;
            if (leftA > leftB)
            {
                left = leftA;
                right = topA;
                leftA = leftB;
                topA = topB;
                leftB = left;
                topB = right;
            }
            int w = _width - 1;
            int h = _height - 1;

            if (leftA > w || leftB < 0)
                return AreaTemperature.Empty;

            int miny = System.Math.Min(topA, topB);
            int maxy = System.Math.Max(topA, topB);
            if (miny > h || maxy < 0)
                return AreaTemperature.Empty;

            int xofffset = leftB - leftA;
            int yofffset = System.Math.Abs(topB - topA);
            float temp;
            if (xofffset == 0 && yofffset == 0)
            {
                if (leftA >= 0 && leftA < w && topA >= 0 && topB < h)
                {
                    temp = _mData[leftA, topA];
                    return new AreaTemperature(temp, temp, temp);
                }
                return AreaTemperature.Empty;
            }

            if (xofffset == 0)
            {
                if (leftA >= 0 && leftA <= w)
                {
                    if (miny < 0) miny = 0;
                    if (maxy > h) maxy = h;
                    temp = _mData[leftA, miny];
                    float mintemp = temp;
                    float maxtemp = temp;
                    float sumtemp = 0;
                    for (int i = miny; i <= maxy; i++)
                    {
                        temp = _mData[leftA, i];
                        if (temp < mintemp)
                            mintemp = temp;
                        if (temp > maxtemp)
                            maxtemp = temp;
                        sumtemp += temp;
                    }
                    return new AreaTemperature(mintemp, maxtemp, float.Parse((sumtemp / (maxy - miny + 1)).ToString("f1")));
                }
                return AreaTemperature.Empty;
            }
            if (yofffset == 0)
            {
                if (topA >= 0 && topA <= h)
                {
                    if (leftA < 0) leftA = 0;
                    if (leftB > w) leftB = w;
                    temp = _mData[leftA, topA];
                    float mintemp = temp;
                    float maxtemp = temp;
                    float sumtemp = 0;
                    for (int i = leftA; i <= leftB; i++)
                    {
                        temp = _mData[i, topA];
                        if (temp < mintemp)
                            mintemp = temp;
                        if (temp > maxtemp)
                            maxtemp = temp;
                        sumtemp += temp;
                    }
                    return new AreaTemperature(mintemp, maxtemp, float.Parse((sumtemp / (leftB - leftA + 1)).ToString("f1")));
                }
                return AreaTemperature.Empty;
            }
            else
            {
                decimal k = decimal.Divide(yofffset + 1, xofffset + 1);
                bool up = topA > topB;
                int j;
                int xmin = leftA;
                int xmax = leftB;
                if (xmin < 0) xmin = 0;
                if (xmax > 0) xmax = w;
                float mintemp = float.NaN;
                float maxtemp = float.NaN;
                float sumtemp = 0;
                int sumcount = 0;
                for (int i = xmin; i <= xmax; i++)
                {
                    //up: j = maxy - [(maxy  - miny + 1)/(maxx - minx + 1)] * (i - minx + 1) + 1
                    //dn: j = [(maxy - miny + 1)/(maxx - minx + 1)] * (i - minx + 1) + miny - 1
                    j = System.Convert.ToInt32(up ? (maxy - (i - leftA + 1) * k + 1) : (i - leftA + 1) * k + miny - 1);
                    if (j >= 0 && j <= h)
                    {
                        temp = _mData[i, j];
                        if (sumcount == 0)
                        {
                            mintemp = temp;
                            maxtemp = temp;
                        }
                        else
                        {
                            if (temp < mintemp)
                                mintemp = temp;
                            if (temp > maxtemp)
                                maxtemp = temp;
                        }
                        sumcount++;
                        sumtemp += temp;
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
            if (left > right || top > bottom)
                return result;
            if (bottom < 0 || top > h || left > w || right < 0)
                return result;

            if (left <= 0) left = 0;
            if (left > w) left = w;

            if (right <= 0) right = 0;
            if (right > w) right = w;

            if (top <= 0) top = 0;
            if (top > h) top = h;

            if (bottom <= 0) bottom = 0;
            if (bottom > h) bottom = h;


            for (int i = left; i <= right; i++)
                for (int j = top; j <= bottom; j++)
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
            if (left > right || top > bottom)
                return AreaTemperature.Empty;
            if (bottom < 0 || top > h || left > w || right < 0)
                return AreaTemperature.Empty;

            if (left <= 0) left = 0;
            if (left > w) left = w;

            if (right <= 0) right = 0;
            if (right > w) right = w;

            if (top <= 0) top = 0;
            if (top > h) top = h;

            if (bottom <= 0) bottom = 0;
            if (bottom > h) bottom = h;

            float temp = _mData[left, top];
            float mintemp = temp;
            float maxtemp = temp;
            float sumtemp = 0;
            for (int i = left; i <= right; i++)
            {
                for (int j = top; j <= bottom; j++)
                {
                    temp = _mData[i, j];
                    sumtemp += temp;
                    if (mintemp > temp) mintemp = temp;
                    if (maxtemp < temp) maxtemp = temp;
                }
            }
            float avgtemp = float.Parse((sumtemp / ((right - left + 1) * (bottom - top + 1))).ToString("f1"));
            return new AreaTemperature(mintemp, maxtemp, avgtemp);
        }
        /// <summary>
        /// 获取图像指定圆形范围内的位置温度集合
        /// </summary>
        /// <param name="left">圆心水平方向位置</param>
        /// <param name="top">圆心垂直方向位置</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        public LTCollection GetCircle(int left, int top, int radius)
        {
            var result = new LTCollection();
            if (radius <= 0)
                return result;

            int h = _height - 1;
            int w = _width - 1;

            if (left < -radius || left > w + radius)
                return result;
            if (top < -radius || top > h + radius)
                return result;

            int rxr = checked(radius * radius);
            int dy, y, yT, yB, xL, xR;

            for (int i = 1; i <= radius; i++)
            {
                xL = left - i;
                xR = left + i;
                if (xR < 0 || xL > h)
                    continue;

                dy = System.Convert.ToInt32(System.Math.Sqrt(rxr - System.Math.Pow(i, 2)));
                yT = top - dy;
                yB = top + dy;
                if (yB < 0 || yT > h)
                    continue;

                if (yT < 0) yT = 0;
                if (yB > h) yB = h;

                if (xL >= 0 && xL <= w)
                    for (y = yT; y <= yB; y++)
                        result.Add(xL, y, _mData[xL, y]);

                if (xR >= 0 && xR <= w)
                    for (y = yT; y <= yB; y++)
                        result.Add(xR, y, _mData[xR, y]);
            }
            if (left >= 0 && left <= w)
            {
                yT = top - radius;
                yB = top + radius;
                if (yT < 0) yT = 0;
                if (yB > h) yB = h;

                for (y = yT; y <= yB; y++)
                    result.Add(left, y, _mData[left, y]);
            }
            return result;
        }
        /// <summary>
        /// 获取图像指定圆形范围内的位置温度
        /// </summary>
        /// <param name="left">圆心水平方向位置</param>
        /// <param name="top">圆心垂直方向位置</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        public AreaTemperature GetCircleTemp(int left, int top, int radius)
        {
            if (radius <= 0)
                return AreaTemperature.Empty;

            int h = _height - 1;
            int w = _width - 1;

            if (left < -radius || left > w + radius)
                return AreaTemperature.Empty; 
            if (top < -radius || top > h + radius)
                return AreaTemperature.Empty;

            int rxr = checked(radius * radius);
            int dy, y, yT, yB, xL, xR;
            float mintemp = _maxtemp;
            float maxtemp = _mintemp;
            float sumtemp = 0;
            int sumcount = 0;
            float temp;
            for (int i = 1; i <= radius; i++)
            {
                xL = left - i;
                xR = left + i;
                if (xR < 0 || xL > h)
                    continue;

                dy = System.Convert.ToInt32(System.Math.Sqrt(rxr - System.Math.Pow(i, 2)));
                yT = top - dy;
                yB = top + dy;
                if (yB < 0 || yT > h)
                    continue;

                if (yT < 0) yT = 0;
                if (yB > h) yB = h;

                if (xL >= 0 && xL <= w)
                {
                    for (y = yT; y <= yB; y++)
                    {
                        temp = _mData[xL, y];
                        if (mintemp > temp) mintemp = temp;
                        if (maxtemp < temp) maxtemp = temp;
                        sumtemp += temp;
                        sumcount++;
                    }
                }

                if (xR >= 0 && xR <= w)
                {
                    for (y = yT; y <= yB; y++)
                    {
                        temp = _mData[xR, y];
                        if (mintemp > temp) mintemp = temp;
                        if (maxtemp < temp) maxtemp = temp;
                        sumtemp += temp;
                        sumcount++;
                    }
                }
            }
            if (left >= 0 && left <= w)
            {
                yT = top - radius;
                yB = top + radius;
                if (yT < 0) yT = 0;
                if (yB > h) yB = h;

                for (y = yT; y <= yB; y++)
                {
                    temp = _mData[left, y];
                    if (mintemp > temp) mintemp = temp;
                    if (maxtemp < temp) maxtemp = temp;
                    sumtemp += temp;
                    sumcount++;
                }
            }
            return new AreaTemperature(mintemp, maxtemp, float.Parse((sumtemp / sumcount).ToString("f1")));
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
            for (i = 0; i < height; i++)
            {
                for (j = 0; j < width; j++)
                {
                    arr[0] = rawData[index];
                    arr[1] = rawData[index + 1];
                    index += 2;
                    temp = float.Parse((System.BitConverter.ToInt16(arr, 0) * 0.100000f).ToString("f1"));
                    result[j, i] = temp;
                    sumTemp += temp;
                    if (mintemp > temp)
                        mintemp = temp;
                    if (maxtemp < temp)
                        maxtemp = temp;
                }
            }
            this._mintemp = mintemp;
            this._maxtemp = maxtemp;
            this._avgtemp = float.Parse((sumTemp / result.Length).ToString("f1"));
            return result;
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