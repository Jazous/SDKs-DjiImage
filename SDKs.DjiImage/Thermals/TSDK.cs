namespace SDKs.DjiImage.Thermals
{
    static class TSDK
    {
        static readonly bool isLinux;
        static TSDK()
        {
            isLinux = OperatingSystem.IsLinux();
        }

        public static int dirp_create_from_rjpeg(byte[] data, int size, ref IntPtr ph)
        {
            return isLinux ? TSDK_Linux.dirp_create_from_rjpeg(data, size, ref ph) : TSDK_Windows.dirp_create_from_rjpeg(data, size, ref ph);
        }

        public static int dirp_get_rjpeg_version(IntPtr h, ref dirp_rjpeg_version_t version)
        {
            return isLinux ? TSDK_Linux.dirp_get_rjpeg_version(h, ref version) : TSDK_Windows.dirp_get_rjpeg_version(h, ref version);
        }

        public static int dirp_get_rjpeg_resolution(IntPtr h, ref dirp_resolution_t resolution)
        {
            return isLinux ? TSDK_Linux.dirp_get_rjpeg_resolution(h, ref resolution) : TSDK_Windows.dirp_get_rjpeg_resolution(h, ref resolution);
        }

        public static int dirp_get_original_raw(IntPtr h, byte[] raw_image, int size)
        {
            return isLinux ? TSDK_Linux.dirp_get_original_raw(h, raw_image, size) : TSDK_Windows.dirp_get_original_raw(h, raw_image, size);
        }

        public static int dirp_measure_ex(IntPtr h, byte[] temp_image, int size)
        {
            return isLinux ? TSDK_Linux.dirp_measure_ex(h, temp_image, size) : TSDK_Windows.dirp_measure_ex(h, temp_image, size);
        }

        public static int dirp_get_measurement_params(IntPtr h, ref dirp_measurement_params_t measurement_params)
        {
            return isLinux ? TSDK_Linux.dirp_get_measurement_params(h, ref measurement_params) : TSDK_Windows.dirp_get_measurement_params(h, ref measurement_params);
        }

        public static int dirp_destroy(IntPtr h)
        {
            return isLinux ? TSDK_Linux.dirp_destroy(h) : TSDK_Windows.dirp_destroy(h);
        }
    }
}