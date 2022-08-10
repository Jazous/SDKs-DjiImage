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
        LTCollection _coll = null;

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
        /// <summary>
        /// 图片最高温度
        /// </summary>
        public float MaxTemp => _coll.MaxTemp;
        /// <summary>
        /// 图片最低温度
        /// </summary>
        public float MinTemp => _coll.MinTemp;
        /// <summary>
        /// 图片平均温度
        /// </summary>
        public float AvgTemp => _coll.AvgTemp;
        /// <summary>
        /// 最低温度位置列表
        /// </summary>
        public System.Collections.ObjectModel.ReadOnlyCollection<Location> MinTempLocs => _coll.MinTempLocs;
        /// <summary>
        /// 最高温度位置列表
        /// </summary>
        public System.Collections.ObjectModel.ReadOnlyCollection<Location> MaxTempLocs => _coll.MaxTempLocs;
        /// <summary>
        /// 当前图片的位置温度集合
        /// </summary>
        public LTCollection Entries => _coll;
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
            int code = _tsdk.dirp_create_from_rjpeg(bytes, bytes.Length, ref _ph);
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
        /// 获取图片指定位置的位置温度集合
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public float GetTemp(Location loc)
        {
            return GetTemp(loc.Left, loc.Top);
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

            return mData[left, top];
        }
        /// <summary>
        /// 获取图像指定直线上的位置温度集合
        /// </summary>
        /// <param name="loc1"></param>
        /// <param name="loc2"></param>
        /// <returns></returns>
        public LTCollection GetLine(Location loc1, Location loc2)
        {
            return GetLine(loc1.Left, loc1.Top, loc2.Left, loc2.Top);
        }
        /// <summary>
        /// 获取图像指定直线上的位置温度集合
        /// </summary>
        /// <param name="left1"></param>
        /// <param name="top1"></param>
        /// <param name="left2"></param>
        /// <param name="top2"></param>
        /// <returns></returns>
        public LTCollection GetLine(int left1, int top1, int left2, int top2)
        {
            var result = new LTCollection();
            int left, right;
            if (left1 > left2)
            {
                left = left1;
                right = top1;
                left1 = left2;
                top1 = top2;
                left2 = left;
                top2 = right;
            }
            int w = Width - 1;
            int h = Height - 1;

            int xofffset = left2 - left1;
            int yofffset = System.Math.Abs(top2 - top1);

            if (xofffset == 0 && yofffset == 0)
            {
                if (left1 >= 0 && left1 < w && left2 >= 0 && top2 < h)
                    result.Add(left1, top1, mData[left1, top1]);
                return result;
            }

            int miny = System.Math.Min(top1, top2);
            int maxy = System.Math.Max(top1, top2);
            if (xofffset == 0)
            {
                if (miny < 0) miny = 0;
                if (miny > h) miny = h;
                if (maxy < 0) maxy = 0;
                if (maxy > h) maxy = h;
                if (left1 >= 0 && left1 < Width)
                    for (int i = miny; i <= maxy; i++)
                        result.Add(left1, i, mData[left1, i]);
                return result;
            }
            if (yofffset == 0)
            {
                if (left1 < 0) left1 = 0;
                if (left1 > w) left1 = w;
                if (left2 < 0) left2 = 0;
                if (left2 > w) left2 = w;
                if (top1 >= 0 && top1 < Height)
                    for (int i = left1; i <= left2; i++)
                        result.Add(i, top1, mData[i, top1]);
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
                if (i >= 0 && i < Width && j >= 0 && j < Height)
                    result.Add(i, j, mData[i, j]);
            }
            return result;
        }
        /// <summary>
        /// 获取图像指定矩形范围的位置温度集合
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public LTCollection GetRect(Location loc, int width, int height)
        {
            return GetRect(loc.Left, loc.Top, loc.Left + width, loc.Top + height);
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
            int w = Width - 1;
            int h = Height - 1;

            if (left <= 0) left = 0;
            if (left > w) left = w;

            if (right <= 0) right = 0;
            if (right > w) right = w - 1;

            if (top <= 0) top = 0;
            if (top > h) top = h - 1;

            if (bottom <= 0) bottom = 0;
            if (bottom > h) bottom = h - 1;

            var result = new LTCollection();
            for (int i = left; i <= right; i++)
                for (int j = top; j <= bottom; j++)
                    result.Add(i, j, mData[i, j]);

            return result;
        }
        /// <summary>
        /// 获取图像指定矩形范围的位置温度集合
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

            int ymaxIndex = Height - 1;
            int rxr = radius * radius;
            int h, y, ymin, ymax, xL, xR;

            for (int i = 1; i <= radius; i++)
            {
                h = System.Convert.ToInt32(System.Math.Floor(System.Math.Sqrt(rxr - System.Math.Pow(i, 2))));
                ymin = top - h;
                ymax = top + h;
                if (ymin < 0) ymin = 0;
                if (ymax > ymaxIndex) ymax = ymaxIndex;

                xL = left - i;
                xR = left + i;
                if (xL >= 0 && xL < Width)
                    for (y = ymin; y <= ymax; y++)
                        result.Add(xL, y, mData[xL, y]);

                if (xR >= 0 && xR < Width)
                    for (y = ymin; y <= ymax; y++)
                        result.Add(xR, y, mData[xR, y]);
            }

            ymin = top - radius;
            ymax = top + radius;
            if (ymin < 0) ymin = 0;
            if (ymax > ymaxIndex) ymax = ymaxIndex;

            if (left >= 0 && left < Width)
                for (y = ymin; y <= ymax; y++)
                    result.Add(left, y, mData[left, y]);

            return result;
        }
        float[,] Cast(byte[] rawData, int width, int height)
        {
            var result = new float[width, height];
            float temp;
            int index = 0;
            int i, j;
            var coll = new LTCollection(result.Length);
            byte[] arr = new byte[2];
            for (i = 0; i < height; i++)
            {
                for (j = 0; j < width; j++)
                {
                    arr[0] = rawData[index];
                    arr[1] = rawData[index + 1];
                    index += 2;
                    temp = float.Parse((System.BitConverter.ToInt16(arr, 0) * 0.1f).ToString("f1"));
                    result[j, i] = temp;
                    coll.Add(j, i, temp);
                }
            }
            this._coll = coll;
            return result;
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