namespace SDKs.DjiImage.Thermals
{
    static class _sdkwin
    {
        const string dllName = "libdirp.dll";
        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_create_from_rjpeg(byte[] data, int size, ref System.IntPtr ph);
        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_get_rjpeg_resolution(System.IntPtr h, ref dirp_resolution_t resolution);
        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_get_rjpeg_version(System.IntPtr h, ref dirp_rjpeg_version_t rjpeg_version);
        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_get_original_raw(System.IntPtr h, byte[] raw_image, int size);
        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_process(System.IntPtr h, byte[] image_data, int size);
        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_measure(System.IntPtr h, byte[] temp_image, int size);
        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_get_measurement_params(System.IntPtr h, ref MeasureParam measurement_params);
        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_get_pseudo_color(System.IntPtr h, ref PseudoColor color);
        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_set_pseudo_color(System.IntPtr h, PseudoColor color);
        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_set_isotherm(System.IntPtr h, dirp_isotherm_t isotherm);
        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_get_enhancement_params(System.IntPtr h, ref dirp_enhancement_params_t enhancement_params_t);
        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_set_enhancement_params(System.IntPtr h,ref dirp_enhancement_params_t enhancement_params_t);
        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_destroy(System.IntPtr h);
    }
}