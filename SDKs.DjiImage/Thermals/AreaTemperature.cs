namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// 区域温度
    /// </summary>
    public struct AreaTemperature : IAreaTemperature
    {
        /// <summary>
        /// 未知区域温度
        /// </summary>
        public static readonly AreaTemperature Empty = new AreaTemperature(float.NaN, float.NaN, float.NaN);
        /// <summary>
        /// 最低温度
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("minTemp")]
        public float MinTemp { get; set; }
        /// <summary>
        /// 最高温度
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("maxTemp")]
        public float MaxTemp { get; set; }
        /// <summary>
        /// 平均温度
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("avgTemp")]
        public float AvgTemp { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="avg"></param>
        public AreaTemperature(float min, float max, float avg)
        {
            MinTemp = min;
            MaxTemp = max;
            AvgTemp = avg;
        }
        /// <summary>
        /// 返回表示当前对象的 JSON 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "{\"minTemp\":" + this.MinTemp + ",\"maxTemp\":" + this.MaxTemp + ",\"avgTemp\":" + this.AvgTemp + "}";
        }
    }
}