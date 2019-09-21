using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PReq.Enum;
using PReq.Models;

namespace PReq.Utility
{
    public interface IOperationsUtility
    {
        Task ProcessPipelinesAsync(AnalyzeRequestModel request,
                                           int numberOfPipeline,
                                           int requestPerPipeline);
    }

    public class OperationsUtility : IOperationsUtility
    {
        public async Task ProcessPipelinesAsync(AnalyzeRequestModel request,
                                                        int numberOfPipeline,
                                                        int requestPerPipeline)
        {
            var sendRequestTasks = new List<Task<List<AnalysisResultModel>>>();

            var analysisResults = new List<AnalysisResultModel>();

            for (int pipeline = 0; pipeline < numberOfPipeline; pipeline++)
            {
                sendRequestTasks.Add(ProcessPipelineAsync(request, requestPerPipeline));
            }

            foreach (var sendRequestTask in sendRequestTasks)
            {
                var requestLogStringBuilder = await sendRequestTask.ConfigureAwait(false);
                analysisResults.AddRange(requestLogStringBuilder);
            }
        }

        private async Task<List<AnalysisResultModel>> ProcessPipelineAsync(AnalyzeRequestModel request,
                                                                           int requestPerPipeline)
        {
            var pipelineId = Guid.NewGuid().ToString();
            var analysisResults = new List<AnalysisResultModel>();

            for (int requestCount = 0; requestCount < requestPerPipeline; requestCount++)
            {
                var requestResult = await ProcessRequestAsync(request).ConfigureAwait(false);
                requestResult.PipelineId = pipelineId;
                analysisResults.Add(requestResult);
            }

            return analysisResults;
        }

        private async Task<AnalysisResultModel> ProcessRequestAsync(AnalyzeRequestModel request)
        {
            var requestId = Guid.NewGuid().ToString();
            var startAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var client = new HttpClient();
            foreach (var header in request.Headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            HttpResponseMessage response = null;
            StringContent httpContent = null;

            if (!string.IsNullOrEmpty(request.Content) && !string.IsNullOrEmpty(request.ContentType))
                httpContent = new StringContent(request.Content, Encoding.UTF8, request.ContentType);

            switch (request.HttpType)
            {
                case HttpType.GET:
                    response = await client.GetAsync(request.Url).ConfigureAwait(false);
                    break;
                case HttpType.POST:
                    response = await client.PostAsync(request.Url, httpContent).ConfigureAwait(false);
                    break;
                case HttpType.PUT:
                    response = await client.PutAsync(request.Url, httpContent).ConfigureAwait(false);
                    break;
                case HttpType.DELETE:
                    response = await client.DeleteAsync(request.Url).ConfigureAwait(false);
                    break;
            }

            var endAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            return new AnalysisResultModel
            {
                IsSucess = response.IsSuccessStatusCode,
                RequestId = requestId,
                StartAt = DateTimeOffset.FromUnixTimeMilliseconds(startAt),
                EndAt = DateTimeOffset.FromUnixTimeMilliseconds(endAt)
            };
        }
    }
}
