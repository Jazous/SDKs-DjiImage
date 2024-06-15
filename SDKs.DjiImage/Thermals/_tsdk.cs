namespace SDKs.DjiImage.Thermals
{
    static class _tsdk
    {
        static readonly bool isLinux;
        static _tsdk()
        {
            isLinux = OperatingSystem.IsLinux();
        }

        public static int dirp_create_from_rjpeg(byte[] data, int size, ref System.IntPtr ph)
        {
            return isLinux ? _sdklinux.dirp_create_from_rjpeg(data, size, ref ph) : _sdkwin.dirp_create_from_rjpeg(data, size, ref ph);
        }
        public static int dirp_get_rjpeg_resolution(System.IntPtr h, ref dirp_resolution_t resolution)
        {
            return isLinux ? _sdklinux.dirp_get_rjpeg_resolution(h, ref resolution) : _sdkwin.dirp_get_rjpeg_resolution(h, ref resolution);
        }
        public static int dirp_get_original(System.IntPtr h, byte[] raw_image, int size)
        {
            return isLinux ? _sdklinux.dirp_get_original_raw(h, raw_image, size) : _sdkwin.dirp_get_original_raw(h, raw_image, size);
        }
        public static int dirp_measure(System.IntPtr h, byte[] temp_image, int size)
        {
            return isLinux ? _sdklinux.dirp_measure(h, temp_image, size) : _sdkwin.dirp_measure(h, temp_image, size);
        }
        public static int dirp_get_measurement_params(System.IntPtr h, ref MeasureParam measurement_params)
        {
            return isLinux ? _sdklinux.dirp_get_measurement_params(h, ref measurement_params) : _sdkwin.dirp_get_measurement_params(h, ref measurement_params);
        }
        public static int dirp_destroy(System.IntPtr h)
        {
            return isLinux ? _sdklinux.dirp_destroy(h) : _sdkwin.dirp_destroy(h);
        }
    }
}