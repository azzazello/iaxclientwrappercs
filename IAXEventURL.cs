namespace AsteriaSGI.IAXClientWrapper
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public struct IAXEventURL
    {

        private IntPtr nextEvent;

        public EventTypes eventtype;

        public Int32 callno;

        public URLReplyTypes urltype;

        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string url;
    }

}

