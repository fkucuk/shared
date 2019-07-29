using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace CoverletIssueRaiser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public ValuesController(IMessageService messageService)
        {
            _messageService = messageService;
        }




        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }


        // GET api/values
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]SendMessageRequest request)
        {
            await _messageService.SendAsync(request.Message);
            return Ok();
        }



    }
}
