namespace AsteriaSGI.IAXClientWrapper
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public struct IAXEventCallState
    {

        private IntPtr nextEvent;

        public EventTypes eventtype;

        public Int32 callno;

        public CallStates state;

        public MediaFormats format;

        public MediaFormats vformat;

        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string remote;

        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string remote_name;

        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string local;

        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string local_context;
    }

}

