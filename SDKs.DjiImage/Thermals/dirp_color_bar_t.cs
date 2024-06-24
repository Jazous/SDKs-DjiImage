using System.Runtime.InteropServices;

namespace SDKs.DjiImage.Thermals
{
    [StructLayout(LayoutKind.Sequential)]
    struct dirp_color_bar_t
    {
        /// <summary>
        /// Color bar mode, manual(true) or automatic(false).
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public bool manual_enable;
        /// <summary>
        /// Upper limit. Only effective when color bar is in manual mode.
        /// </summary>
        public float high;
        /// <summary>
        /// Lower limit. Only effective when color bar is in manual mode.
        /// </summary>
        public float low;
    }
}
