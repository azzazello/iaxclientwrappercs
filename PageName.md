C# IAXClient Wrapper

Asteria Solutions Group is pleased to announce the first release of the C# IAXClient Wrapper. for vs.NET 2005.

This code is based on the Visual Basic.NET IAXClient Wrapper developed by Andrew Pollack at Second Signal (http://www.secondsignal.com/secondsignal/sshome.nsf/html/2ndSignal-IAXCl...).

This source code has been released under the following conditions:

  * If you use this wrapper you must include credit to Asteria Solutions Group and Second Signal in the documentation and the About form.
  * You adhere to license requirements of the iaxclient library (http://sourceforge.net/projects/iaxclient).
  * There is no warranty, guarantee, support for the source code. Use it at your own risk.

If you find the wrapper useful then please let us know. If you find a bug and fix it please send us the fix. If you have questions or comments then you are free to ask us but we may not reply. We will not "hand-hold" someone trying to get this wrapper to work.
chris@asteriasgi.com

Download Source

We plan on releasing the source code to a simple softphone utilizing this wrapper very soon. Until then here's a quick start guide:

```
  //Create a new instance of the IAXClient
  IAXClient iax = new IAXClient();

  //initialize 4 lines
  iax.initialize(4);

  //register with the Asterisk (or whatever) server
  iax.register("user1", "pass1", "192.168.1.1");

  //Create an event handler to handle the Call State Events.
  iax.IAXCallStateEvent += new IAXClient.IAXCallStateEventHandler(HandleIAXCallStateEvent);

  //Place a call with the wrapper
  int callno = iax.placeCall("user1:pass1@192.168.1.1/18005551212");
```