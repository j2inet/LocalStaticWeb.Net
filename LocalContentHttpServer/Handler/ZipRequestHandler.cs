using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace LocalContentHttpServer.Handler
{
    public class ZipRequestHandler : RequestHandlerBase
    {
        ZipArchive _zipArchive;
        readonly bool _decompress ;
        readonly bool _caseSensitive = true;
        Dictionary<string, ZipArchiveEntry> _entryLookup = new Dictionary<string, ZipArchiveEntry>();


        private bool Contains(String path)
        {
            if (path.EndsWith("/"))
                path += DefaultDocument;
            if(path.StartsWith("/"))
                path = path.Substring(1);
            return _entryLookup.ContainsKey(path);
        }
        public ZipRequestHandler(String prefix, string pathToZipArchive, bool caseSensitive = true, bool decompress = false):base(prefix)
        {
            FileStream fs = new FileStream(pathToZipArchive, FileMode.Open, FileAccess.Read);
            _zipArchive = new ZipArchive(fs);            
            this._decompress = decompress;
            this._caseSensitive = caseSensitive;
            foreach (var entry in _zipArchive.Entries)
            {
                var entryName = (_caseSensitive) ? entry.FullName : entry.FullName.ToLower();
                _entryLookup[entryName] = entry;
            }

        }
        public ZipRequestHandler(String prefix, ZipArchive archive, bool caseSensitive = true, bool decompress = false):base(prefix)
        {
            this._zipArchive = archive;
            this._decompress = decompress;
            this._caseSensitive = caseSensitive;
            foreach(var entry in archive.Entries)
            {
                var entryName = (_caseSensitive) ? entry.FullName : entry.FullName.ToLower();
                _entryLookup[entryName] = entry;
            }
        }


        public override bool CanHandleRequest(string method, string path)
        {
            if (method != "GET") return false;
            return Contains(path);
        }

        public override void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            var path = request.Url.AbsolutePath;

            if (path.EndsWith("/"))
                path += DefaultDocument;
            if (path.StartsWith("/"))
                path = path.Substring(1);

            if (_entryLookup.TryGetValue(path, out var entry))
            {
                var mimeType = GetMimeTypeForExtension(path);
                if(mimeType != null)
                {
                    response.AppendHeader("Content-Type", mimeType);
                }
                try
                {
                    var size = entry.Length;
                    byte[] buffer = new byte[size];
                    var entryFile = entry.Open();
                    entryFile.Read(buffer, 0, buffer.Length);

                    var output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Flush();
                    output.Close();
                }catch(Exception exc)
                {

                }
            }
            else
            {
                
            }
        }
    }
}
