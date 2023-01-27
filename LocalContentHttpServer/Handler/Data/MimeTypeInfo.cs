using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace LocalContentHttpServer.Handler.Data
{
    public  class MimeTypeInfo
    {
        [DataMember(IsRequired = true, Name = "extension")]
        public string Extension { get; set; }
        [DataMember(IsRequired = true, Name = "mimeType")]
        public string MimeTypeString { get; set; }
    }
}
