namespace SDKs.DjiImage
{
    /// <summary>
    /// 大疆无人机照片。
    /// </summary>
    public sealed class VJPEG : IJPEG
    {
        /// <summary>
        /// XMP Meta drone-dji 信息。
        /// </summary>
        public RdfDroneDji DroneDji { get; private set; }

        private VJPEG()
        {
            
        }

        /// <summary>
        /// 从指定文件创建大疆 JPEG 图片
        /// </summary>
        /// <param name="filename">JPEG 文件路径</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static VJPEG FromFile(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");

            using (var stream = File.OpenRead(filename))
                return FromStream(stream);
        }
        /// <summary>
        /// 从指定文件流创建大疆 JPEG 图片
        /// </summary>
        /// <param name="stream">图片字节流。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static VJPEG FromStream(Stream stream)
        {
            if (stream == null || stream == Stream.Null)
                throw new ArgumentNullException(nameof(stream));
            if (stream.Length == 0)
                throw new InvalidDataException("stream is invalid r-jpeg data.");
            
            int len = (int)stream.Length;
            byte[] buffer = new byte[len];
            stream.Read(buffer, 0, buffer.Length);
            var img = new VJPEG();
            img.DroneDji = Rdf.GetDroneDji(buffer);
            stream.Position = 0;
            return img;
        }

        /// <summary>
        /// 从指定文件字节数组中创建大疆热红外 R-JPEG 图片
        /// </summary>
        /// <param name="bytes">文件字节数组</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public static VJPEG FromBytes(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length == 0)
                throw new InvalidDataException("bytes is invalid r-jpeg data.");

            var img = new VJPEG();
            img.DroneDji = Rdf.GetDroneDji(bytes);
            return img;
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {

        }
    }
}