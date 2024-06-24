namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// 红外测温参数
    /// </summary>
    public struct MeasureParam
    {
        /// <summary>
        /// 距离
        /// </summary>
        /// <remarks>1^25 米</remarks>
        public float Distance { get; set; }
        /// <summary>
        /// 空气湿度
        /// </summary>
        /// <remarks>20^100（%）</remarks>
        public float Humidity { get; set; }
        /// <summary>
        /// 发射率
        /// </summary>
        /// <remarks>0.10^1.00</remarks>
        public float Emissivity { get; set; }
        /// <summary>
        /// 反射温度
        /// </summary>
        /// <remarks>-40.0~500.0</remarks>
        public float Reflection { get; set; }
        /// <summary>
        /// 环境温度，单位：摄氏度
        /// </summary>
        /// <remarks>-50~80</remarks>
        public float Ambient_Temp { get; set; }
    }
}