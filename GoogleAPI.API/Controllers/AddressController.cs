
using GoogleAPI.Domain.Models.NEBIM.Address;
using GoogleAPI.Persistance.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoogleAPI.API.Controllers
{
    [Route("api/addresses")]
    [ApiController]
    //[Authorize(AuthenticationSchemes ="Admin")]
    public class AddressController : ControllerBase
    {
        private readonly GooleAPIDbContext _context;


        public AddressController(GooleAPIDbContext context)
        {
            _context = context;
        }

        [HttpGet("get-countries")]
        public async Task<ActionResult> GetCities()
        {
            List<Address_VM> cityList = _context.Address_VM.FromSqlRaw("msg_GetCountries").ToList();
            return Ok(cityList);


        }

        [HttpGet("get-regions/{upperCode}")]
        public async Task<ActionResult> GetRegions(string upperCode)
        {
            if (String.IsNullOrEmpty(upperCode))
            {
                List<Address_VM> cityList = _context.Address_VM.FromSqlRaw("exec msg_GetStates '' ").ToList();
                return Ok(cityList);
            }
            else
            {
                List<Address_VM> cityList = _context.Address_VM.FromSqlRaw($"exec msg_GetStates '{upperCode}' ").ToList();
                return Ok(cityList);
            }


        }


        [HttpGet("get-cities/{upperCode}")]
        public async Task<ActionResult> GetCities(string upperCode)
        {
            if (String.IsNullOrEmpty(upperCode))
            {
                List<Address_VM> cityList = _context.Address_VM.FromSqlRaw("exec msg_GetCities '' ").ToList();
                return Ok(cityList);
            }
            else
            {
                List<Address_VM> cityList = _context.Address_VM.FromSqlRaw($"exec msg_GetCities '{upperCode}' ").ToList();
                return Ok(cityList);
            }


        }


        [HttpGet("get-districts/{upperCode}")]
        public async Task<ActionResult> GetDistricts(string upperCode)
        {
            if (String.IsNullOrEmpty(upperCode))
            {
                List<Address_VM> cityList = _context.Address_VM.FromSqlRaw("exec msg_GetDistricts '' ").ToList();
                return Ok(cityList);
            }
            else
            {
                List<Address_VM> cityList = _context.Address_VM.FromSqlRaw($"exec msg_GetDistricts '{upperCode}' ").ToList();
                return Ok(cityList);
            }


        }

        [HttpGet("get-tax-offices/{upperCode}")]
        public async Task<ActionResult> GetTaxOffices(string upperCode)
        {
            if (String.IsNullOrEmpty(upperCode))
            {
                List<Address_VM> cityList = _context.Address_VM.FromSqlRaw("exec msg_GetTaxOffice '' ").ToList();
                return Ok(cityList);
            }
            else
            {
                List<Address_VM> cityList = _context.Address_VM.FromSqlRaw($"exec msg_GetTaxOffice '{upperCode}' ").ToList();
                return Ok(cityList);
            }


        }



    }

    }

