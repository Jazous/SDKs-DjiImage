namespace SDKs.DjiImage
{
    /// <summary>
    /// 位置信息
    /// </summary>
    public struct Location : System.IEquatable<Location>
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
        /// 创建位置对象
        /// </summary>
        /// <param name="left">距离左上角水平方向的索引位置</param>
        /// <param name="top">距离左上角垂直方向的索引位置</param>
        public Location(int left, int top)
        {
            Left = left;
            Top = top;
        }
        /// <summary>
        /// 与指定位置是否相同
        /// </summary>
        /// <param name="other">比较的位置</param>
        /// <returns></returns>
        public bool Equals(Location other)
        {
            return Left == other.Left && Top == other.Top;
        }
    }
}