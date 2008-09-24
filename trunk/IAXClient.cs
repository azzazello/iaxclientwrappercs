// ------------------------------------------------------------------------------
// IAXClient C# Wrapper by Chris Howard @ Asteria Solutions Group
// chris@asteriasgi.com  http://www.asteriasgi.com/IAXClientWrapperCS
// based on the Visual Basic Wrapper Developed by Andrew Pollack 
// @ Second Signal. 
// http://www.secondsignal.com/secondsignal/sshome.nsf/html/2ndSignal-IAXClientWrapper2005
// Version 0.6.5 r11619
// ------------------------------------------------------------------------------

namespace AsteriaSGI.IAXClientWrapper
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;
    using Microsoft.VisualBasic;
    using AsteriaSGI.IAXClientWrapper;

    public class IAXClient
    {
        private static object syncroot;
        private static GCHandle delegateHandle;
        private static GCHandle originalHandle;
        private static NativeWindow nwin;
        private static int prevprocptr;
        private static int newprocptr;
        private static int myeventid;
        private static IAXClient.SubClassProcDelegate thisdelegate;
        private static float CurrentInputLevel;
        private static float CurrentOutputLevel;
        private static bool clientInitialized;
        private static bool eventHandleCaptured;
        private static Hashtable reglist;
        private static Hashtable ptrlist;
        private static IAXClient.callInfo[] calls;
        private static IntPtr arrayAddrPtr;
        private static IntPtr devnbrptr;
        private static IntPtr inputnbr;
        private static IntPtr outputnbr;
        private static IntPtr ringnbr;
        private static string dllpath;
        internal const int IAXC_EVENT_BUFSIZ = 256;

        //Event Handler Delegates
        public event IAXAudioEventHandler IAXAudioEvent;
        public delegate void IAXAudioEventHandler(IAXEventAudio theIAXRAudio);

        public event IAXCallStateEventHandler IAXCallStateEvent;
        public delegate void IAXCallStateEventHandler(IAXEventCallState theIAXCallState);
        
        public event IAXLevelsEventHandler IAXLevelsEvent;
        public delegate void IAXLevelsEventHandler(IAXEventLevels theIAXLevels);
        
        public event IAXNetstatsEventHandler IAXNetstatsEvent;
        public delegate void IAXNetstatsEventHandler(IAXEventNetstats theIAXNetstats);
        
        public event IAXRegisterEventHandler IAXRegisterEvent;
        public delegate void IAXRegisterEventHandler(IAXEventRegistration theIAXRegistration);
        
        public event IAXTextEventHandler IAXTextEvent;
        public delegate void IAXTextEventHandler(IAXEventText theIAXText);
        
        public event IAXURLEventHandler IAXURLEvent;
        public delegate void IAXURLEventHandler(IAXEventURL theIAXURL);
        
        public event IAXVideoEventHandler IAXVideoEvent;
        public delegate void IAXVideoEventHandler(IAXEventVideo theIAXVideo);

        public event IAXVideoStatsEventHandler IAXVideoStatsEvent;
        public delegate void IAXVideoStatsEventHandler(IAXEventVideoStats theIAXVideoStats);


        static IAXClient()
        {
            IAXClient.syncroot = new object();
            IAXClient.clientInitialized = false;
            IAXClient.eventHandleCaptured = false;
            IAXClient.reglist = new Hashtable();
            IAXClient.ptrlist = new Hashtable();
            IAXClient.arrayAddrPtr = IntPtr.Zero;
            IAXClient.devnbrptr = IntPtr.Zero;
            IAXClient.inputnbr = IntPtr.Zero;
            IAXClient.outputnbr = IntPtr.Zero;
            IAXClient.ringnbr = IntPtr.Zero;
            IAXClient.dllpath = "";
        }
        public IAXClient()
        {
            try
            {
                string p1;
                int x;
                bool found = false;
                string[] p;
                bool badpath = false;
                p1 = (System.Environment.GetEnvironmentVariable("PATH") + (";" + System.Windows.Forms.Application.StartupPath));
                p = p1.Split(';');
                for (x = 0; (x <= p.GetUpperBound(0)); x++)
                {
                    if (!p[x].EndsWith("\\"))
                    {
                        p[x] = (p[x] + "\\");
                    }
                    p1 = (p[x] + "libiaxclient.dll");
                    try
                    {
                        if ((System.IO.File.Exists(p1)))
                        {
                            found = true;
                            dllpath = p1;
                            x = (p.GetUpperBound(0) + 1);
                        }
                    }
                    catch (Exception ex)
                    {
                        badpath = true;
                    }
                }
                p1 = "";
                if (badpath)
                {
                    p1 = (" There may be a problem with your system \'PATH\' environment variable."
                                + (Environment.NewLine + Environment.NewLine));
                }
                if (!found)
                {
                    MessageBox.Show(("The libiaxclient.dll was not found in the"
                                    + (Environment.NewLine + (" application startup directory or on the system"
                                    + (Environment.NewLine + (" executable path."
                                    + (Environment.NewLine
                                    + (Environment.NewLine + (" executable path."
                                    + (Environment.NewLine
                                    + (Environment.NewLine
                                    + (p1 + ("The Second Signal IAX Client Wrapper requires this file."
                                    + (Environment.NewLine + (" executable path."
                                    + (Environment.NewLine
                                    + (Environment.NewLine + ("LIBIAXCLIENT.DLL" + Environment.NewLine)))))))))))))))))
                                    + ":IAX Client Wrapper Error");

                }
            }
            catch (Exception ex)
            {
            }
        }

        public IAXClient.callInfo getCallInfo(int callNo)
        {
            IAXClient.callInfo callInfo1;
            lock (this)
            {
                if (!(!((!(callNo > IAXClient.calls.GetUpperBound(0))) & (!(callNo < IAXClient.calls.GetLowerBound(0))))))
                {
                    return IAXClient.calls[callNo];
                }
                callInfo1 = new IAXClient.callInfo();
            }
            return callInfo1;
        }
        public string getLibDllPath()
        {
            return dllpath;
        }
        public float getCurrentInputLevel()
        {
            return IAXClient.CurrentInputLevel;
        }
        public float getCurrentOutputLevel()
        {
            return CurrentOutputLevel; 
        }
        ~IAXClient()
        {
            try
            {
                IAXClient.iaxc_dump_all_calls();
            }
            catch
            {
            }
            try
            {
                IAXClient.iaxc_stop_processing_thread();
            }
            catch 
            {

            }
            try
            {
                IAXClient.iaxc_shutdown();
            }
            catch
            {
            }
            try
            {
                IAXClient.delegateHandle.Free();
            }
            catch 
            {
            }
            try
            {
                IAXClient.originalHandle.Free();
            }
            catch
            {
            }
            try
            {
                Marshal.FreeHGlobal(IAXClient.arrayAddrPtr);
            }
            catch 
            {

            }
            try
            {
                Marshal.FreeHGlobal(IAXClient.devnbrptr);
            }
            catch
            {
            }
            try
            {
                Marshal.FreeHGlobal(IAXClient.inputnbr);
            }
            catch 
            {
            }
            try
            {
                Marshal.FreeHGlobal(IAXClient.outputnbr);
            }
            catch
            {
            }
            try
            {
                Marshal.FreeHGlobal(IAXClient.ringnbr);
            }
            catch 
            {

            }
            
        }
        public bool initialize(int MaxConcurrentCalls)
        {
            
            int n;
            try
            {
                if (!clientInitialized)
                {

                    if (dllpath == "")
                    {
                        MessageBox.Show("Cannot initialize IAX Client. The libiaxclient.dll was not found. - IAX Client Wrapper Error");
                        return false;
                    }

                    arrayAddrPtr = Marshal.AllocHGlobal(4);
                    devnbrptr = Marshal.AllocHGlobal(4);
                    inputnbr = Marshal.AllocHGlobal(4);
                    outputnbr = Marshal.AllocHGlobal(4);
                    ringnbr = Marshal.AllocHGlobal(4);

                    // initialize the call holders 
                    callInfo[] localcalls = new callInfo[MaxConcurrentCalls];
                    for (n = 0; n <= localcalls.GetUpperBound(0); n++)
                    {
                        localcalls[n] = new callInfo();
                    }
                    calls = localcalls;
                    // make sure we have captured the handle first 
                    if (this.captureEventHandle())
                    {
                        // initialize with the given parameters 
                        n = iaxc_initialize(MaxConcurrentCalls);
                        if (!(n == 0))
                        {
                            System.Console.WriteLine("Failed to intitialize IAX Client: " + n.ToString());
                            return false;
                        }
                        // set up the event processing 
                        n = iaxc_set_event_callpost(newprocptr, myeventid);
                        if (!(n == 0))
                        {
                            System.Console.WriteLine("Failed to setup event post routine: " + n.ToString());
                            return false;
                        }
                        // start the processing thread 
                        n = iaxc_start_processing_thread();
                        if (!(n == 0))
                        {
                            System.Console.WriteLine("Failed to start the processing thread: " + n.ToString());
                            return false;
                        }
                        clientInitialized = true;
                        return true;
                    }
                    else
                    {
                        System.Console.WriteLine("Failed to capture events. Exiting.");
                        return false;
                    }
                }
                else
                {
                    System.Console.WriteLine("Client is already initialized");
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error trapped in iaxClientWrapper.initialize");
                System.Console.WriteLine(ex.StackTrace);
                return clientInitialized;
            } 
        }
        public bool set_preferred_source_udp_port(int udpPort)
        {
            bool retval=false;
            try
            {
               lock (syncroot)
                {
                    if (!IAXClient.clientInitialized)
                    {
                        IAXClient.iaxc_set_preferred_source_udp_port(udpPort);
                        retval =  true;
                    }
                }
            }
            catch
            {
            }
            return retval;
        }
        public int get_bind_port()
        {
            try
            {
                lock (syncroot)
                {
                    if (clientInitialized)
                    {
                        return iaxc_get_bind_port();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error in function call: " + ex.Message + " -- " + ex.StackTrace);
                return -1;
            }
            return - 1;
        }
        public bool set_formats(MediaFormats preferred, MediaFormats allow)
        {
            bool retval = false;
            try
            {
                lock (syncroot)
                {
                    if (IAXClient.clientInitialized)
                    {
                        IAXClient.iaxc_set_formats(preferred, allow);
                        retval = true;
                    }
                }
            }
            catch
            {
            }
            return retval;
        }
        private bool set_min_outgoing_framesize(int samples)
        {
            try
            {
                lock (syncroot)
                {
                    if (clientInitialized)
                    {
                        iaxc_set_min_outgoing_framesize(samples);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error in function call: " + ex.Message + " -- " + ex.StackTrace);
            }
            return false;
            
        }
        public int selected_call()
        {
            int retval = -1;
            try
            {
                lock (syncroot)
                {
                    if (IAXClient.clientInitialized)
                    {
                        retval = IAXClient.iaxc_selected_call();
                    }
                }
            }
            catch
            {
            }
            return retval;
        }
        public bool set_callerid(string name, string number)
        {

            try
            {
                lock (syncroot)
                {
                    if (clientInitialized)
                    {
                        int n = iaxc_selected_call();
                        if (n >= 0)
                        {
                            if (!(calls[n].calleridname == IntPtr.Zero))
                                Marshal.ZeroFreeCoTaskMemAnsi(calls[n].calleridname);
                            if (!(calls[n].calleridnumber == IntPtr.Zero))
                                Marshal.ZeroFreeCoTaskMemAnsi(calls[n].calleridnumber);
                            calls[n].calleridname = Marshal.StringToCoTaskMemAnsi(name);
                            calls[n].calleridnumber = Marshal.StringToCoTaskMemAnsi(number);
                            iaxc_set_callerid(calls[n].calleridname.ToInt32(), calls[n].calleridnumber.ToInt32());
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
            }
            return false;
        }
        public int placeCall(string address)
        {
            int retval = -1;
            try
            {
                lock (syncroot)
                {
                    if (IAXClient.clientInitialized)
                    {
                        int i2 = 0;
                        int callno = IAXClient.iaxc_first_free_call();
                        if (callno < 0)
                        {
                            System.Console.WriteLine("error Selecting call no:" + callno.ToString());
                        }
                        else
                        {
                            i2 = IAXClient.iaxc_select_call(callno);
                            if (i2 < 0)
                            {
                                System.Console.WriteLine("error Selecting call no:" + callno.ToString());
                            }
                            else
                            {
                                IAXClient.calls[callno].setnumber(address);
                                IAXClient.iaxc_call(IAXClient.calls[callno].numberptr.ToInt32());
                                i2 = IAXClient.iaxc_select_call(callno);
                                retval = callno;
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
            return retval;
        }
        public bool unregister(int regid)
        {
            
            int n;
            try
            {
                lock (syncroot)
                {
                    if (clientInitialized)
                    {
                        n = iaxc_unregister(regid);
                        if (!(n > 0))
                        {
                            System.Console.WriteLine("Failed to unregister id: " + regid.ToString() + " [" + n.ToString() + "]");
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }
        public int register(string username, string password, string hostaddr)
        {
            int i;
            IAXClient.regUserInfo regUserInfo;
            string str;
            try
            {
                lock (syncroot)
                {
                    if (IAXClient.clientInitialized)
                    {
                        regUserInfo = null;
                        str = string.Concat(username, "::", hostaddr);
                        if (IAXClient.reglist.ContainsKey(str))
                        {
                            regUserInfo = IAXClient.reglist[str] as IAXClient.regUserInfo;
                            unregister(regUserInfo.regid);
                        }
                        if (regUserInfo == null)
                        {
                            regUserInfo = new IAXClient.regUserInfo(ref username, ref password, ref hostaddr);
                            IAXClient.reglist.Add(str, regUserInfo);
                        }
                        int i1 = IAXClient.iaxc_register(regUserInfo.usernameptr.ToInt32(), regUserInfo.passwordptr.ToInt32(), regUserInfo.serveraddrptr.ToInt32());
                        regUserInfo.regid = i1;
                        if (IAXClient.reglist.ContainsKey(regUserInfo.regid.ToString()))
                        {
                            IAXClient.reglist.Remove(regUserInfo.regid.ToString());
                        }
                        IAXClient.reglist.Add(regUserInfo.regid.ToString(), regUserInfo);
                        return i1;
                    }
                    i = -1;
                }
            }
            catch
            {
                i = -1;
            }
            return i;
        }
        public string getIaxcVersion()
        {
            try
            {
                lock (syncroot)
                {
                    if (clientInitialized)
                    {
                        IntPtr strptr = Marshal.AllocHGlobal(256);
                        string version;
                        iaxc_version(strptr.ToInt32());
                        version = string.Copy(Marshal.PtrToStringAnsi(strptr));
                        Marshal.FreeHGlobal(strptr);
                        return version;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error trapped in iaxClientWrapper.version");
                System.Console.WriteLine(ex.StackTrace);
            }
            return ""; 
        }
        public bool sendDTMF(char digit)
        {
            bool bl;
            try
            {
                if (IAXClient.clientInitialized)
                {
                    lock (syncroot)
                    {
                        if (IAXClient.iaxc_selected_call() >= 0)
                        {
                            IAXClient.iaxc_send_dtmf(digit);
                            return true;
                        }
                        bl = false;
                    }
                    return bl;
                }
                bl = false;
            }
            catch
            {
                bl = false;
            }
            return bl;
        }
        public bool sendText(string text)
        {
            //TODO: Implement method
            return false;
        }
        private int stopSound(int soundid)
        {
            int i=-1;
            try
            {
                if (IAXClient.clientInitialized)
                {
                    lock (syncroot)
                    {
                        if (IAXClient.iaxc_selected_call() >= 0)
                        {
                            if (soundid > -1)
                            {
                                if (IAXClient.ptrlist.ContainsKey(string.Concat("soundid:", soundid.ToString())))
                                {
                                    IAXClient.ptrlist.Remove(string.Concat("soundid:", soundid.ToString()));
                                }
                                return IAXClient.iaxc_stop_sound(soundid);
                            }
                            return -1;
                        }
                        i = -1;
                    }
                }
            }
            catch
            {
                i = -1;
            }
            return i;
        }
        private int playSound(IAXSound soundDef, bool useRingOutputDevice)
        {
            //TODO: fix this 
            return -1;
        }
        public int selectCall(int number)
        {
            int retval = -1;

            if (number <= calls.GetUpperBound(0) & number >= calls.GetLowerBound(0))
            {
                try
                {
                    if (IAXClient.clientInitialized)
                    {
                        lock (syncroot)
                        {
                            Monitor.Enter(IAXClient.calls[number]);
                            Monitor.Exit(IAXClient.calls[number]);
                            retval = IAXClient.iaxc_select_call(number);
                        }
                        return retval;
                    }
                    return retval;
                }
                catch (Exception e)
                {
                    return retval;
                }
            }
            return retval;
            
        }
        public bool rejectCurrentCall()
        {
            try
            {
                lock (syncroot)
                {
                    if (clientInitialized)
                    {
                        CallStates n;
                        int number;
                        number = iaxc_selected_call();
                        if (number > -1)
                        {
                            n = calls[number].callstatus;
                            if (!((n & CallStates.IAXC_CALL_STATE_RINGING) > 0))
                            {
                                System.Console.WriteLine("That line isn't ringing");
                                return false;
                            }
                            else
                            {
                                iaxc_reject_call_number(number);
                                return true;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error trapped in iaxClientWrapper.rejectCall");
                System.Console.WriteLine(ex.StackTrace);
                return false;
            }
            return false;
        }
        public bool rejectCall(int number)
        {
            bool retval = false;
            if (number <= calls.GetUpperBound(0) & number >= calls.GetLowerBound(0))
            {
                try
                {
                    lock (syncroot)
                    {
                        if (IAXClient.clientInitialized)
                        {
                            if ((IAXClient.calls[number].callstatus & CallStates.IAXC_CALL_STATE_RINGING) <= 0)
                            {
                                return false;
                            }
                            IAXClient.iaxc_reject_call_number(number);
                            return true;
                        }
                        retval = false;
                    }
                }
                catch
                {
                    retval = false;
                }
            }
            return retval;
        }
        public bool dropCurrentCall()
        {
            try
            {
                CallStates theCallState;
                int number;
                lock (syncroot)
                {
                    if (clientInitialized)
                    {
                        number = iaxc_selected_call();
                        if (number > -1)
                        {
                            theCallState = calls[number].callstatus;
                            if (!((theCallState & CallStates.IAXC_CALL_STATE_ACTIVE) > 0))
                            {
                                System.Console.WriteLine("That line isn't active");
                                return false;
                            }
                            else
                            {
                                int m = iaxc_select_call(number);
                                if (m >= 0)
                                {
                                    iaxc_dump_call();
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error trapped in iaxClientWrapper.DropCall");
                System.Console.WriteLine(ex.StackTrace);
            }
            return false;
        }
        public bool dropCall(int number)
        {
            if (number <= calls.GetUpperBound(0) & number >= calls.GetLowerBound(0))
            {
                try
                {
                    lock (syncroot)
                    {
                        if (IAXClient.clientInitialized)
                        {
                            CallStates i = IAXClient.calls[number].callstatus;
                            if ((i & CallStates.IAXC_CALL_STATE_ACTIVE) <= 0)
                            {
                                return false;
                            }
                            int n = IAXClient.iaxc_select_call(number);
                            if (n >= 0)
                            {
                                IAXClient.iaxc_dump_call();
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
                catch
                {
                }
                return false;
            }
            return false;
        }
        public bool answerCall(int number)
        {
            if (number <= calls.GetUpperBound(0) & number >= calls.GetLowerBound(0))
            {
                try
                {
                    if (clientInitialized)
                    {
                        lock (syncroot)
                        {
                            CallStates theCallState;
                            theCallState = calls[number].callstatus;
                            if (!((theCallState & CallStates.IAXC_CALL_STATE_RINGING) > 0))
                            {
                                System.Console.WriteLine("That line isn't ringing");
                                return false;
                            }
                            else
                            {
                                iaxc_answer_call(number);
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Error trapped in iaxClientWrapper.AnswerCall");
                    System.Console.WriteLine(ex.StackTrace);
                    return false;
                }
            }
            return false;
        }

        public bool send_busy_on_incoming_call(int number)
        {
            bool retval=false;

            if (number <= calls.GetUpperBound(0) & number >= calls.GetLowerBound(0))
            {
                try
                {
                    if (IAXClient.clientInitialized)
                    {
                        object obj = IAXClient.syncroot;
                        //ObjectFlowControl.CheckForSyncLockOnValueType(obj);
                        lock (obj)
                        {
                            if ((IAXClient.calls[number].callstatus & CallStates.IAXC_CALL_STATE_RINGING) <= 0)
                            {
                                retval = false;
                            }
                            IAXClient.iaxc_send_busy_on_incoming_call(number);
                            retval = true;
                        }
                    }
                }
                catch
                {
                    retval = false;
                }
            }
            return retval;
        }
        public bool blind_transfer_call(int number, string address)
        {
            if (number <= calls.GetUpperBound(0) & number >= calls.GetLowerBound(0))
            {
                try
                {
                    if (clientInitialized)
                    {
                        lock (syncroot)
                        {
                            CallStates theCallState;
                            theCallState = calls[number].callstatus;
                            if (!((theCallState & CallStates.IAXC_CALL_STATE_ACTIVE) > 0))
                            {
                                System.Console.WriteLine("That line isn't active");
                                return false;
                            }
                            else
                            {
                                IntPtr numberptr = IntPtr.Zero;
                                numberptr = Marshal.StringToCoTaskMemAnsi(address);
                                iaxc_blind_transfer_call(number, numberptr.ToInt32());
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.StackTrace);
                    return false;
                }
            }
            return false;
        }
        public bool send_url(int linktxt, string linkaddr)
        {
            //TODO: fix this
            return false;
        }
        public bool dump_all_calls()
        {
            try
            {
                lock (syncroot)
                {
                    if (clientInitialized)
                    {
                        iaxc_dump_all_calls();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error in function call: " + ex.Message + " -- " + ex.StackTrace);
            }
            return false;
            
        }
        public bool millisleep(long milliseconds)
        {
            bool retval = false;
            try
            {
                lock (syncroot)
                {
                    if (IAXClient.clientInitialized)
                    {
                        IAXClient.iaxc_millisleep(milliseconds);
                        retval = true;
                    }
                    retval = false;
                }
            }
            catch
            {
            }
            return retval;
        }
        public bool set_audio_output(int mode)
        {
            try
            {
                lock (syncroot)
                {
                    if (clientInitialized)
                    {
                        iaxc_set_audio_output(mode);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error in function call: " + ex.Message + " -- " + ex.StackTrace);
            }
            return false;
        }
        public bool set_silence_threshold(float val)
        {
            bool retval = false;
            try
            {
                lock (syncroot)
                {
                    if (IAXClient.clientInitialized)
                    {
                        IAXClient.iaxc_set_silence_threshold(val);
                        retval = true;
                    }
                    retval = false;
                }
            }
            catch
            {
            }
            return retval;
        }
        public int get_mic_boost()
        {
            int retval = -1;
            try
            {
                lock (syncroot)
                {
                    if (clientInitialized)
                    {
                        retval = iaxc_mic_boost_get();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error in function call: " + ex.Message + " -- " + ex.StackTrace);
            }
            return retval;
        }
        public int set_mic_boost(int val)
        {
            int retval = -1;
            try
            {
                lock (syncroot)
                {
                    if (IAXClient.clientInitialized)
                    {
                        retval = IAXClient.iaxc_mic_boost_set(val);
                    }
                }
            }
            catch
            {
            }
            return retval;
        }
        public int output_level_set(float val)
        {
            int retval = -1;
            try
            {
                lock (syncroot)
                {
                    if (clientInitialized)
                    {
                        retval = iaxc_output_level_set(val);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error in function call: " + ex.Message + " -- " + ex.StackTrace);
            }
            return retval;
        }
        public int input_level_set(float val)
        {
            int retval = -1;
            try
            {
                lock (syncroot)
                {
                    if (IAXClient.clientInitialized)
                    {
                        retval = IAXClient.iaxc_input_level_set(val);
                    }
                }
            }
            catch(Exception ex)
            {
                System.Console.WriteLine("Error in function call: " + ex.Message + " -- " + ex.StackTrace);

            }
            return retval;
        }
        public int quelch(int number, int moh)
        {
            int retval = -1;
            if (number <= calls.GetUpperBound(0) & number >= calls.GetLowerBound(0))
            {
                try
                {
                    if (clientInitialized)
                    {
                        lock (syncroot)
                        {
                            CallStates theCallState;
                            theCallState = calls[number].callstatus;
                            if (!((theCallState & CallStates.IAXC_CALL_STATE_ACTIVE) > 0))
                            {
                                System.Console.WriteLine("That line isn't active");
                            }
                            else
                            {
                                retval = iaxc_quelch(number, moh);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Error trapped in iaxClientWrapper.AnswerCall");
                    System.Console.WriteLine(ex.StackTrace);
                }
            }
            return retval;
            
        }
        public int unquelch(int number)
        {
            int retval = -1;
            if (number <= calls.GetUpperBound(0) & number >= calls.GetLowerBound(0))
            {
                try
                {
                    if (IAXClient.clientInitialized)
                    {
                        object obj = IAXClient.syncroot;
                        lock (obj)
                        {
                            if (!((IAXClient.calls[number].callstatus & CallStates.IAXC_CALL_STATE_ACTIVE) <= 0))
                            {
                                retval = IAXClient.iaxc_unquelch(number);
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            return retval;
        }
        private bool set_jb_target_extra(long val)
        {
            try { 
                lock (syncroot) { 
                    if (clientInitialized) { 
                        iaxc_set_jb_target_extra(val); 
                        return true; 
                    } 
                    else { 
                        return false; 
                    } 
                } 
            } 
            catch (Exception ex) { 
                System.Console.WriteLine("Error in function call: " + ex.Message + " -- " + ex.StackTrace); 
            }
            return false;
        }
        public FilterTypes get_filters()
        {
            FilterTypes filterTypes = FilterTypes.IAXC_FILTER_ERROR;
            try
            {
                lock (syncroot)
                {
                    if (IAXClient.clientInitialized)
                    {
                        return IAXClient.iaxc_get_filters();
                    }
                    filterTypes = ((FilterTypes)0);
                }
            }
            catch
            {
            }
            return filterTypes;
        }
        public bool set_filters(FilterTypes filters)
        {
            try
            {
                lock (syncroot)
                {
                    if (clientInitialized)
                    {
                        iaxc_set_filters(filters);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error in function call: " + ex.Message + " -- " + ex.StackTrace);
            }
            return false;
        }
        public VideoPrefs get_video_prefs()
        {
            VideoPrefs videoPrefs = ((VideoPrefs)0);
            try
            {
                lock (syncroot)
                {
                    if (IAXClient.clientInitialized)
                    {
                        return IAXClient.iaxc_get_video_prefs();
                    }
                }
            }
            catch
            {
            }
            return videoPrefs;
        }
        public bool set_video_prefs(VideoPrefs prefs)
        {
            try
            {
                lock (syncroot)
                {
                    if (clientInitialized)
                    {
                        iaxc_set_video_prefs(prefs);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error in function call: " + ex.Message + " -- " + ex.StackTrace);
            }
            return false;
        }
        public VideoPreferencesContainer get_video_format_get_cap()
        {
            VideoPreferencesContainer videoPreferencesContainer = null;
            //TODO:  Implement this method
            return videoPreferencesContainer;

        }
        public bool set_video_format_set_cap(VideoPreferencesContainer prefs)
        {
            try
            {
                lock (syncroot)
                {
                    if (clientInitialized)
                    {
                        iaxc_video_format_set_cap(prefs.preferred, prefs.allowed);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error in function call: " + ex.Message + " -- " + ex.StackTrace);
            }
            return false;
        }
        private int video_bypass_jitter(int val)
        {
            int retval = -1;
            try
            {
                lock (syncroot)
                {
                    if (IAXClient.clientInitialized)
                    {
                        retval = IAXClient.iaxc_video_bypass_jitter(val);
                    }
                }
            }
            catch
            {
            }
            return retval;
        }
        public int is_camera_working()
        {
            int retval = -1;
            try
            {
                lock (syncroot)
                {
                    if (clientInitialized)
                    {
                        retval = iaxc_is_camera_working();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error in function call: " + ex.Message + " -- " + ex.StackTrace);
            }
            return retval;
        }

        public bool set_speex_settings(int decodeEnhance, float quality, int bitrate, int vbr, int abr, int complexity)
        {
            try
            {
                lock (syncroot)
                {
                    if (IAXClient.clientInitialized)
                    {
                        IAXClient.iaxc_set_speex_settings(decodeEnhance, quality, bitrate, vbr, abr, complexity);
                        return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        }
        public bool video_format_set(MediaFormats preferred, MediaFormats allowed, int framerate, int bitrate, int width, int height, int fs)
        {
            try
            {
                lock (syncroot)
                {
                    if (clientInitialized)
                    {
                        iaxc_video_format_set(preferred, allowed, framerate, bitrate, width, height, fs);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error in function call: " + ex.Message + " -- " + ex.StackTrace);
            }
            return false;
        }
        public bool video_params_change(int framerate, int bitrate, int width, int height, int fs)
        {
            try
            {
                lock (syncroot)
                {
                    if (IAXClient.clientInitialized)
                    {
                        IAXClient.iaxc_video_params_change(framerate, bitrate, width, height, fs);
                        return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        }
        public bool audio_devices_set(int inputDevNo, int outputDevNo, int ringDevNo)
        {
            try
            {
                lock (syncroot)
                {
                    if (clientInitialized)
                    {
                        iaxc_audio_devices_set(inputDevNo, outputDevNo, ringDevNo);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error in function call: " + ex.Message + " -- " + ex.StackTrace);
            }
            return false;
        }
        public AudioDeviceListStructure audio_devices_get()
        {
            AudioDeviceListStructure audioDeviceListStructure1;
            IntPtr arrayAddrLocation;
            int i;
            IAXAudioDevice device;
            IAXAudioDevice[] arrdevice;
            IntPtr devnameptr;
            int i1;
            int i2;
            audioDeviceListStructure1 = new AudioDeviceListStructure();
            audioDeviceListStructure1.count = -1;
            try
            {
                lock (syncroot)
                {
                    if (IAXClient.clientInitialized)
                    {
                        arrayAddrLocation = IntPtr.Zero;
                        devnameptr = IntPtr.Zero;
                        i1 = IAXClient.iaxc_audio_devices_get(((int)IAXClient.arrayAddrPtr), ((int)IAXClient.devnbrptr), ((int)IAXClient.inputnbr), ((int)IAXClient.outputnbr), ((int)IAXClient.ringnbr));
                        arrayAddrLocation = Marshal.ReadIntPtr(IAXClient.arrayAddrPtr);
                        audioDeviceListStructure1.count = Marshal.ReadInt32(IAXClient.devnbrptr);
                        audioDeviceListStructure1.inputdevnumber = Marshal.ReadInt32(IAXClient.inputnbr);
                        audioDeviceListStructure1.outputdevnumber = Marshal.ReadInt32(IAXClient.outputnbr);
                        audioDeviceListStructure1.ringdevnumber = Marshal.ReadInt32(IAXClient.ringnbr);
                        arrdevice = new IAXAudioDevice[(audioDeviceListStructure1.count - 1) + 1];
                        i2 = 12;
                        i1 = 0;
                        i = 0;
                        while (i1 <= (audioDeviceListStructure1.count - 1))
                        {
                            devnameptr = Marshal.ReadIntPtr(arrayAddrLocation, i + 0);
                            device.capabilities = (AudioDeviceTypes)Marshal.ReadInt32(arrayAddrLocation, i + 4);
                            device.devid = Marshal.ReadInt32(arrayAddrLocation, i + 8);
                            device.devname = string.Copy(Marshal.PtrToStringAnsi(devnameptr));
                            device.devnumber = i1;
                            arrdevice[i1] = device;
                            i += i2;
                            i1++;
                        }
                        audioDeviceListStructure1.deviceArray = arrdevice;
                        return audioDeviceListStructure1;
                    }
                    AudioDeviceListStructure audioDeviceListStructure = audioDeviceListStructure1;
                }
            }
            catch
            {
            }
            return audioDeviceListStructure1;
        }
        private bool captureEventHandle()
        {
            try
            {
                if (!IAXClient.eventHandleCaptured)
                {
                    CreateParams createParams = new CreateParams();
                    createParams.Caption = "AsteriaSGI Internal Event Window";
                    createParams.Style &= -268435457;
                    IAXClient.nwin = new NativeWindow();
                    IAXClient.nwin.CreateHandle(createParams);
                    Random r = new Random();
                    IAXClient.myeventid = (((int)Math.Round(((double)(r.Next(1) * 10000F))))) + 60000;
                    IAXClient.thisdelegate = new IAXClient.SubClassProcDelegate(this.handler);
                    IAXClient.originalHandle = GCHandle.Alloc(IAXClient.nwin.Handle, GCHandleType.Pinned);
                    IAXClient.newprocptr = IAXClient.nwin.Handle.ToInt32();
                    IAXClient.delegateHandle = GCHandle.Alloc(IAXClient.thisdelegate);
                    IAXClient.prevprocptr = IAXClient.SetWindowLong(IAXClient.newprocptr, -4, IAXClient.thisdelegate);
                    IAXClient.eventHandleCaptured = true;
                }
            }
            catch
            {
                IAXClient.eventHandleCaptured = false;
            }
            return IAXClient.eventHandleCaptured;
        }
        private int handler(int hwnd, int msg, int wParam, int lParam)
        {

            if ((msg == myeventid)) 
            {
                processEvent(lParam);
                iaxc_free_event(lParam);
                return 1;
            }
            else 
            {
                return callwindowproc(prevprocptr, hwnd, msg, wParam, lParam);
            }

        }
        private object processEvent(int srcPtr)
        {
            IAXClient.NctIaxcEventArgsClass nctIaxcEventArgsClass;
            IntPtr intPtr;
            int eventType;
            intPtr = new IntPtr(srcPtr);
            int i = Marshal.ReadInt32(intPtr, 0);
            eventType = Marshal.ReadInt32(intPtr, 4);
            switch (eventType)
            {
                case 1:
                    try
                    {
                        if (IAXTextEvent != null)
                        {
                            IAXEventText text = (IAXEventText)Marshal.PtrToStructure(intPtr, typeof(IAXEventText));
                            processEvent(text);
                            IAXTextEvent(text);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("Exception in text event: " + e.Message);

                    }
                    break;

                case 2:
                    try
                    {
                        if (IAXLevelsEvent != null)
                        {
                            IAXEventLevels levels = (IAXEventLevels)Marshal.PtrToStructure(intPtr, typeof(IAXEventLevels));
                            processEvent(levels);
                            IAXLevelsEvent(levels);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("Exception in levels event: " + e.Message);
                    }
                    break;

                case 3:
                    try
                    {
                        if (IAXCallStateEvent != null)
                        {
                            IAXEventCallState state = (IAXEventCallState)Marshal.PtrToStructure(intPtr, typeof(IAXEventCallState));
                            processEvent(state);
                            IAXCallStateEvent(state);
                        }

                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("Exception in state event: " + e.Message);

                    }
                    break;

                case 4:
                    try
                    {
                        if (IAXNetstatsEvent != null)
                        {
                            IAXEventNetstats netstats = (IAXEventNetstats)Marshal.PtrToStructure(intPtr, typeof(IAXEventNetstats));
                            processEvent(netstats);
                            IAXNetstatsEvent(netstats);
                        }                        
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("Exception in netstats event: " + e.Message);

                    }
                    break;

                case 5:
                    try
                    {
                        if (IAXURLEvent != null)
                        {
                            IAXEventURL url = (IAXEventURL)Marshal.PtrToStructure(intPtr, typeof(IAXEventURL));
                            processEvent(url);
                            IAXURLEvent(url);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("Exception in url event: " + e.Message);
                    }
                    break;

                case 6:
                    try
                    {
                        if (IAXVideoEvent != null)
                        {
                            IAXEventVideo video = (IAXEventVideo)Marshal.PtrToStructure(intPtr, typeof(IAXEventVideo));
                            processEvent(video);
                            IAXVideoEvent(video);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("Exception in video event: " + e.Message);
                    }
                    break;

                case 8:
                    try
                    {
                        if (IAXRegisterEvent != null)
                        {
                            IAXEventRegistration registration = (IAXEventRegistration)Marshal.PtrToStructure(intPtr, typeof(IAXEventRegistration));
                            processEvent(registration);
                            IAXRegisterEvent(registration);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("Exception in registration event: " + e.Message);
                    }
                    break;

                case 10:
                    try
                    {
                        if (IAXAudioEvent != null)
                        {
                            IAXEventAudio audio = (IAXEventAudio)Marshal.PtrToStructure(intPtr, typeof(IAXEventAudio));
                            processEvent(audio);
                            IAXAudioEvent(audio);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("Exception in audio event: " + e.Message);
                    }
                    break;

                case 11:
                    try
                    {
                        if (IAXVideoStatsEvent != null)
                        {
                            IAXEventVideoStats videostats = (IAXEventVideoStats)Marshal.PtrToStructure(intPtr, typeof(IAXEventVideoStats));
                            processEvent(videostats);
                            IAXVideoStatsEvent(videostats);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("Exception in video stats event: " + e.Message);
                      }
                    break;

            }
            return 0;
        }
        private void processEvent(IAXEventText evt)
        {
            // TODO: Need to proccess this event
        }
        private void processEvent(IAXEventLevels evt)
        {
            if (!((IAXClient.CurrentInputLevel == evt.input) & (IAXClient.CurrentOutputLevel == evt.output)))
            {
                IAXClient.CurrentInputLevel = evt.input;
                IAXClient.CurrentOutputLevel = evt.output;
            }
        }
        private void processEvent(IAXEventRegistration evt)
        {
            if (reglist.ContainsKey(evt.id.ToString()))
            {
                regUserInfo userreg = (regUserInfo)reglist[evt.id.ToString()];
                if ((userreg != null))
                    userreg.lastReply = evt.reply;
            } 
        }
        private void processEvent(IAXEventCallState evt)
        {
            IAXNetstat netstat;
            lock (syncroot)
            {
                System.Console.WriteLine("iaxc_ev_call_state event for call:" + evt.callno + " State:" + evt.state);
                IAXClient.calls[evt.callno].callstatus = evt.state;
                IAXClient.calls[evt.callno].mediaformat = evt.format;
                IAXClient.calls[evt.callno].remote = evt.remote;
                IAXClient.calls[evt.callno].remoteName = evt.remote_name;
                IAXClient.calls[evt.callno].local = evt.local;
                IAXClient.calls[evt.callno].localContext = evt.local_context;
                if ((evt.state & CallStates.IAXC_CALL_STATE_FREE) > CallStates.IAXC_CALL_STATE_FREE)
                {
                    IAXClient.calls[evt.callno].videoFormat = 0;
                    IAXClient.calls[evt.callno].videoHeight = 0;
                    IAXClient.calls[evt.callno].videoWidth = 0;
                    IAXClient.calls[evt.callno].videoUnsafeAddrPtr = IntPtr.Zero;
                    IAXClient.calls[evt.callno].url = "";
                    IAXClient.calls[evt.callno].urlType = ((URLReplyTypes)0);
                    IAXClient.calls[evt.callno].RemoteNetstats = netstat;
                    IAXClient.calls[evt.callno].LocalNetstats = netstat;
                    IAXClient.calls[evt.callno].netstat_rtt = 0;
                }
            }
           
        }
        private void processEvent(IAXEventVideo evt)
        {
            lock (syncroot)
            {
                if (evt.callno > 0 & evt.callno <= calls.GetUpperBound(0))
                {
                    calls[evt.callno].videoFormat = evt.format;
                    calls[evt.callno].videoHeight = evt.height;
                    calls[evt.callno].videoWidth = evt.width;
                    calls[evt.callno].videoUnsafeAddrPtr = evt.datapointer;
                }
            } 
        }
        private void processEvent(IAXEventNetstats evt)
        {
            lock (syncroot)
            {
                IAXClient.calls[evt.callno].netstat_rtt = evt.rtt;
                IAXClient.calls[evt.callno].LocalNetstats = evt.local;
                IAXClient.calls[evt.callno].RemoteNetstats = evt.remote;
            }
        }
        private void processEvent(IAXEventURL evt)
        {
            lock (syncroot)
            {
                calls[evt.callno].url = evt.url;
                calls[evt.callno].urlType = (URLReplyTypes)evt.urltype;
            } 
        }
        private void processEvent(IAXEventAudio evt)
        {
            //TODO: Process this event
        }
        private void processEvent(IAXEventVideoStats evt)
        {
            lock (syncroot)
            {
                calls[evt.callno].videostats = (IAXVideoStat)evt.stats;
            } 
        }
        public delegate void IAXClientEventEventHandler(object sender, IAXClient.NctIaxcEventArgsClass evt);
        public class NctIaxcEventArgsClass : EventArgs
        {
            public object eventType;
            public object iaxcEvent;
            public NctIaxcEventArgsClass(object e, object o)
                : base()
            {
                this.eventType = e;
                this.iaxcEvent = o;
            }
        }

        public class callInfo
        {
            public IntPtr calleridname;
            public IntPtr calleridnumber;
            public Queue textQueue;
            public IntPtr numberptr;
            public string number;
            public int callno;
            public CallStates callstatus;
            public MediaFormats mediaformat;
            public string remote;
            public string remoteName;
            public string local;
            public string localContext;
            public MediaFormats videoFormat;
            public int videoWidth;
            public int videoHeight;
            public IntPtr videoUnsafeAddrPtr;
            public int netstat_rtt;
            public IAXNetstat LocalNetstats;
            public IAXNetstat RemoteNetstats;
            public string url;
            public URLReplyTypes urlType;
            public object syncroot;
            public IAXVideoStat videostats;
            public callInfo()
            {
                textQueue = new Queue(); 
            }

            internal void setnumber(string n)
            {
                
                number = n;
                if (!(numberptr == IntPtr.Zero))
                {
                    try
                    {
                        numberptr = Marshal.StringToCoTaskMemAnsi(number);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                try
                {
                    numberptr = Marshal.StringToCoTaskMemAnsi(number);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(("Error attempting to set number. " + ex.StackTrace));
                }
            }
            ~callInfo()
            {
                try
                {
                    lock (syncroot)
                    {
                        Marshal.ZeroFreeCoTaskMemAnsi(this.numberptr);
                        this.numberptr = IntPtr.Zero;
                    }
                }
                catch
                {
                }
            }
        }
        private class regUserInfo
        {
            public IntPtr calleridname;
            public IntPtr calleridnumber;
            public IntPtr usernameptr;
            public IntPtr passwordptr;
            public IntPtr serveraddrptr;
            public int regid;
            public RegistrationReplyTypes lastReply;
            public regUserInfo(ref string username, ref string password, ref string serveraddr)
                : base()
            {
                this.calleridname = IntPtr.Zero;
                this.calleridnumber = IntPtr.Zero;
                this.usernameptr = IntPtr.Zero;
                this.passwordptr = IntPtr.Zero;
                this.serveraddrptr = IntPtr.Zero;
                this.regid = 0;
                this.lastReply = 0;
                this.usernameptr = Marshal.StringToCoTaskMemAnsi(username);
                this.passwordptr = Marshal.StringToCoTaskMemAnsi(password);
                this.serveraddrptr = Marshal.StringToCoTaskMemAnsi(serveraddr);
            }
            public void unAlloc()
            {
                try
                {
                    if (!(usernameptr == IntPtr.Zero))
                        Marshal.ZeroFreeCoTaskMemAnsi(usernameptr);
                }
                catch (Exception ex)
                {
                }
                try
                {
                    if (!(passwordptr == IntPtr.Zero))
                        Marshal.ZeroFreeCoTaskMemAnsi(passwordptr);
                }
                catch (Exception ex)
                {
                }
                try
                {
                    if (!(serveraddrptr == IntPtr.Zero))
                        Marshal.ZeroFreeCoTaskMemAnsi(serveraddrptr);
                }
                catch (Exception ex)
                {
                }
                try
                {
                    if (!(calleridname == IntPtr.Zero))
                        Marshal.ZeroFreeCoTaskMemAnsi(calleridname);
                }
                catch (Exception ex)
                {
                }
                try
                {
                    if (!(calleridnumber == IntPtr.Zero))
                        Marshal.ZeroFreeCoTaskMemAnsi(calleridnumber);
                }
                catch (Exception ex)
                {
                }
                try
                {
                    usernameptr = IntPtr.Zero;
                    passwordptr = IntPtr.Zero;
                    serveraddrptr = IntPtr.Zero;
                    calleridname = IntPtr.Zero;
                    calleridnumber = IntPtr.Zero;
                }
                catch (Exception ex)
                {
                } 
            }
            ~regUserInfo()
            {
                try
                {
                    unAlloc();
                }
                catch
                {
                }
            }
        }
        private class pointerholder
        {
            public IntPtr objectptr;
            public object obj;
            public pointerholder(ref object o)
            {
                try
                {
                    objectptr = Marshal.AllocHGlobal(Marshal.SizeOf(o));
                    obj = o;
                    Marshal.StructureToPtr(o, objectptr, true);
                }
                catch (Exception ex)
                {
                } 
            }
            ~pointerholder()
            {
                try
                {
                    if (this.objectptr != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(this.objectptr);
                        this.objectptr = IntPtr.Zero;
                    }
                }
                catch
                {
                }
            }
        }

        private delegate int SubClassProcDelegate(int hwnd, int msg, int wParam, int lParam);

        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_set_preferred_source_udp_port(int udpPort);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_get_bind_port();
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_initialize(int MaxNumberOfCalls);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_shutdown();
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_set_formats(MediaFormats preferred, MediaFormats allows);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_set_min_outgoing_framesize(int samples);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_set_callerid(int nameAddr, int numberAddr);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_start_processing_thread();
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_stop_processing_thread();
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_call(int addressPtr);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_register(int usernamePtr, int passwordPtr, int hostnamePtr);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_unregister(int id);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_send_busy_on_incoming_call(int callno);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_answer_call(int callno);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_blind_transfer_call(int callNo, int number);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_dump_all_calls();
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_dump_call();
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_reject_call();
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_reject_call_number(int callno);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_send_dtmf(char digit);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_send_text(int textAddrPtr);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_send_url(int urlAddrPtr, int linkAddrPtr);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_millisleep(long milliseconds);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_set_silence_threshold(float val);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_set_audio_output(int outputMode);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_select_call(int callno);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_first_free_call();
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_selected_call();
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_quelch(int callno, int MOH);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_unquelch(int callno);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_mic_boost_get();
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_mic_boost_set(int level);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_version(int versionStringAddr);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_set_jb_target_extra(long jbvalue);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_audio_devices_get(int audioDevicesArrayAddrPtr, int ndevsptrInt, int InputptrInt, int OutputptrInt, int ringptrInt);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_audio_devices_set(int input, int output, int ring);
        [DllImport("libiaxclient.dll")]
        private static extern float iaxc_input_level_get();
        [DllImport("libiaxclient.dll")]
        private static extern float iaxc_output_level_get();
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_input_level_set(float level);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_output_level_set(float level);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_play_sound(int soundStructAddrptr, int ringDevice);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_stop_sound(int id);
        [DllImport("libiaxclient.dll")]
        private static extern FilterTypes iaxc_get_filters();
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_set_filters(FilterTypes filters);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_set_files(int inputFileAddrPtr, int outputFileAddrPtr);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_set_speex_settings(int decodeEnhance, float quality, int bitrate, int vbr, int abr, int complexity);
        [DllImport("libiaxclient.dll")]
        private static extern VideoPrefs iaxc_get_video_prefs();
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_set_video_prefs(VideoPrefs prefs);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_listvidcapdevices(int bufferAddrPtr, int buffersize);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_video_format_get_cap(int preferredAddrPtr, int allowedAddrPtr);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_video_format_set_cap(VideoPrefs preferred, VideoPrefs allowed);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_video_format_set(MediaFormats preferred, MediaFormats allowed, int framerate, int bitrate, int width, int height, int fs);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_video_params_change(int framerate, int bitrate, int width, int height, int fs);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_set_holding_frame(int AddressPtr);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_video_bypass_jitter(int bypass);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_is_camera_working();
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_YUV420_to_RGB32(int width, int height, int SrcBufferAddrPtr, int DestBufferAddrPtr);
        [DllImport("libiaxclient.dll")]
        private static extern int iaxc_set_event_callpost(int hwnd, int id);
        [DllImport("libiaxclient.dll")]
        private static extern void iaxc_free_event(int eventHandle);
        [DllImport("USER32.DLL", EntryPoint = "SetWindowLongA", ExactSpelling = true, CharSet = CharSet.Ansi)]
        private static extern int SetWindowLong(int hwnd, int attr, IAXClient.SubClassProcDelegate lVal);
        [DllImport("user32.dll", EntryPoint = "CallWindowProcA", ExactSpelling = true, CharSet = CharSet.Ansi)]
        private static extern int callwindowproc(int prevprocptr, int hwnd, int msg, int wparam, int lparam);

    }
}