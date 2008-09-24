namespace AsteriaSGI.IAXClientWrapper
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public struct IAXEventVideoStats
    {

        private IntPtr nextEvent;

        public EventTypes eventtype;

        public Int32 callno;

        public IAXVideoStat stats;
    }

}

