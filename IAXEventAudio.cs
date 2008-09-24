namespace AsteriaSGI.IAXClientWrapper
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public struct IAXEventAudio
    {

        private IntPtr nextEvent;

        public EventTypes eventtype;

        public Int32 callno;

        public UInt32 ts;

        public MediaFormats format;

        public Int32 encoded;

        public Int32 source;

        public Int32 size;

        public IntPtr datapointer;
    }

}

