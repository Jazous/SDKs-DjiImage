namespace SDKs.DjiImage.Thermals
{
    static class TSDK_Linux
    {
        const string dllName = "libdirp.so";

        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_create_from_rjpeg(byte[] data, int size, ref IntPtr ph);

        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_get_rjpeg_version(IntPtr h, ref dirp_rjpeg_version_t version);

        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_get_rjpeg_resolution(IntPtr h, ref dirp_resolution_t resolution);

        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_get_original_raw(IntPtr h, byte[] raw_image, int size);

        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_measure(IntPtr h, byte[] temp_image, int size);

        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_get_measurement_params(IntPtr h, ref dirp_measurement_params_t measurement_params);

        [System.Runtime.InteropServices.DllImport(dllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public extern static int dirp_destroy(IntPtr h);
    }
}