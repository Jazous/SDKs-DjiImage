﻿using System.Runtime.InteropServices;

namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// Isotherm parameters structure definition.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct dirp_isotherm_t
    {
        /// <summary>
        /// Isotherm switch, enable(true) or disable(false).
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public bool enable;
        /// <summary>
        /// Upper limit. Only effective when isotherm is enabled.
        /// </summary>
        public float high;
        /// <summary>
        /// Lower limit. Only effective when isotherm is enabled.
        /// </summary>
        public float low;
    }
}