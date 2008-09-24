namespace AsteriaSGI.IAXClientWrapper
{

    public enum CallStates
    {
        IAXC_CALL_STATE_FREE = 0,
        IAXC_CALL_STATE_ACTIVE = 2,
        IAXC_CALL_STATE_OUTGOING = 4,
        IAXC_CALL_STATE_RINGING = 8,
        IAXC_CALL_STATE_COMPLETE = 16,
        IAXC_CALL_STATE_SELECTED = 32,
        IAXC_CALL_STATE_BUSY = 64,
        IAXC_CALL_STATE_TRANSFER = 128
    }

}

