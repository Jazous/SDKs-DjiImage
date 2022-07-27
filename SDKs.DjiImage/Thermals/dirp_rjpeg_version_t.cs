namespace SDKs.DjiImage.Thermals
{
    struct dirp_rjpeg_version_t
    {
        /// <summary>
        /// Version number of the opened R-JPEG itself.
        /// </summary>
        int rjpeg;
        /// <summary>
        /// Version number of the header data in R-JPEG
        /// </summary>
        int header;
        /// <summary>
        /// Version number of the curve LUT data in R-JPEG
        /// </summary>
        int curve;
    }
}
