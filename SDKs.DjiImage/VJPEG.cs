namespace SDKs.DjiImage
{
    /// <summary>
    /// 大疆无人机照片
    /// </summary>
    public sealed class VJPEG : IJPEG
    {
        /// <summary>
        /// XMP Meta drone-dji 信息
        /// </summary>
        public RdfDroneDji DroneDji { get; private set; }

        private VJPEG()
        {
            
        }

        /// <summary>
        /// 从指定文件创建大疆 JPEG 图片
        /// </summary>
        /// <param name="path">JPEG 文件路径</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        public static VJPEG FromFile(string path)
        {
            return FromStream(System.IO.File.OpenRead(path), false);
        }
        /// <summary>
        /// 从指定文件流创建大疆 JPEG 图片
        /// </summary>
        /// <param name="stream">图片字节流</param>
        /// <param name="leaveOpen">使用完后是否关闭流</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static VJPEG FromStream(System.IO.Stream stream, bool leaveOpen = false)
        {
            if (stream == null || stream == System.IO.Stream.Null)
                throw new System.ArgumentNullException(nameof(stream));
            if (stream.Length == 0)
                throw new System.ArgumentException("stream is invalid r-jpeg data.");

            return new VJPEG() { DroneDji = Rdf.GetDroneDji(stream, leaveOpen) };
        }

        /// <summary>
        /// 从指定文件字节数组中创建大疆热 JPEG 图片
        /// </summary>
        /// <param name="bytes">文件字节数组</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static VJPEG FromBytes(byte[] bytes)
        {
            if (bytes == null)
                throw new System.ArgumentNullException(nameof(bytes));
            if (bytes.Length == 0)
                throw new System.ArgumentException("bytes is invalid r-jpeg data.");

            return new VJPEG() { DroneDji = Rdf.GetDroneDji(bytes) };
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {

        }
    }
}