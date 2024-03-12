using GoogleAPI.Domain.Models.Cargo.Mng.Request;
using GoogleAPI.Domain.Models.Cargo.Mng.Response;
using GoogleAPI.Persistance.Concreates;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoogleAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CargosController : ControllerBase
    {
        private readonly IMNGCargoService _mngCargoService;
        public CargosController( IMNGCargoService mngCargoService)
        {
         
            _mngCargoService = mngCargoService;
           
        }

        [HttpPost("create-cargo")]
        public async Task<IActionResult> CreateCargo(CreatePackage_MNG_Request request)
        {        
            var response = await _mngCargoService.CreateCargo(request);

            return Ok(response);
        }

        [HttpGet("getToken")]
        public async Task<IActionResult> GetToken( )
        {
            var response = await _mngCargoService.CreateToken();

            return Ok(response);
        }

        [HttpPost("print-barcode")]
        public async Task<IActionResult> PrintBarcode(CreateBarcode_MNG_Request request)
        {

            var _response = await _mngCargoService.CreateBarcode(request);

            return Ok(_response);
        }
    }


}

