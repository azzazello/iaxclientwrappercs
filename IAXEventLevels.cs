namespace AsteriaSGI.IAXClientWrapper
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public struct IAXEventLevels
    {

        private IntPtr nextEvent;

        public EventTypes eventtype;

        public float input;

        public float output;
    }

}

