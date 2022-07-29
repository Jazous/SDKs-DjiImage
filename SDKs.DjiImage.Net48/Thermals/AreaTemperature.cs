namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// 区域温度
    /// </summary>
    public struct AreaTemperature
    {
        /// <summary>
        /// 最高温度
        /// </summary>
        public float MaxTemp { get; set; }
        /// <summary>
        /// 最低温度
        /// </summary>
        public float MinTemp { get; set; }
        /// <summary>
        /// 平均温度
        /// </summary>
        public float AvgTemp { get; set; }
        /// <summary>
        /// 最高温度位置列表
        /// </summary>
        public System.Collections.Generic.List<Location> MaxTempLocs { get; set; }
        /// <summary>
        /// 最低温度位置列表
        /// </summary>
        public System.Collections.Generic.List<Location> MinTempLocs { get; set; }
    }
}