namespace AsteriaSGI.IAXClientWrapper
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public struct IAXEventText
    {

        private IntPtr nextEvent;

        public EventTypes eventtype;

        public TextEventTypes type;

        public Int32 callno;

        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string text;
    }

}

