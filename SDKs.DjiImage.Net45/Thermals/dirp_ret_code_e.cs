namespace SDKs.DjiImage.Thermals
{
    internal enum dirp_ret_code_e
    {
        DIRP_SUCCESS = 0,
        DIRP_ERROR_MALLOC = -1,
        DIRP_ERROR_POINTER_NULL = -2,
        DIRP_ERROR_INVALID_PARAMS = -3,
        DIRP_ERROR_INVALID_RAW = -4,
        DIRP_ERROR_INVALID_HEADER = -5,
        DIRP_ERROR_INVALID_CURVE = -6,
        DIRP_ERROR_RJPEG_PARSE = -7,
        DIRP_ERROR_SIZE = -8,
        DIRP_ERROR_INVALID_HANDLE = -9,
        DIRP_ERROR_FORMAT_INPUT = -10,
        DIRP_ERROR_FORMAT_OUTPUT = -11,
        DIRP_ERROR_UNSUPPORTED_FUNC = -12,
        DIRP_ERROR_NOT_READY = -13,
        DIRP_ERROR_ACTIVATION = -14,
        DIRP_ERROR_INVALID_INI = -15,
        DIRP_ERROR_INVALID_SUB_DLL = -16,
        DIRP_ERROR_ADVANCED = -32
    }
}