using System.Xml.Serialization;

namespace SDKs.DjiImage
{
    // <summary>
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
        [XmlAttribute("drone-dji:Version")]
        public string Version { get; set; }
        /// <summary>
        /// Gps 状态。Normal、RTK
        /// </summary>
        [XmlAttribute("drone-dji:GpsStatus")]
        public string GpsStatus { get; set; }
        /// <summary>
        /// 相机位置的纬度。北纬为正，南纬为负
        /// </summary>
        [XmlAttribute("drone-dji:GpsLatitude")]
        public decimal GpsLatitude { get; set; }
        /// <summary>
        /// 相机位置的经度。东经为正，西经为负
        /// </summary>
        [XmlAttribute("drone-dji:GpsLongitude")]
        public decimal GpsLongitude { get; set; }
        /// <summary>
        /// 绝对高度。
        /// </summary>
        [XmlAttribute("drone-dji:AbsoluteAltitude")]
        public decimal AbsoluteAltitude { get; set; }
        /// <summary>
        /// 相对高度。相对于起飞点
        /// </summary>
        [XmlAttribute("drone-dji:RelativeAltitude")]
        public decimal RelativeAltitude { get; set; }
        /// <summary>
        /// 云台翻滚角
        /// </summary>
        [XmlAttribute("drone-dji:GimbalRollDegree")]
        public decimal GimbalRollDegree { get; set; }
        /// <summary>
        /// 云台偏航角
        /// </summary>
        [XmlAttribute("drone-dji:GimbalYawDegree")]
        public decimal GimbalYawDegree { get; set; }
        /// <summary>
        /// 云台俯仰角
        /// </summary>
        [XmlAttribute("drone-dji:GimbalPitchDegree")]
        public decimal GimbalPitchDegree { get; set; }
        /// <summary>
        /// 无人机翻滚角
        /// </summary>
        [XmlAttribute("drone-dji:FlightRollDegree")]
        public decimal FlightRollDegree { get; set; }
        /// <summary>
        /// 无人机偏航角
        /// </summary>
        [XmlAttribute("drone-dji:FlightYawDegree")]
        public decimal FlightYawDegree { get; set; }
        /// <summary>
        /// 无人机俯仰角
        /// </summary>
        [XmlAttribute("drone-dji:FlightPitchDegree")]
        public decimal FlightPitchDegree { get; set; }
        /// <summary>
        /// RTK 状态
        /// </summary>
        /// <remarks>
        /// 0-无定位； 16-单点定位模式； 34-RTK浮点解； 50-RTK固定解
        /// 当不为 50 时，不推荐使用次照片进行建图
        /// </remarks>
        [XmlAttribute("drone-dji:RtkFlag")]
        public int RtkFlag { get; set; }
    }
}