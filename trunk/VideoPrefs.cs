namespace AsteriaSGI.IAXClientWrapper
{

    public enum VideoPrefs
    {
        IAXC_VIDEO_PREF_RECV_LOCAL_RAW = 1,
        IAXC_VIDEO_PREF_RECV_LOCAL_ENCODED = 2,
        IAXC_VIDEO_PREF_RECV_REMOTE_RAW = 4,
        IAXC_VIDEO_PREF_RECV_REMOTE_ENCODED = 8,
        IAXC_VIDEO_PREF_SEND_DISABLE = 16,
        IAXC_VIDEO_PREF_RECV_RGB32 = 32,
        IAXC_VIDEO_PREF_CAPTURE_DISABLE = 64
    }

}

