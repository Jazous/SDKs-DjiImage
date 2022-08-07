namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// 位置温度
    /// </summary>
    public struct RTEntry
    {
        /// <summary>
        /// 距离左上角水平方向的索引位置
        /// </summary>
        public int Left;
        /// <summary>
        /// 距离左上角垂直方向的索引位置
        /// </summary>
        public int Top;
        /// <summary>
        /// 温度
        /// </summary>
        public float Temp;
    }
}