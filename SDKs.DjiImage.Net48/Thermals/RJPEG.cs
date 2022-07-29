namespace SDKs.DjiImage.Thermals
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

            var result = new AreaTemperature();
            var minList = new System.Collections.Generic.List<Location>();
            var maxList = new System.Collections.Generic.List<Location>();

            if (p1.Left > p2.Left)
            {
                Location p3 = p1;
                p1 = p2;
                p2 = p3;
            }
            float temp = mData[p1.Left, p1.Top];
            result.MinTemp = temp;
            result.MaxTemp = temp;
            result.AvgTemp = temp;
            minList.Add(p1);
            maxList.Add(p1);

            if (p1.Left == p2.Left && p1.Top == p2.Top)
                return result;

            float sumTemp = 0;
            int miny = System.Math.Min(p1.Top, p2.Top);
            int maxy = System.Math.Max(p1.Top, p2.Top);
            if (p1.Left == p2.Left)
            {
                for (int i = miny; i <= maxy; i++)
                {
                    temp = mData[p1.Left, i];
                    sumTemp += temp;
                    RefProcess(ref result, minList, maxList, temp, p1.Left, i);
                }
                result.MinTempLocs = minList;
                result.MaxTempLocs = maxList;
                result.AvgTemp = float.Parse((sumTemp / (maxy - miny + 1)).ToString("f1"));
                return result;
            }
            if (p1.Top == p2.Top)
            {
                for (int i = p1.Left; i <= p2.Left; i++)
                {
                    temp = mData[i, p1.Top];
                    sumTemp += temp;
                    RefProcess(ref result, minList, maxList, temp, i, p1.Top);
                }
                result.MinTempLocs = minList;
                result.MaxTempLocs = maxList;
                result.AvgTemp = float.Parse((sumTemp / (p2.Left - p1.Left + 1)).ToString("f1"));
                return result;
            }
            decimal k = decimal.Divide(maxy - miny + 1, p2.Left - p1.Left + 1);
            bool up = p1.Top > p2.Top;
            int j;
            int sum = 0;
            //up: j = maxy - [(maxy  - miny + 1)/(maxx - minx + 1)] * (i - minx + 1) + 1
            //dn: j = [(maxy - miny + 1)/(maxx - minx + 1)] * (i - minx + 1) + miny - 1
            for (int i = p1.Left; i <= p2.Left; i++)
            {
                j = System.Convert.ToInt32(up ? (maxy - (i - p1.Left + 1) * k + 1) : (i - p1.Left + 1) * k + miny - 1);
                temp = mData[i, j];
                sumTemp += temp;
                sum++;
                RefProcess(ref result, minList, maxList, temp, i, j);
            }
            result.MinTempLocs = minList;
            result.MaxTempLocs = maxList;
            result.AvgTemp = float.Parse((sumTemp / (p2.Left - p1.Left + 1)).ToString("f1"));
            return result;
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
            if (left > right)
                throw new System.ArgumentOutOfRangeException(nameof(right), right, "right must greater than left.");
            if (top > bottom)
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
            int sumCount = (right - left + 1) * (bottom - top + 1);
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

        static void RefProcess(ref AreaTemperature area,System.Collections.Generic.List<Location> minList, System.Collections.Generic.List<Location> maxList, float temp, int x, int y)
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