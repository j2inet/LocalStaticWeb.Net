using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LocalContentHttpServer
{
    public interface IRequestHandler
    {
        public string Prefix { get;  }
        public string DefaultDocument { get; }
        bool CanHandleRequest(string method, string path);
        void HandleRequest(HttpListenerRequest request, HttpListenerResponse response);
    }
}
