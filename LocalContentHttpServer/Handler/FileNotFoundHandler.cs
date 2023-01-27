using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LocalContentHttpServer.Handler
{
    public class FileNotFoundHandler : IRequestHandler
    {
        public string Prefix => "/";

        public string DefaultDocument => "";

        public bool CanHandleRequest(string method, string path)
        {
            return true;
        }

        public async void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            String responseString = $"<html><body>Cannof find the file at the location [{request.Url.ToString()}]</body></html>";
            byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.StatusCode = 404;
            response.ContentLength64 = responseBytes.Length;
            var output = response.OutputStream;
            await output.WriteAsync(responseBytes, 0, responseBytes.Length);
            await output.FlushAsync();
            output.Close();
        }
    }
}
