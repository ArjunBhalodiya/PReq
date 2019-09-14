using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PReq.Controllers
{
    [Route("operations")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        [HttpGet("requests/process")]
        public async Task<string> ProcessRequests()
        {
            return await ProcessPipelinesAsync().ConfigureAwait(false);
        }

        private async Task<string> ProcessPipelinesAsync()
        {
            List<Task<StringBuilder>> sendRequestTasks = new List<Task<StringBuilder>>();

            var builder = new StringBuilder();
            builder.AppendLine("Result,PilelineId,RequestId,StartAt,EndAt,TotalTime(ms)");

            for (int pipeline = 0; pipeline < 5; pipeline++)
            {
                sendRequestTasks.Add(ProcessPipelineAsync());
            }

            foreach (var sendRequestTask in sendRequestTasks)
            {
                var requestLogStringBuilder = await sendRequestTask.ConfigureAwait(false);
                builder.Append(requestLogStringBuilder.ToString());
            }

            return builder.ToString();
        }

        private async Task<StringBuilder> ProcessPipelineAsync()
        {
            var pipelineId = Guid.NewGuid().ToString();
            return await ProcessRequestsAsync(pipelineId).ConfigureAwait(false);
        }

        private async Task<StringBuilder> ProcessRequestsAsync(string pipelineId)
        {
            var builder = new StringBuilder();

            for (int requestCount = 0; requestCount < 15; requestCount++)
            {
                var requestId = Guid.NewGuid().ToString();
                var startAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var client = new HttpClient();
                var response = await client.GetAsync("https://ist.homegenius.com/Gateway/v1/public/locations?cityCode=mi").ConfigureAwait(false);

                var endAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                if (response.IsSuccessStatusCode)
                    builder.AppendLine($"SUCCESS,{pipelineId},{requestId},{DateTimeOffset.FromUnixTimeMilliseconds(startAt)},{DateTimeOffset.FromUnixTimeMilliseconds(endAt)},{endAt - startAt}");
                else
                    builder.AppendLine($"FAIL,{pipelineId},{requestId},{DateTimeOffset.FromUnixTimeMilliseconds(startAt)},{DateTimeOffset.FromUnixTimeMilliseconds(endAt)},{endAt - startAt}");
            }

            return builder;
        }
    }
}