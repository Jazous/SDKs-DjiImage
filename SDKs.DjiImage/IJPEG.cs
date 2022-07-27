namespace SDKs.DjiImage
{
    public interface IJPEG : IDisposable
    {
        /// <summary>
        /// XMP Meta drone-dji 信息。
        /// </summary>
        public RdfDroneDji DroneDji { get; }
    }
}