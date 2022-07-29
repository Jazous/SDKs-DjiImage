namespace SDKs.DjiImage
{
    public interface IJPEG : System.IDisposable
    {
        /// <summary>
        /// XMP Meta drone-dji 信息
        /// </summary>
        RdfDroneDji DroneDji { get; }
    }
}