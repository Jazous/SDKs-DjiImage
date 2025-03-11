using System.ComponentModel;

namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// 伪彩色调色板枚举
    /// </summary>
    public enum PseudoColor : byte
    {
        /// <summary>
        /// White Hot
        /// </summary>
        [Description("白热")]
        DIRP_PSEUDO_COLOR_WHITEHOT = 0,
        /// <summary>
        /// Fulgurite
        /// </summary>
        [Description("熔岩")]
        DIRP_PSEUDO_COLOR_FULGURITE = 1,
        /// <summary>
        /// Iron Red
        /// </summary>
        [Description("铁红")]
        DIRP_PSEUDO_COLOR_IRONRED = 2,
        /// <summary>
        /// Hot Iron
        /// </summary>
        [Description("热铁")]
        DIRP_PSEUDO_COLOR_HOTIRON = 3,
        /// <summary>
        /// Medical
        /// </summary>
        [Description("医疗")]
        DIRP_PSEUDO_COLOR_MEDICAL = 4,
        /// <summary>
        /// Arctic
        /// </summary>
        [Description("北极")]
        DIRP_PSEUDO_COLOR_ARCTIC = 5,
        /// <summary>
        /// Rainbow 1
        /// </summary>
        [Description("彩虹1")]
        DIRP_PSEUDO_COLOR_RAINBOW1 = 6,
        /// <summary>
        /// Rainbow 2
        /// </summary>
        [Description("彩虹2")]
        DIRP_PSEUDO_COLOR_RAINBOW2 = 7,
        /// <summary>
        /// Tint
        /// </summary>
        [Description("描红")]
        DIRP_PSEUDO_COLOR_TINT = 8,
        /// <summary>
        /// Black Hot
        /// </summary>
        [Description("黑热")]
        DIRP_PSEUDO_COLOR_BLACKHOT = 9
    }
}