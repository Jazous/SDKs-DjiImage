namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// Isotherm parameters structure definition.
    /// </summary>
    struct dirp_isotherm_t
    {
        /// <summary>
        /// Isotherm switch, enable(true) or disable(false).
        /// </summary>
        public bool enable;
        /// <summary>
        /// Upper limit. Only effective when isotherm is enabled.
        /// </summary>
        public float high;
        /// <summary>
        /// Lower limit. Only effective when isotherm is enabled.
        /// </summary>
        public float low;
    }
}