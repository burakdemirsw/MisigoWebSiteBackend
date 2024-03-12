using GooleAPI.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoogleAPI.API.Controllers
{
    [Route("api/direct-request")]
    [ApiController]
    public class DirectRequestsController : ControllerBase
    {
        private IGeneralService _generalService;

        public DirectRequestsController(IGeneralService generalService)
        {
            _generalService = generalService;
        }

        [HttpGet("{request}")]
        public async Task<ActionResult<string>> SendNebimRequest(string request )
        {
           var response = await  _generalService.PostNebimAsync(request,"");

            return response;
        }
    }
}
