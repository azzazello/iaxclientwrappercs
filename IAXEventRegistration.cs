namespace AsteriaSGI.IAXClientWrapper
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public struct IAXEventRegistration
    {

        private IntPtr nextEvent;

        public EventTypes eventtype;

        public Int32 id;

        public RegistrationReplyTypes reply;

        public Int32 msgcount;
    }

}

