using System;

namespace SDKs.DjiImage
{
    /// <summary>
    /// 大疆 XMP Meta drone-dji 信息
    /// </summary>
    /// <remarks>经纬度坐标为 WGS-84 标准</remarks>
    public struct RdfDroneDji
    {
        /// <summary>
        /// 表示没有 drone-dji 信息
        /// </summary>
        public static readonly RdfDroneDji Empty = new RdfDroneDji();

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 相机型号
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 照片相机来源
        /// </summary>
        /// <remarks>如：InfraredCamera</remarks>
        public string ImageSource { get; set; }
        /// <summary>
        /// Gps 状态
        /// </summary>
        /// <remarks>如：Normal，RTK</remarks>
        public string GpsStatus { get; set; }
        /// <summary>
        /// 相机位置的纬度
        /// </summary>
        /// <remarks>北纬为正，南纬为负</remarks>
        public decimal GpsLatitude { get; set; }
        /// <summary>
        /// 相机位置的经度
        /// </summary>
        /// <remarks>东经为正，西经为负</remarks>
        public decimal GpsLongitude { get; set; }
        /// <summary>
        /// 绝对高度
        /// </summary>
        public decimal AbsoluteAltitude { get; set; }
        /// <summary>
        /// 相对高度
        /// </summary>
        public decimal RelativeAltitude { get; set; }
        /// <summary>
        /// 云台翻滚角
        /// </summary>
        public decimal GimbalRollDegree { get; set; }
        /// <summary>
        /// 云台偏航角
        /// </summary>
        public decimal GimbalYawDegree { get; set; }
        /// <summary>
        /// 云台俯仰角
        /// </summary>
        public decimal GimbalPitchDegree { get; set; }
        /// <summary>
        /// 无人机翻滚角
        /// </summary>
        public decimal FlightRollDegree { get; set; }
        /// <summary>
        /// 无人机偏航角
        /// </summary>
        public decimal FlightYawDegree { get; set; }
        /// <summary>
        /// 无人机俯仰角
        /// </summary>
        public decimal FlightPitchDegree { get; set; }
        /// <summary>
        /// RTK 状态
        /// </summary>
        /// <remarks>
        /// 0-无定位， 16-单点定位模式， 34-RTK浮点解， 50-RTK固定解。<br/>
        /// 当不为 50 时，不推荐使用次照片进行建图。
        /// </remarks>
        public int? RtkFlag { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyDate { get; set; }
    }
}