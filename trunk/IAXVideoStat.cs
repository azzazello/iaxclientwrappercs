namespace AsteriaSGI.IAXClientWrapper
{

    public struct IAXVideoStat
    {

        public ulong acc_recv_size;
        public ulong acc_sent_size;
        public float avg_inbound_bps;
        public float avg_inbound_fps;
        public float avg_outbound_bps;
        public float avg_outbound_fps;
        public ulong dropped_frames;
        public ulong inbound_frames;
        public ulong outbound_frames;
        public ulong received_slices;
        public ulong sent_slices;
        public AsteriaSGI.IAXClientWrapper.IAXTimeval start_time;

    }

}

