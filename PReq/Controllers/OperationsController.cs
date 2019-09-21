using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PReq.Models;
using PReq.Resources;
using PReq.Utility;

namespace PReq.Controllers
{
    [Route("operations")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly IOperationsUtility operationsUtility;

        public OperationsController(IOperationsUtility _operationsUtility)
        {
            operationsUtility = _operationsUtility;
        }

        [HttpPost("requests/process")]
        public async Task<string> ProcessRequests(AnalyzeRequestModel request,
                                                  int numberOfPipeline,
                                                  int requestPerPipeline)
        {
            operationsUtility.ProcessPipelinesAsync(request, numberOfPipeline, requestPerPipeline)
                                          .ConfigureAwait(false);

            return Messages.WeWillSendEmailOnceAnalysisWillBeDone;
        }
    }
}