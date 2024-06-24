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
        public static int dirp_get_rjpeg_version(System.IntPtr h, ref dirp_rjpeg_version_t rjpeg_version)
        {
            return isLinux ? _sdklinux.dirp_get_rjpeg_version(h, ref rjpeg_version) : _sdkwin.dirp_get_rjpeg_version(h, ref rjpeg_version);
        }
        public static int dirp_get_original_raw(System.IntPtr h, byte[] raw_image, int size)
        {
            return isLinux ? _sdklinux.dirp_get_original_raw(h, raw_image, size) : _sdkwin.dirp_get_original_raw(h, raw_image, size);
        }
        public static int dirp_process(System.IntPtr h, byte[] image_data, int size)
        {
            return isLinux ? _sdklinux.dirp_process(h, image_data, size) : _sdkwin.dirp_process(h, image_data, size);
        }
        public static int dirp_measure(System.IntPtr h, byte[] temp_image, int size)
        {
            return isLinux ? _sdklinux.dirp_measure(h, temp_image, size) : _sdkwin.dirp_measure(h, temp_image, size);
        }
        public static int dirp_get_measurement_params(System.IntPtr h, ref MeasureParam measurement_params)
        {
            return isLinux ? _sdklinux.dirp_get_measurement_params(h, ref measurement_params) : _sdkwin.dirp_get_measurement_params(h, ref measurement_params);
        }
        public static int dirp_get_color_bar(System.IntPtr h, ref dirp_color_bar_t color_bar)
        {
            return isLinux ? _sdklinux.dirp_get_color_bar(h, ref color_bar) : _sdkwin.dirp_get_color_bar(h, ref color_bar);
        }
        public static int dirp_get_color_bar_adaptive_params(System.IntPtr h, ref dirp_color_bar_t color_bar)
        {
            return isLinux ? _sdklinux.dirp_get_color_bar_adaptive_params(h, ref color_bar) : _sdkwin.dirp_get_color_bar_adaptive_params(h, ref color_bar);
        }
        public static int dirp_set_color_bar(System.IntPtr h, ref dirp_color_bar_t color_bar)
        {
            return isLinux ? _sdklinux.dirp_set_color_bar(h, ref color_bar) : _sdkwin.dirp_set_color_bar(h, ref color_bar);
        }
        public static int dirp_get_pseudo_color(System.IntPtr h, ref PseudoColor color)
        {
            return isLinux ? _sdklinux.dirp_get_pseudo_color(h, ref color) : _sdkwin.dirp_get_pseudo_color(h, ref color);
        }
        public static int dirp_set_pseudo_color(System.IntPtr h, PseudoColor color)
        {
            return isLinux ? _sdklinux.dirp_set_pseudo_color(h, color) : _sdkwin.dirp_set_pseudo_color(h, color);
        }
        public static int dirp_set_isotherm(System.IntPtr h, dirp_isotherm_t isotherm)
        {
            return isLinux ? _sdklinux.dirp_set_isotherm(h, isotherm) : _sdkwin.dirp_set_isotherm(h, isotherm);
        }
        public static int dirp_get_enhancement_params(System.IntPtr h, ref dirp_enhancement_params_t enhancement_params_t)
        {
            return isLinux ? _sdklinux.dirp_get_enhancement_params(h, ref enhancement_params_t) : _sdkwin.dirp_get_enhancement_params(h, ref enhancement_params_t);
        }
        public static int dirp_set_enhancement_params(System.IntPtr h, ref dirp_enhancement_params_t enhancement_params_t)
        {
            return isLinux ? _sdklinux.dirp_set_enhancement_params(h, ref enhancement_params_t) : _sdkwin.dirp_set_enhancement_params(h, ref enhancement_params_t);
        }
        public static int dirp_destroy(System.IntPtr h)
        {
            return isLinux ? _sdklinux.dirp_destroy(h) : _sdkwin.dirp_destroy(h);
        }
    }
}