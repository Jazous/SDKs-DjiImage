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
        [System.Text.Json.Serialization.JsonPropertyName("left")]
        public int Left;
        /// <summary>
        /// 距离左上角垂直方向的索引位置
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("top")]
        public int Top;
        /// <summary>
        /// 温度
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("temp")]
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
        /// <summary>
        /// 返回当前对象与指定对象的位置和温度是否都相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null)
                return false;
            if (obj is LTEntry)
                return Equals((LTEntry)obj);
            return false;
        }

        /// <summary>
        /// 获取当前对象的哈希代码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// 返回当前对象与指定对象的位置和温度是否都相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(LTEntry other)
        {
            return this.Left == other.Left && this.Top == other.Top && this.Temp == other.Temp;
        }
        /// <summary>
        /// 返回表示当前对象的 JSON 字符串
        /// </summary>
        /// <returns></returns>
        /// <remarks>如：{"left":100,"top":100,"temp":36.7}</remarks>
        public override string ToString()
        {
            return "{\"left\":" + this.Left + ",\"top\":" + this.Top + ",\"temp\":" + this.Temp + "}";
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