namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// 大疆无人机 R-JPEG 热红外照片
    /// </summary>
    /// <remarks>支持：禅思 H20N、禅思 Zenmuse XT S、禅思 Zenmuse H20 系列、经纬 M30 系列、御 2 行业进阶版</remarks>
    public sealed class RJPEG : IJPEG
    {
        IntPtr _ph = IntPtr.Zero;
        decimal[,]? mData = null;

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
        public decimal MaxTemp => _areaTemp.MaxTemp;
        public decimal MinTemp => _areaTemp.MinTemp;
        public decimal AvgTemp => _areaTemp.AvgTemp;
        public List<Location> MinTempLocs => _areaTemp.MinTempLocs;
        public List<Location> MaxTempLocs => _areaTemp.MaxTempLocs;

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
                throw new ArgumentNullException("filename");

            using (var stream = File.OpenRead(filename))
                return FromStream(stream);
        }
        /// <summary>
        /// 从指定文件流创建大疆热红外 R-JPEG 图片
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
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

                int rawsize = res.width * res.height * 4;
                byte[] buffer = new byte[rawsize];
                TSDK.dirp_measure_ex(_ph, buffer, rawsize);
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

            return Math.Round(mData[left, top], 1);
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

            if (p1.Left > p2.Left)
            {
                Location p3 = p1;
                p1 = p2;
                p2 = p3;
            }
            decimal temp = mData[p1.Left, p1.Top];
            result.MinTemp = temp;
            result.MaxTemp = temp;
            result.AvgTemp = temp;

            minList.Add(p1);
            maxList.Add(p1);
            if (p1.Left == p2.Left && p1.Top == p2.Top)
                return result;

            int sum = 0;
            decimal sumTemp = 0;
            int miny = Math.Min(p1.Top, p2.Top);
            int maxy = Math.Max(p1.Top, p2.Top);
            if (p1.Left == p2.Left)
            {
                for (int i = miny; i <= maxy; i++)
                {
                    temp = mData[p1.Left, i];
                    sumTemp += temp;
                    sum++;
                    RefProcess(ref result, minList, maxList, temp , p1.Left, i);
                }
                result.MinTempLocs = minList;
                result.MaxTempLocs = maxList;
                result.AvgTemp = Math.Round(decimal.Divide(sumTemp, sum), 1);
                return result;
            }
            if (p1.Top == p2.Top)
            {
                for (int i = p1.Left; i <= p2.Left; i++)
                {
                    temp = mData[i, p1.Top];
                    sumTemp += temp;
                    sum++;
                    RefProcess(ref result, minList, maxList, temp, i, p1.Top);
                }
                result.MinTempLocs = minList;
                result.MaxTempLocs = maxList;
                result.AvgTemp = Math.Round(decimal.Divide(sumTemp, sum), 1);
                return result;
            }
            decimal k = decimal.Divide(Math.Abs(p2.Top - p1.Top), p2.Left - p1.Left);
            bool up = p1.Top > p2.Top;
            int lastjy = -1;
            for (int i = p1.Left; i <= p2.Left; i++)
            {
                int y = Convert.ToInt32(up ? (maxy - (i - p1.Left) * k) : miny + (i - p1.Left) * k);
                if (lastjy == y)
                    continue;
                lastjy = y;
                temp = mData[i, y];
                sumTemp += temp;
                sum++;
                RefProcess(ref result, minList, maxList, temp, i, y);
            }
            result.MinTempLocs = minList;
            result.MaxTempLocs = maxList;
            result.AvgTemp = Math.Round(decimal.Divide(sumTemp, sum), 1);
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

            decimal temp = mData[left, top];
            var result = new AreaTemperature();
            result.MinTemp = temp;
            result.MaxTemp = temp;
            result.AvgTemp = temp;

            List<Location> minList = new List<Location>();
            List<Location> maxList = new List<Location>();

            int sum = 0;
            decimal sumTemp = 0;
            for (int i = left; i < right; i++)
            {
                for (int j = top; j < bottom; j++)
                {
                    temp = mData[i, j];
                    sumTemp += temp;
                    sum++;
                    RefProcess(ref result, minList, maxList, temp, i, j);
                }
            }
            result.MinTempLocs = minList;
            result.MaxTempLocs = maxList;
            result.AvgTemp = Math.Round(decimal.Divide(sumTemp, sum), 1);
            return result;
        }

        decimal[,] Cast(byte[] rawData, int width, int height)
        {
            var result = new decimal[width, height];
            int index = 0;
            byte[] arr = new byte[4];
            decimal temp;
            int i, j;
            var area = new AreaTemperature();
            area.MinTemp = short.MaxValue;
            area.MaxTemp = short.MinValue;
            List<Location> minList = new List<Location>();
            List<Location> maxList = new List<Location>();

            int sum = 0;
            decimal sumTemp = 0;
            for (i = 0; i < height; i++)
            {
                for (j = 0; j < width; j++)
                {
                    arr[0] = rawData[index];
                    arr[1] = rawData[index + 1];
                    arr[2] = rawData[index + 2];
                    arr[3] = rawData[index + 3];
                    index += 4;
                    temp = Convert.ToDecimal(BitConverter.ToSingle(arr, 0));
                    result[j, i] = temp;

                    sumTemp += temp;
                    sum++;
                    RefProcess(ref area, minList, maxList, temp, j, i);
                }
            }

            area.MinTempLocs = minList;
            area.MaxTempLocs = maxList;
            area.AvgTemp = Math.Round(decimal.Divide(sumTemp, sum), 1);
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
        /// 释放资源
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