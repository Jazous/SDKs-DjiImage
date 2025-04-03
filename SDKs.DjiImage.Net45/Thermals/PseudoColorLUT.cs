using System.Runtime.InteropServices;

namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// 8 bit伪彩色 LUT 结构
    /// </summary>
    public struct PseudoColorLUT
    {
        /// <summary>
        /// 红色 LUT，0~255
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2560, ArraySubType = UnmanagedType.U1)]
        public byte[] Red;
        /// <summary>
        /// 绿色 LUT，0~255
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2560, ArraySubType = UnmanagedType.U1)]
        public byte[] Green;
        /// <summary>
        /// 蓝色 LUT，0~255
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2560, ArraySubType = UnmanagedType.U1)]
        public byte[] Blue;

        /// <summary>
        /// 获取指定伪彩色映射的 Red 颜色值
        /// </summary>
        /// <param name="color"></param>
        /// <param name="value">0~255</param>
        /// <returns></returns>
        public byte GetRed(PseudoColor color, byte value)
        {
            return Red[(byte)color * 256 + value];
        }
        /// <summary>
        /// 获取指定伪彩色映射的 Green 颜色值
        /// </summary>
        /// <param name="color"></param>
        /// <param name="value">0~255</param>
        /// <returns></returns>

        public byte GetGreen(PseudoColor color, int value)
        {
            return Green[(byte)color * 256 + value];
        }
        /// <summary>
        /// 获取指定伪彩色映射的 Blue 颜色值
        /// </summary>
        /// <param name="color"></param>
        /// <param name="value">0~255</param>
        /// <returns></returns>

        public byte GetBlue(PseudoColor color, int value)
        {
            return Blue[(byte)color * 256 + value];
        }
    }
}