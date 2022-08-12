namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// 区域温度
    /// </summary>
    public interface IAreaTemperature
    {
        /// <summary>
        /// 最低温度
        /// </summary>
        public float MinTemp { get; }
        /// <summary>
        /// 最高温度
        /// </summary>
        public float MaxTemp { get; }
        /// <summary>
        /// 平均温度
        /// </summary>
        public float AvgTemp { get; }
    }
}