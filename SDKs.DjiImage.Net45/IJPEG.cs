namespace SDKs.DjiImage
{
    /// <summary>
    /// 大疆图片接口
    /// </summary>
    public interface IJPEG : System.IDisposable
    {
        /// <summary>
        /// XMP Meta drone-dji 信息
        /// </summary>
        RdfDroneDji DroneDji { get; }
    }
}