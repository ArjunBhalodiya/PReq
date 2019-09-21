using System;

namespace PReq.Models
{
    public class AnalysisResultModel
    {
        public bool IsSucess { get; set; }
        public string PipelineId { get; set; }
        public string RequestId { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
    }
}