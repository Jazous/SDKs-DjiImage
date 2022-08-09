namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// 位置温度
    /// </summary>
    public struct LTEntry : IEquatable<LTEntry>
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

        /// <summary>
        /// 比较位置和温度是否相同
        /// </summary>
        /// <param name="other">比较的对象</param>
        /// <returns></returns>
        public bool Equals(LTEntry other)
        {
            return this.Left == other.Left && this.Top == other.Top && this.Temp == other.Temp;
        }
    }
}