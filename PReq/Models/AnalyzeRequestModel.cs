using System;
using System.Collections.Generic;
using PReq.Enum;

namespace PReq.Models
{
    public class AnalyzeRequestModel
    {
        public string Url { get; set; }
        public HttpType HttpType { get; set; }
        public List<RequestHeaderModel> Headers { get; set; }
        public string Content { get; set; }
        public string ContentType { get; set; }
    }
}