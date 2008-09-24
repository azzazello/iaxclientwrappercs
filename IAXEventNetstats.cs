namespace AsteriaSGI.IAXClientWrapper
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public struct IAXEventNetstats
    {

        private IntPtr nextEvent;

        internal EventTypes eventtype;

        public Int32 callno;

        public Int32 rtt;

        public IAXNetstat local;

        public IAXNetstat remote;
    }

}

