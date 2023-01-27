using LocalContentHttpServer.Handler;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace LocalContentHttpServer
{
    public class LocalServer: IDisposable
    {
        HttpListener listener;
        Thread _listenerThread;
        bool _keepListening= false;

        List<IRequestHandler> _handlers = new List<IRequestHandler>();



        static LocalServer _instance;

        public static LocalServer Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new LocalServer(false);
                }
                return _instance;
            }
        }

        public LocalServer(bool autoStart = false) {
            var fnf = new FileNotFoundHandler();
            AddHandler(fnf);
            if(autoStart)
            {
                Start();
            }
        }

        

        public void AddHandler(IRequestHandler handler)
        {
            _handlers.Insert(0, handler);
        }


        public void Start()
        {
            if (_listenerThread == null)
            {
                _listenerThread = new Thread(ListenRoutine);
                _listenerThread.Start();
            }
        }

        public void Stop()
        {
            _keepListening = false;
            listener.Stop();
            _listenerThread.Join();
            _listenerThread = null;
        }


        String[] PrefixList
        {
            get
            {
                return new string[] { "http://*:8081/",  "http://127.0.0.1:8081/" };
            }
        }

        void ListenRoutine()
        {
            _keepListening = true;
            listener = new HttpListener();
            
            foreach (var prefix in PrefixList)
            {
                listener.Prefixes.Add(prefix);
            }
            
            listener.Start();
            while (_keepListening)
            {
                //This call blocks until a request comes in
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                bool handled = false;
                foreach(var handler in _handlers)
                {
                    if(handler.CanHandleRequest(request.HttpMethod, request.Url.AbsolutePath))
                    {
                        handler.HandleRequest(request, response);
                        handled = true;
                        break;
                    }
                }
                if (!handled)
                {
                    HandleResponse(request, response);
                }
            }
            listener.Stop();

        }



        async void HandleResponse(HttpListenerRequest request, HttpListenerResponse response)
        {
            String responseString = "<html><body>Hello World</body></html>";
            byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = responseBytes.Length;
            var output = response.OutputStream;
            await output.WriteAsync(responseBytes, 0, responseBytes.Length);
            await output.FlushAsync();
            output.Close();
        }

        public void Dispose()
        {
            _keepListening= false;
            if(listener!= null)
            {
                if(listener.IsListening)
                {

                    listener.Close();
                }
            }
        }
    }
}
