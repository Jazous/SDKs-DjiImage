namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// R-JPEG version structure definition
    /// </summary>
    struct dirp_rjpeg_version_t
    {
        /// <summary>
        /// Version number of the opened R-JPEG itself.
        /// </summary>
        public int rjpeg;
        /// <summary>
        /// Version number of the header data in R-JPEG
        /// </summary>
        public int header;
        /// <summary>
        /// Version number of the curve LUT data in R-JPEG
        /// </summary>
        public int curve;
    }
}
