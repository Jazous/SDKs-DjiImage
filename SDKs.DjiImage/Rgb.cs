namespace SDKs.DjiImage
{
    /// <summary>
    /// RGB 颜色
    /// </summary>
    public struct Rgb : IEquatable<Rgb>
    {
        /// <summary>
        /// 黑色
        /// </summary>
        public static readonly Rgb Black = new Rgb(0, 0, 0);
        /// <summary>
        /// 白色
        /// </summary>
        public static readonly Rgb White = new Rgb(255, 255, 255);
        /// <summary>
        /// 红色
        /// </summary>
        public static readonly Rgb Red = new Rgb(255, 0, 0);
        /// <summary>
        /// 绿色
        /// </summary>
        public static readonly Rgb Green = new Rgb(0, 255, 0);
        /// <summary>
        /// 蓝色
        /// </summary>
        public static readonly Rgb Blue = new Rgb(0, 0, 255);

        /// <summary>
        /// 红色通道
        /// </summary>
        public byte R { get; private set; }
        /// <summary>
        /// 绿色通道
        /// </summary>
        public byte G { get; private set; }
        /// <summary>
        /// 蓝色通道
        /// </summary>
        public byte B { get; private set; }

        /// <summary>
        /// 创建对象新实例。
        /// </summary>
        /// <param name="r">red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        public Rgb(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// 判断颜色是否与指定相同。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Rgb other)
        {
            return this.R == other.R && this.G == other.G && this.B == other.B;
        }
    }
}