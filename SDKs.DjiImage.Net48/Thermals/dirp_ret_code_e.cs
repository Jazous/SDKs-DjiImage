namespace SDKs.DjiImage.Thermals
{
    internal enum dirp_ret_code_e
    {
        DIRP_SUCCESS = 0,      /**<   0: Success (no error) */
        DIRP_ERROR_MALLOC = -1,      /**<  -1: Malloc error */
        DIRP_ERROR_POINTER_NULL = -2,      /**<  -2: NULL pointer input */
        DIRP_ERROR_INVALID_PARAMS = -3,      /**<  -3: Invalid parameters input */
        DIRP_ERROR_INVALID_RAW = -4,      /**<  -4: Invalid RAW in R-JPEG */
        DIRP_ERROR_INVALID_HEADER = -5,      /**<  -5: Invalid header in R-JPEG */
        DIRP_ERROR_INVALID_CURVE = -6,      /**<  -6: Invalid curve LUT in R-JPEG */
        DIRP_ERROR_RJPEG_PARSE = -7,      /**<  -7: Parse error for R-JPEG data */
        DIRP_ERROR_SIZE = -8,      /**<  -8: Wrong size input */
        DIRP_ERROR_INVALID_HANDLE = -9,      /**<  -9: Invalid handle input */
        DIRP_ERROR_FORMAT_INPUT = -10,      /**< -10: Wrong input image format */
        DIRP_ERROR_FORMAT_OUTPUT = -11,      /**< -11: Wrong output image format */
        DIRP_ERROR_UNSUPPORTED_FUNC = -12,      /**< -12: Unsupported function called */
        DIRP_ERROR_NOT_READY = -13,      /**< -13: Some preliminary conditions not meet */
        DIRP_ERROR_ACTIVATION = -14,      /**< -14: SDK activate failed */
        DIRP_ERROR_INVALID_INI = -15,      /**< -15: Invalid ini file */
        DIRP_ERROR_INVALID_SUB_DLL = -16,      /**< -16: Invalid sub DLL */
        DIRP_ERROR_ADVANCED = -32,      /**< -32: Advanced error codes which may be smaller than this value */
    }
}