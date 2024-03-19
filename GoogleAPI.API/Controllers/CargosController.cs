using GoogleAPI.Domain.Entities;
using GoogleAPI.Domain.Models.Cargo.Mng.Request;
using GoogleAPI.Domain.Models.Cargo.Mng.Response;
using GoogleAPI.Domain.Models.Cargo.Response;
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
        public CargosController(IMNGCargoService mngCargoService)
        {

            _mngCargoService = mngCargoService;

        }

        [HttpPost("create-cargo")]
        public async Task<IActionResult> CreateCargo(CreatePackage_MNG_RM Order)
        {
            var response = await _mngCargoService.CreateCargo(Order);

            return Ok(response);
        }

        [HttpGet("getToken")]
        public async Task<IActionResult> GetToken( )
        {
            var response = await _mngCargoService.CreateToken();

            return Ok(response);
        }


        [HttpGet("get-package-status/{shipmentId}")]
        public async Task<ActionResult<List<GetPackageStatus_MNG_Response>>> GetPackageStatus(string shipmentId )
        {
            var response = await _mngCargoService.GetPackageStatus(shipmentId);

            return Ok(response);
        }


        [HttpGet("get-shipped-cargos")]
        public async Task<ActionResult<List<CargoBarcode_VM>>> GetShippedCargos( )
        {
            var response = await _mngCargoService.GetShippedCargos();

            return Ok(response);
        }

        [HttpPost("delete-shipped-cargo")]
        public async Task<ActionResult<List<CargoBarcode_VM>>> GetShippedCargo(DeletePackage_MNG_Request request )
        {
            var response = await _mngCargoService.DeleteShippedCargo(request);

            return Ok(response);
        }


        [HttpGet("create-barcode/{referenceId}")]
        public async Task<IActionResult> PrintBarcode(string referenceId)
        {

            var _response = await _mngCargoService.CreateBarcode(referenceId);

            return Ok(_response);
        }
        [HttpGet("print-single-barcode")]

        public async Task<IActionResult> PrintBarcode(PrintSingleBarcode_MNG_Request request)
        {

                var _response = await _mngCargoService.PrintSingleBarcode(request.ZplBarcode);

            return Ok(_response);
        }
    }
    public class PrintSingleBarcode_MNG_Request
    {
        public string? ZplBarcode { get; set; }
    }

}

