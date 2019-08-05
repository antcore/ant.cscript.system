using System;
using System.Configuration;
using System.Threading;

namespace ant.csscript.run
{
    public class ControlService
    {
        private CsScriptRun _CsScriptRun;
        //private WebSocketService _webSocket;

        public ControlService()
        {
            _CsScriptRun = new CsScriptRun();

            //int websocketPort = 10244; 
            //websocketPort = int.Parse(ConfigurationManager.AppSettings["WebSocket:Port"]);
            //_webSocket = new WebSocketService(websocketPort);
        }

        public void Start()
        {
            ThreadAction((o) =>
            {
                _CsScriptRun.Start();
            });

            //ThreadAction((o) =>
            //{
            //    _webSocket.Start();
            //});
        }

        public void Stop()
        {
            _CsScriptRun.Stop();
            //_webSocket.Stop();
        }

        private void ThreadAction(Action<object> action)
        {
            new Thread(new ParameterizedThreadStart(action)).Start();
        }
    }
}