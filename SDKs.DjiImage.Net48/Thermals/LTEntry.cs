using System.Diagnostics.CodeAnalysis;

namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// 位置温度
    /// </summary>
    public struct LTEntry : System.IEquatable<LTEntry>, System.IComparable<LTEntry>
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
        /// 创建对象实例
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="temp"></param>
        public LTEntry(int left, int top, float temp)
        {
            Left = left;
            Top = top;
            Temp = temp;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is LTEntry)
                return Equals((LTEntry)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// 是否位置和温度都相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(LTEntry other)
        {
            return this.Left == other.Left && this.Top == other.Top && this.Temp == other.Temp;
        }
        /// <summary>
        /// 返回 JSON 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "{\"Left\":" + this.Left + ",\"Top\":" + this.Top + ",\"Temp\":" + this.Temp + "}";
        }

        /// <summary>
        /// 按照温度从大到小、水平方向位置大小、垂直方向位置大小
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(LTEntry other)
        {
            int result = Temp.CompareTo(other.Temp);
            if (result == 0)
            {
                result = Left.CompareTo(other.Left);
                if (result == 0)
                    return Top.CompareTo(other.Top);
                return result;
            }
            return result;
        }
    }
}