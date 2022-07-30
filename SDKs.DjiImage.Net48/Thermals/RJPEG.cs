﻿namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// 大疆无人机 R-JPEG 热红外照片
    /// </summary>
    /// <remarks>支持：禅思 H20N、禅思 Zenmuse XT S、禅思 Zenmuse H20 系列、经纬 M30 系列、御 2 行业进阶版</remarks>
    public sealed class RJPEG : IJPEG
    {
        System.IntPtr _ph = System.IntPtr.Zero;
        float[,] mData = null;

        /// <summary>
        /// 图像宽度
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// 图像高度
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// 红外测温参数
        /// </summary>
        public MeasureParam Params { get; private set; }

        AreaTemperature _areaTemp;
        public float MaxTemp => _areaTemp.MaxTemp;
        public float MinTemp => _areaTemp.MinTemp;
        public float AvgTemp => _areaTemp.AvgTemp;
        public System.Collections.Generic.List<Location> MinTempLocs => _areaTemp.MinTempLocs;
        public System.Collections.Generic.List<Location> MaxTempLocs => _areaTemp.MaxTempLocs;

        /// <summary>
        /// 文件大小
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// XMP Meta drone-dji 信息。
        /// </summary>
        public RdfDroneDji DroneDji { get; private set; }

        private RJPEG()
        {
        }

        /// <summary>
        /// 从指定文件创建大疆热红外 R-JPEG 图片
        /// </summary>
        /// <param name="filename">JPEG 文件路径</param>
        /// <returns></returns>
        public static RJPEG FromFile(string filename)
        {
            if (filename == null)
                throw new System.ArgumentNullException(nameof(filename));

            using (var stream = System.IO.File.OpenRead(filename))
                return FromStream(stream);
        }
        /// <summary>
        /// 从指定文件流创建大疆热红外 R-JPEG 图片
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static RJPEG FromStream(System.IO.Stream stream)
        {
            if (stream == null || stream == System.IO.Stream.Null)
                throw new System.ArgumentNullException(nameof(stream));
            if (stream.Length == 0)
                throw new System.ArgumentException("stream's length can not be zero.", nameof(stream));

            int len = (int)stream.Length;
            byte[] buffer = new byte[len];
            stream.Read(buffer, 0, buffer.Length);
            var img = new RJPEG();
            int code = img.Load(buffer);
            if (code == 0)
            {
                img.DroneDji = Rdf.GetDroneDji(buffer);
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
                img.DroneDji = Rdf.GetDroneDji(bytes);
                return img;
            }
            img.Dispose();
            throw new System.ArgumentException(((dirp_ret_code_e)code).ToString(), nameof(bytes));
        }

        int Load(byte[] bytes)
        {
            this.Size = bytes.Length;
            int code = _tsdk.dirp_create_from_rjpeg(bytes, Size, ref _ph);
            if (code == 0)
            {
                dirp_resolution_t res = new dirp_resolution_t();
                _tsdk.dirp_get_rjpeg_resolution(_ph, ref res);
                this.Width = res.width;
                this.Height = res.height;

                var mp = new MeasureParam();
                _tsdk.dirp_get_measurement_params(_ph, ref mp);
                this.Params = mp;

                int rawsize = res.width * res.height * 2;
                byte[] buffer = new byte[rawsize];
                _tsdk.dirp_measure(_ph, buffer, rawsize);
                _tsdk.dirp_destroy(_ph);
                this._ph = System.IntPtr.Zero;
                this.mData = Cast(buffer, res.width, res.height);
            }
            return code;
        }

        /// <summary>
        /// 获取图片指定位置的温度
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public float GetTemp(Location p)
        {
            if (p.Left < 0 || p.Left > Width - 1)
                throw new System.ArgumentOutOfRangeException(nameof(p.Left), p.Left, $"must be positive integer and less than {Width}.");

            if (p.Top < 0 || p.Top > Height - 1)
                throw new System.ArgumentOutOfRangeException(nameof(p.Top), p.Top, $"must be positive integer and less than {Height}.");

            return mData[p.Left, p.Top];
        }
        /// <summary>
        /// 获取图片指定位置的温度
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public float GetTemp(int left, int top)
        {
            if (left < 0 || left > Width - 1)
                throw new System.ArgumentOutOfRangeException(nameof(left), left, $"must be positive integer and less than {Width}.");

            if (top < 0 || top > Height - 1)
                throw new System.ArgumentOutOfRangeException(nameof(top), top, $"must be positive integer and less than {Height}.");

            return mData[left, top];
        }

        /// <summary>
        /// 获取图像指定直线上的温度
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public AreaTemperature GetTempLine(Location p1, Location p2)
        {
            if (p1.Left < 0 || p1.Left > Width - 1)
                throw new System.ArgumentOutOfRangeException(nameof(p1.Left), p1.Left, $"must be positive integer and less than {Width}.");
            if (p1.Top < 0 || p1.Top > Height - 1)
                throw new System.ArgumentOutOfRangeException(nameof(p1.Top), p1.Top, $"must be positive integer and less than {Height}.");
            if (p2.Left < 0 || p2.Left > Width - 1)
                throw new System.ArgumentOutOfRangeException(nameof(p2.Left), p2.Left, $"must be positive integer and less than {Width}..");
            if (p2.Top < 0 || p2.Top > Height - 1)
                throw new System.ArgumentOutOfRangeException(nameof(p2.Top), p2.Top, $"must be positive integer and less than {Height}.");

            return GetTempLine(p1.Left, p1.Top, p2.Left, p2.Top);
        }
        /// <summary>
        /// 获取图像指定直线上的温度
        /// </summary>
        /// <param name="left1"></param>
        /// <param name="top1"></param>
        /// <param name="left2"></param>
        /// <param name="top2"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public AreaTemperature GetTempLine(int left1, int top1, int left2, int top2)
        {
            if (left1 < 0 || left1 > Width - 1)
                throw new System.ArgumentOutOfRangeException(nameof(left1), left1, $"must be positive integer and less than {Width}.");
            if (top1 < 0 || top1 > Height - 1)
                throw new System.ArgumentOutOfRangeException(nameof(top1), top1, $"must be positive integer and less than {Height}.");
            if (left2 < 0 || left2 > Width - 1)
                throw new System.ArgumentOutOfRangeException(nameof(left2), left2, $"must be positive integer and less than {Width}..");
            if (top2 < 0 || top2 > Height - 1)
                throw new System.ArgumentOutOfRangeException(nameof(top2), top2, $"must be positive integer and less than {Height}.");

            var result = new AreaTemperature();
            var minList = new System.Collections.Generic.List<Location>();
            var maxList = new System.Collections.Generic.List<Location>();

            Location loc = new Location(left1, top1);
            if (left1 > left2)
            {
                left1 = left2;
                top1 = top2;
                left2 = loc.Left;
                top2 = loc.Top;
            }
            float temp = mData[left1, top1];
            result.MinTemp = temp;
            result.MaxTemp = temp;
            result.AvgTemp = temp;
            minList.Add(loc);
            maxList.Add(loc);

            if (left1 == left2 && top1 == top2)
                return result;

            float sumTemp = 0;
            int miny = System.Math.Min(top1, top2);
            int maxy = System.Math.Max(top1, top2);
            if (left1 == left2)
            {
                for (int i = miny; i <= maxy; i++)
                {
                    temp = mData[left1, i];
                    sumTemp += temp;
                    RefProcess(ref result, minList, maxList, temp, left1, i);
                }
                result.MinTempLocs = minList;
                result.MaxTempLocs = maxList;
                result.AvgTemp = float.Parse((sumTemp / (maxy - miny + 1)).ToString("f1"));
                return result;
            }
            if (top1 == top2)
            {
                for (int i = left1; i <= left2; i++)
                {
                    temp = mData[i, top1];
                    sumTemp += temp;
                    RefProcess(ref result, minList, maxList, temp, i, top1);
                }
                result.MinTempLocs = minList;
                result.MaxTempLocs = maxList;
                result.AvgTemp = float.Parse((sumTemp / (left2 - left1 + 1)).ToString("f1"));
                return result;
            }
            decimal k = decimal.Divide(maxy - miny + 1, left2 - left1 + 1);
            bool up = top1 > top2;
            int j;
            int sum = 0;
            //up: j = maxy - [(maxy  - miny + 1)/(maxx - minx + 1)] * (i - minx + 1) + 1
            //dn: j = [(maxy - miny + 1)/(maxx - minx + 1)] * (i - minx + 1) + miny - 1
            for (int i = left1; i <= left2; i++)
            {
                j = System.Convert.ToInt32(up ? (maxy - (i - left1 + 1) * k + 1) : (i - left1 + 1) * k + miny - 1);
                temp = mData[i, j];
                sumTemp += temp;
                sum++;
                RefProcess(ref result, minList, maxList, temp, i, j);
            }
            result.MinTempLocs = minList;
            result.MaxTempLocs = maxList;
            result.AvgTemp = float.Parse((sumTemp / (left2 - left1 + 1)).ToString("f1"));
            return result;
        }

        public AreaTemperature GetTempRect(Location p, int width, int height)
        {
            if (p.Left < 0 || p.Left > Width - 1)
                throw new System.ArgumentOutOfRangeException(nameof(p.Left), p.Left, $"must be positive integer and less than {Width}.");

            if (p.Top < 0 || p.Top > Height - 1)
                throw new System.ArgumentOutOfRangeException(nameof(p.Top), p.Top, $"must be positive integer and less than {Height}.");

            int right = p.Left + width;
            int bottom = p.Top + height;

            if (right < 0 || right > Width - 1)
                throw new System.ArgumentOutOfRangeException(nameof(width), width, $"p.Left + width must be positive integer and less than {Width}.");

            if (bottom < 0 || bottom > Height - 1)
                throw new System.ArgumentOutOfRangeException(nameof(height), height, $"p.Top + height must be positive integer and less than {Height}.");

            return p.Left < right ? GetTempRect(p.Left, p.Top, right, bottom) : GetTempRect(right, bottom, p.Left, p.Top);
        }
        /// <summary>
        /// 获取图像指定矩形范围的温度
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public AreaTemperature GetTempRect(int left, int top, int right, int bottom)
        {
            if (left < 0 || left > Width - 1)
                throw new System.ArgumentOutOfRangeException(nameof(left), left, $"must be positive integer and less than {Width}.");
            if (top < 0 || top > Height - 1)
                throw new System.ArgumentOutOfRangeException(nameof(top), top, $"must be positive integer and less than {Height}.");
            if (right < 0 || right > Width - 1)
                throw new System.ArgumentOutOfRangeException(nameof(right), right, $"must be positive integer and less than {Width}.");
            if (bottom < 0 || bottom > Height - 1)
                throw new System.ArgumentOutOfRangeException(nameof(bottom), bottom, $"must be positive integer and less than {Height}.");

            int xoffset = right - left;
            if (xoffset < 0)
                throw new System.ArgumentOutOfRangeException(nameof(right), right, "right must greater than left.");

            int yoffset = bottom - top;
            if (yoffset < 0)
                throw new System.ArgumentOutOfRangeException(nameof(bottom), right, "bottom must greater than top.");

            float temp = mData[left, top];
            var result = new AreaTemperature();
            result.MinTemp = temp;
            result.MaxTemp = temp;
            result.AvgTemp = temp;

            var minList = new System.Collections.Generic.List<Location>();
            var maxList = new System.Collections.Generic.List<Location>();
            var floc = new Location(left, top);
            minList.Add(floc);
            maxList.Add(floc);

            float sumTemp = 0;
            for (int i = left; i <= right; i++)
            {
                for (int j = top; j <= bottom; j++)
                {
                    temp = mData[i, j];
                    sumTemp += temp;
                    RefProcess(ref result, minList, maxList, temp, i, j);
                }
            }
            int sumCount = (xoffset + 1) * (yoffset + 1);
            result.MinTempLocs = minList;
            result.MaxTempLocs = maxList;
            result.AvgTemp = float.Parse((sumTemp / sumCount).ToString("f1"));
            return result;
        }

        float[,] Cast(byte[] rawData, int width, int height)
        {
            var result = new float[width, height];
            var area = new AreaTemperature();
            area.MinTemp = short.MaxValue;
            area.MaxTemp = short.MinValue;
            var minList = new System.Collections.Generic.List<Location>();
            var maxList = new System.Collections.Generic.List<Location>();

            float temp;
            float sumTemp = 0;
            int index = 0;
            int i, j;
            byte[] arr = new byte[4];
            for (i = 0; i < height; i++)
            {
                for (j = 0; j < width; j++)
                {
                    arr[0] = rawData[index];
                    arr[1] = rawData[index + 1];
                    index += 2;
                    temp = System.BitConverter.ToInt16(arr, 0) * 0.1f;
                    result[j, i] = temp;

                    sumTemp += temp;
                    RefProcess(ref area, minList, maxList, temp, j, i);
                }
            }

            area.MinTempLocs = minList;
            area.MaxTempLocs = maxList;
            area.AvgTemp = float.Parse((sumTemp / result.Length).ToString("f1"));
            _areaTemp = area;
            return result;
        }

        static void RefProcess(ref AreaTemperature area, System.Collections.Generic.List<Location> minList, System.Collections.Generic.List<Location> maxList, float temp, int x, int y)
        {
            if (temp < area.MinTemp)
            {
                area.MinTemp = temp;
                minList.Clear();
                minList.Add(new Location() { Left = x, Top = y });
            }
            else if (temp == area.MinTemp)
            {
                minList.Add(new Location() { Left = x, Top = y });
            }
            if (temp > area.MaxTemp)
            {
                area.MaxTemp = temp;
                maxList.Clear();
                maxList.Add(new Location() { Left = x, Top = y });
            }
            else if (temp == area.MaxTemp)
            {
                maxList.Add(new Location() { Left = x, Top = y });
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_ph != System.IntPtr.Zero)
            {
                _tsdk.dirp_destroy(_ph);
                _ph = System.IntPtr.Zero;
            }
        }
    }
}