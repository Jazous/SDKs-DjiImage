namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// 大疆无人机 R-JPEG 热红外照片
    /// </summary>
    /// <remarks>支持：禅思 H20N、禅思 Zenmuse XT S、禅思 Zenmuse H20 系列、经纬 M30 系列、御 2 行业进阶版</remarks>
    public sealed class RJPEG : IJPEG
    {
        IntPtr _ph = IntPtr.Zero;
        short[,]? mData = null;

        /// <summary>
        /// 图像宽度
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// 图像高度
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// 红外测温参数。
        /// </summary>
        public MeasureParam Params { get; private set; }

        AreaTemperature _areaTemp;
        public decimal MaxTemp => _areaTemp.MaxTemp;
        public decimal MinTemp => _areaTemp.MinTemp;
        public decimal AvgTemp => _areaTemp.AvgTemp;
        public List<Location> MinTempLocs => _areaTemp.MinTempLocs;
        public List<Location> MaxTempLocs => _areaTemp.MaxTempLocs;

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
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static RJPEG FromFile(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");

            using (var stream = File.OpenRead(filename))
                return FromStream(stream);
        }
        /// <summary>
        /// 从指定文件流创建大疆热红外 R-JPEG 图片
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static RJPEG FromStream(Stream stream)
        {
            if (stream == null || stream == Stream.Null)
                throw new ArgumentNullException(nameof(stream));
            if (stream.Length == 0)
                throw new InvalidDataException("stream is invalid r-jpeg data.");

            int len = (int)stream.Length;
            byte[] buffer = new byte[len];
            stream.Read(buffer, 0, buffer.Length);
            var img = new RJPEG();
            int code = img.Load(buffer);
            if (code == 0)
            {
                img.DroneDji = Rdf.GetDroneDji(buffer);
                stream.Position = 0;
                return img;
            }
            img.Dispose();
            throw new InvalidDataException(((dirp_ret_code_e)code).ToString());
        }

        /// <summary>
        /// 从指定字节数组创建大疆热红外 R-JPEG 图片
        /// </summary>
        /// <param name="bytes">文件字节数组</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public static RJPEG FromBytes(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length == 0)
                throw new InvalidDataException("bytes is invalid r-jpeg data.");

            var img = new RJPEG();
            int code = img.Load(bytes);
            if (code == 0)
            {
                img.DroneDji = Rdf.GetDroneDji(bytes);
                return img;
            }
            img.Dispose();
            throw new InvalidDataException(((dirp_ret_code_e)code).ToString());
        }

        int Load(byte[] bytes)
        {
            Size = bytes.Length;
            int code = TSDK.dirp_create_from_rjpeg(bytes, Size, ref _ph);
            if (code == 0)
            {
                dirp_resolution_t res = new dirp_resolution_t();
                TSDK.dirp_get_rjpeg_resolution(_ph, ref res);
                Width = res.width;
                Height = res.height;

                var dmp = new dirp_measurement_params_t();
                TSDK.dirp_get_measurement_params(_ph, ref dmp);

                var mp = new MeasureParam();
                mp.Distance = Math.Round(Convert.ToDecimal(dmp.distance), 2);
                mp.Humidity = Convert.ToInt32(dmp.humidity);
                mp.Reflection = Math.Round(Convert.ToDecimal(dmp.reflection), 1);
                mp.Emissivity = Math.Round(Convert.ToDecimal(dmp.emissivity), 2);
                Params = mp;

                int rawsize = res.width * res.height * 2;
                byte[] buffer = new byte[rawsize];
                TSDK.dirp_measure(_ph, buffer, rawsize);
                TSDK.dirp_destroy(_ph);
                _ph = IntPtr.Zero;
                mData = Cast(buffer, res.width, res.height);
            }
            return code;
        }

        /// <summary>
        /// 获取图片指定位置的温度
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public decimal GetTemp(int left, int top)
        {
            if (left < 0 || left > Width - 1)
                throw new IndexOutOfRangeException($"The left must range from 0 to {Width - 1} ,but now is {left}");

            if (top < 0 || top > Height - 1)
                throw new IndexOutOfRangeException($"The top must range from 0 to {Height - 1} ,but now is {top}");

            return mData[left, top] * 0.1m;
        }

        /// <summary>
        /// 获取图像指定直线上的温度
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public AreaTemperature GetTempLine(Location p1, Location p2)
        {
            if (p1.Left < 0 || p1.Left > Width - 1)
                throw new IndexOutOfRangeException($"The p1.Left must range from 0 to {Width - 1} ,but now is {p1.Left}");
            if (p1.Top < 0 || p1.Top > Height - 1)
                throw new IndexOutOfRangeException($"The p1.Top must range from 0 to {Height - 1} ,but now is {p1.Top}");
            if (p2.Left < 0 || p2.Left > Width - 1)
                throw new IndexOutOfRangeException($"The p1.Left must range from 0 to {Width - 1} ,but now is {p2.Left}");
            if (p2.Top < 0 || p2.Top > Height - 1)
                throw new IndexOutOfRangeException($"The p2.Top must range from 0 to {Height - 1} ,but now is {p2.Top}");

            var result = new AreaTemperature();
            List<Location> minList = new List<Location>();
            List<Location> maxList = new List<Location>();

            int xoffset = p2.Left - p1.Left;
            int yoffset = p2.Top - p1.Top;
            int minx = p1.Left < p2.Left ? p1.Left : p2.Left;
            int maxx = p1.Left < p2.Left ? p2.Left : p1.Left;
            int miny = p1.Top < p2.Top ? p1.Top : p2.Top;
            int maxy = p1.Top < p2.Top ? p2.Top : p1.Top;
            int x = p1.Left < p2.Left ? p1.Left : p2.Left;
            int y = p1.Left < p2.Left ? p1.Top : p2.Top;

            short temp = mData[x, y];
            result.MinTemp = temp * 0.1m;
            result.MaxTemp = temp * 0.1m;
            result.AvgTemp = temp * 0.1m;
            var tp = new Location() { Left = x, Top = y };
            minList.Add(tp);
            maxList.Add(tp);
            if (xoffset == 0 && yoffset == 0)
                return result;

            int sum = 0;
            int sumTemp = 0;
            if (xoffset == 0)
            {
                for (int i = miny; i <= maxy; i++)
                {
                    temp = mData[x, i];
                    sum = sum + temp;
                    RefProcess(ref result, minList, maxList, temp * 0.1m, x, i);
                }
                result.MinTempLocs = minList;
                result.MaxTempLocs = maxList;
                result.AvgTemp = Math.Round(decimal.Divide(sum, maxy - miny) * 0.1m, 1);
                return result;
            }
            if (yoffset == 0)
            {
                for (int i = minx; i <= maxx; i++)
                {
                    temp = mData[i, y];
                    sum = sum + temp;
                    RefProcess(ref result, minList, maxList, temp * 0.1m, i, y);
                }
                result.MinTempLocs = minList;
                result.MaxTempLocs = maxList;
                result.AvgTemp = Math.Round(decimal.Divide(sum, maxx - minx) * 0.1m, 1);
                return result;
            }
            for (int i = minx; i <= maxx; i++)
            {
                for (int j = miny; j <= maxy; j++)
                {
                    int dx1 = i - minx;
                    int dy1 = j - miny;

                    int dx2 = maxx - i;
                    int dy2 = maxy - j;

                    if (dx1 * dy2 != dx2 * dy1)
                        continue;

                    temp = mData[i, j];
                    sumTemp += temp;
                    sum++;
                    RefProcess(ref result, minList, maxList, temp * 0.1m, i, j);
                }
            }
            result.MinTempLocs = minList;
            result.MaxTempLocs = maxList;
            result.AvgTemp = Math.Round(decimal.Divide(sumTemp, sum) * 0.1m, 1);
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
        /// <exception cref="IndexOutOfRangeException"></exception>
        public AreaTemperature GetTempRect(int left, int top, int right, int bottom)
        {
            if (left < 0 || left > Width - 1)
                throw new IndexOutOfRangeException($"The rect.Left must range from 0 to {Width - 1} ,but now is {left}");

            if (top < 0 || top > Height - 1)
                throw new IndexOutOfRangeException($"The rect.Top must range from 0 to {Height - 1} ,but now is {top}");

            if (right < 0 || right > Width - 1)
                throw new IndexOutOfRangeException($"The rect.Right must range from 0 to {Width - 1} ,but now is {right}");

            if (bottom < 0 || bottom > Height - 1)
                throw new IndexOutOfRangeException($"The rect.Bottom must range from 0 to {Height - 1} ,but now is {bottom}");

            short temp = mData[left, top];
            var result = new AreaTemperature();
            result.MinTemp = temp * 0.1m;
            result.MaxTemp = temp * 0.1m;
            result.AvgTemp = temp * 0.1m;

            List<Location> minList = new List<Location>();
            List<Location> maxList = new List<Location>();

            int sum = 0;
            int sumTemp = 0;
            for (int i = left; i < right; i++)
            {
                for (int j = top; j < bottom; j++)
                {
                    temp = mData[i, j];
                    sumTemp += temp;
                    sum++;
                    RefProcess(ref result, minList, maxList, temp * 0.1m, i, j);
                }
            }
            result.MinTempLocs = minList;
            result.MaxTempLocs = maxList;
            result.AvgTemp = Math.Round(decimal.Divide(sumTemp, sum) * 0.1m, 1);
            return result;
        }

        short[,] Cast(byte[] rawData, int width, int height)
        {
            short[,] result = new short[width, height];
            int index = 0;
            byte[] arr = new byte[2];
            short temp;
            int i, j;
            var area = new AreaTemperature();
            area.MinTemp = short.MaxValue;
            area.MaxTemp = short.MinValue;
            List<Location> minList = new List<Location>();
            List<Location> maxList = new List<Location>();

            int sum = 0;
            int sumTemp = 0;
            for (i = 0; i < height; i++)
            {
                for (j = 0; j < width; j++)
                {
                    arr[0] = rawData[index];
                    arr[1] = rawData[index + 1];
                    index = index + 2;
                    temp = BitConverter.ToInt16(arr, 0);
                    result[j, i] = temp;

                    sumTemp += temp;
                    sum++;
                    RefProcess(ref area, minList, maxList, temp * 0.1m, j, i);
                }
            }
            area.MinTempLocs = minList;
            area.MaxTempLocs = maxList;
            area.AvgTemp = Math.Round(decimal.Divide(sumTemp, sum) * 0.1m, 1);
            _areaTemp = area;
            return result;
        }

        static void RefProcess(ref AreaTemperature area, List<Location> minList, List<Location> maxList, decimal temp, int x, int y)
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
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            if (_ph != IntPtr.Zero)
            {
                TSDK.dirp_destroy(_ph);
                _ph = IntPtr.Zero;
            }
        }
    }
}