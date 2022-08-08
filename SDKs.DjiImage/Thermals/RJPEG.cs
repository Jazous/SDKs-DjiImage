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
        /// <summary>
        /// 图片最高温度
        /// </summary>
        public float MaxTemp => _areaTemp.MaxTemp;
        /// <summary>
        /// 图片最低温度
        /// </summary>
        public float MinTemp => _areaTemp.MinTemp;
        /// <summary>
        /// 图片平均温度
        /// </summary>
        public float AvgTemp => _areaTemp.AvgTemp;
        /// <summary>
        /// 最低温度位置列表
        /// </summary>
        public System.Collections.Generic.List<Location> MinTempLocs => _areaTemp.MinTempLocs;
        /// <summary>
        /// 最高温度位置列表
        /// </summary>
        public System.Collections.Generic.List<Location> MaxTempLocs => _areaTemp.MaxTempLocs;
        /// <summary>
        /// 文件大小
        /// </summary>
        public int Size { get; private set; }
        /// <summary>
        /// XMP Meta drone-dji 信息
        /// </summary>
        public RdfDroneDji DroneDji { get; private set; }

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
                img.DroneDji = Rdf.GetDroneDji(bytes);
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
                img.DroneDji = Rdf.GetDroneDji(buffer);
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
                img.DroneDji = Rdf.GetDroneDji(bytes);
                return img;
            }
            img.Dispose();
            return null;
        }
        int Load(byte[] bytes)
        {
            this.Size = bytes.Length;
            int code = _tsdk.dirp_create_from_rjpeg(bytes, Size, ref _ph);
            if (code == 0)
            {
                var res = new dirp_resolution_t();
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
        /// <param name="location"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public float GetTemp(Location location)
        {
            if (location.Left < 0 || location.Left > Width - 1)
                throw new System.ArgumentOutOfRangeException(nameof(location), location.Left, $"location.Left must be positive integer and less than {Width}.");

            if (location.Top < 0 || location.Top > Height - 1)
                throw new System.ArgumentOutOfRangeException(nameof(location), location.Top, $"location.Top must be positive integer and less than {Height}.");

            return mData[location.Left, location.Top];
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
        /// <param name="location1"></param>
        /// <param name="location2"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public AreaTemperature GetTempLine(Location location1, Location location2)
        {
            if (location1.Left < 0 || location1.Left > Width - 1)
                throw new System.ArgumentOutOfRangeException(nameof(location1), location1.Left, $"location1.Left must be positive integer and less than {Width}.");
            if (location1.Top < 0 || location1.Top > Height - 1)
                throw new System.ArgumentOutOfRangeException(nameof(location1), location1.Top, $"location1.Top must be positive integer and less than {Height}.");
            if (location2.Left < 0 || location2.Left > Width - 1)
                throw new System.ArgumentOutOfRangeException(nameof(location2), location2.Left, $"location2.Left must be positive integer and less than {Width}..");
            if (location2.Top < 0 || location2.Top > Height - 1)
                throw new System.ArgumentOutOfRangeException(nameof(location2), location2.Top, $"location2.Top must be positive integer and less than {Height}.");

            return GetTempLine(location1.Left, location1.Top, location2.Left, location2.Top);
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

            var loc = new Location(left1, top1);
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

            int xofffset = left2 - left1;
            int yofffset = System.Math.Abs(top2 - top1);

            if (xofffset == 0 && yofffset == 0)
                return result;

            float sumTemp = 0;
            int miny = System.Math.Min(top1, top2);
            int maxy = System.Math.Max(top1, top2);
            if (xofffset == 0)
            {
                for (int i = miny; i <= maxy; i++)
                {
                    temp = mData[left1, i];
                    sumTemp += temp;
                    RefProcess(ref result, minList, maxList, temp, left1, i);
                }
                result.MinTempLocs = minList;
                result.MaxTempLocs = maxList;
                result.AvgTemp = System.MathF.Round(sumTemp / (yofffset + 1), 1);
                return result;
            }
            if (yofffset == 0)
            {
                for (int i = left1; i <= left2; i++)
                {
                    temp = mData[i, top1];
                    sumTemp += temp;
                    RefProcess(ref result, minList, maxList, temp, i, top1);
                }
                result.MinTempLocs = minList;
                result.MaxTempLocs = maxList;
                result.AvgTemp = System.MathF.Round(sumTemp / (xofffset + 1), 1);
                return result;
            }
            decimal k = decimal.Divide(yofffset + 1, xofffset + 1);
            bool up = top1 > top2;
            int j;
            //up: j = maxy - [(maxy  - miny + 1)/(maxx - minx + 1)] * (i - minx + 1) + 1
            //dn: j = [(maxy - miny + 1)/(maxx - minx + 1)] * (i - minx + 1) + miny - 1
            for (int i = left1; i <= left2; i++)
            {
                j = System.Convert.ToInt32(up ? (maxy - (i - left1 + 1) * k + 1) : (i - left1 + 1) * k + miny - 1);
                temp = mData[i, j];
                sumTemp += temp;
                RefProcess(ref result, minList, maxList, temp, i, j);
            }
            result.MinTempLocs = minList;
            result.MaxTempLocs = maxList;
            result.AvgTemp = System.MathF.Round(sumTemp / (xofffset + 1), 1);
            return result;
        }
        /// <summary>
        /// 获取图像指定矩形范围的温度
        /// </summary>
        /// <param name="location"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public AreaTemperature GetTempRect(Location location, int width, int height)
        {
            if (location.Left < 0 || location.Left > Width - 1)
                throw new System.ArgumentOutOfRangeException(nameof(location), location.Left, $"location.Left must be positive integer and less than {Width}.");

            if (location.Top < 0 || location.Top > Height - 1)
                throw new System.ArgumentOutOfRangeException(nameof(location), location.Top, $"location.Topmust be positive integer and less than {Height}.");

            int right = location.Left + width;
            int bottom = location.Top + height;

            if (right < 0 || right > Width - 1)
                throw new System.ArgumentOutOfRangeException(nameof(width), width, $"location.Left + width must be positive integer and less than {Width}.");

            if (bottom < 0 || bottom > Height - 1)
                throw new System.ArgumentOutOfRangeException(nameof(height), height, $"location.Top + height must be positive integer and less than {Height}.");

            return location.Left < right ? GetTempRect(location.Left, location.Top, right, bottom) : GetTempRect(right, bottom, location.Left, location.Top);
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
            var result = new AreaTemperature
            {
                MinTemp = temp,
                MaxTemp = temp,
                AvgTemp = temp
            };
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
            result.AvgTemp = System.MathF.Round((sumTemp / sumCount), 1);
            return result;
        }
        /// <summary>
        /// 获取图像指定矩形范围的温度
        /// </summary>
        /// <param name="left">圆心水平方向位置</param>
        /// <param name="top">圆心垂直方向位置</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public AreaTemperature GetTempCircle(int left, int top, int radius)
        {
            if (left < 0 || left > Width - 1)
                throw new System.ArgumentOutOfRangeException(nameof(left), left, $"must be positive integer and less than {Width}.");
            if (top < 0 || top > Height - 1)
                throw new System.ArgumentOutOfRangeException(nameof(top), top, $"must be positive integer and less than {Height}.");
            if (radius < 0)
                throw new System.ArgumentOutOfRangeException(nameof(radius), radius, $"must be positive integer.");

            float temp = mData[left, top];
            var result = new AreaTemperature
            {
                MinTemp = temp,
                MaxTemp = temp,
                AvgTemp = temp
            };
            var minList = new System.Collections.Generic.List<Location>();
            var maxList = new System.Collections.Generic.List<Location>();
            var floc = new Location(left, top);
            if (radius == 0)
            {
                minList.Add(floc);
                maxList.Add(floc);
                return result;
            }

            int rxr = radius * radius;
            int h;
            int y;
            int ymin;
            int ymax;
            int xL;
            int xR;
            float sumTemp = 0;
            int sumCount = 0;
            for (int i = 0; i < radius; i++)
            {
                h = System.Convert.ToInt32(System.Math.Floor(System.Math.Sqrt(rxr - System.Math.Pow(i, 2))));
                ymin = top - h + 1;
                ymax = top + h + 1;
                if (ymin < 0) ymin = 0;
                if (ymax > Height) ymax = Height;

                xL = top - i;
                xR = top + i;

                for (y = ymin; y < ymax; y++)
                {
                    if (xR < Width)
                    {
                        temp = mData[xR, y];
                        sumTemp += temp;
                        sumCount++;
                        RefProcess(ref result, minList, maxList, temp, xR, y);
                    }
                    if (xL > 0)
                    {
                        temp = mData[xL, y];
                        sumTemp += temp;
                        sumCount++;
                        RefProcess(ref result, minList, maxList, temp, xL, y);
                    }
                }
            }

            result.MinTempLocs = minList;
            result.MaxTempLocs = maxList;
            result.AvgTemp = System.MathF.Round((sumTemp / sumCount), 1);
            return result;
        }
        /// <summary>
        /// 获取指定温度范围的位置温度清单
        /// </summary>
        /// <param name="predicate">温度过滤条件</param>
        /// <returns></returns>
        public System.Collections.Generic.List<RTEntry> Get(System.Predicate<float> predicate)
        {
            var result = new System.Collections.Generic.List<RTEntry>();
            for (int j = 0; j < this.Height; j++)
            {
                for (int i = 0; i < this.Width; i++)
                {
                    var temp = mData[i, j];
                    if (predicate.Invoke(temp))
                    {
                        result.Add(new RTEntry() { Left = i, Top = j, Temp = temp });
                    }
                }
            }
            return result;
        }
        float[,] Cast(byte[] rawData, int width, int height)
        {
            var result = new float[width, height];
            var area = new AreaTemperature
            {
                MinTemp = short.MaxValue,
                MaxTemp = short.MinValue
            };
            var minList = new System.Collections.Generic.List<Location>();
            var maxList = new System.Collections.Generic.List<Location>();

            float temp;
            float sumTemp = 0;
            int index = 0;
            int i, j;
            byte[] arr = new byte[2];
            for (i = 0; i < height; i++)
            {
                for (j = 0; j < width; j++)
                {
                    arr[0] = rawData[index];
                    arr[1] = rawData[index + 1];
                    index += 2;
                    temp = System.MathF.Round(System.BitConverter.ToInt16(arr, 0) * 0.1f, 1);
                    result[j, i] = temp;

                    sumTemp += temp;
                    RefProcess(ref area, minList, maxList, temp, j, i);
                }
            }

            area.MinTempLocs = minList;
            area.MaxTempLocs = maxList;
            area.AvgTemp = System.MathF.Round((sumTemp / result.Length), 1);
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