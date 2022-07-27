namespace SDKs.DjiImage.Thermals
{
    struct dirp_measurement_params_t
    {
        /// <summary>
        /// 距离。[1^25] 米
        /// </summary>
        public float distance;
        /// <summary>
        /// 空气湿度。[20^100]%
        /// </summary>
        public float humidity;
        /// <summary>
        /// 发射率。[0.10^1.00]
        /// </summary>
        public float emissivity;
        /// <summary>
        /// 反射温度。[-40.0~500.0]
        /// </summary>
        public float reflection;
    }
}