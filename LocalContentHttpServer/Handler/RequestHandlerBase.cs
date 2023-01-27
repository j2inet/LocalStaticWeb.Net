using LocalContentHttpServer.Handler.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text;

namespace LocalContentHttpServer.Handler
{
    public abstract class RequestHandlerBase: IRequestHandler
    {
        string _prefix;
        public string Prefix => _prefix;
        public RequestHandlerBase(String prefix)
        {
            this._prefix = prefix;
        }

        public string DefaultDocument
        {
            get  => "index.html";
        }

        static StringDictionary ExtensionToMimeType = new StringDictionary();

        static void LoadMimeTypes()
        {
            var resourceStreamNameList = typeof(RequestHandlerBase).Assembly.GetManifestResourceNames();
            var nameList = new List<String>(resourceStreamNameList);
            var targetResource = nameList.Find(x => x.EndsWith(".mimetypes.json"));
            if (targetResource != null)
            {
                DataContractJsonSerializer dcs = new DataContractJsonSerializer(typeof(LocalContentHttpServer.Handler.Data.MimeTypeInfo[]));
                using (var resourceStream = typeof(RequestHandlerBase).Assembly.GetManifestResourceStream(targetResource))
                {
                    var mtList = dcs.ReadObject(resourceStream) as MimeTypeInfo[];
                    foreach(var m in mtList)
                    {
                        ExtensionToMimeType[m.Extension] = m.MimeTypeString;
                    }
                }
                    
            }
        }
        static RequestHandlerBase()
        {

            
            ExtensionToMimeType.Clear();
            ExtensionToMimeType.Add("js", "application/javascript");
            ExtensionToMimeType.Add("html", "text/html");
            ExtensionToMimeType.Add("htm", "text/html");
            ExtensionToMimeType.Add("png", "image/png");
            ExtensionToMimeType.Add("svg", "image/svg+xml");
            LoadMimeTypes();
        }
        public static string GetMimeTypeForExtension(string extension)
        {
            if(extension.Contains("."))
            {
                extension = extension.Substring( extension.LastIndexOf("."));
            }
            if(extension.StartsWith('.'))
                extension = extension.Substring(1);
            if(ExtensionToMimeType.ContainsKey(extension))
            {
                return ExtensionToMimeType[extension];
            }
            return null;
        }


        public abstract bool CanHandleRequest(string method, string path);
        public abstract void HandleRequest(HttpListenerRequest request, HttpListenerResponse response);
    }
}
