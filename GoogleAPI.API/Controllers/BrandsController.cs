
using GoogleAPI.Domain.Entities;
using GoogleAPI.Domain.Models;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoogleAPI.API.Controllers
{
    [Route("api/Brands")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly GooleAPIDbContext _context;
        private readonly IBrandWriteRepository _cw;
        private readonly IBrandReadRepository _cr;

        private readonly string ErrorTextBase = "İstek Sırasında Hata Oluştu: ";
        public BrandsController(
           GooleAPIDbContext context, IBrandWriteRepository cw, IBrandReadRepository cr
        )
        {
            _cw = cw;
            _cr = cr;
            _context = context;
        }


        [HttpPost("GetBrands")]
        public async Task<ActionResult<IEnumerable<Brand_VM>>> GetCategories( string? brandId)
        {
            try
            {
                var query = $"exec GetBrandById '{brandId}'";
                List<Brand_VM> brands = _context.Brand_VM.FromSqlRaw(query).AsEnumerable().ToList();
                return brands;
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }



        [HttpPost]
        public async Task<ActionResult<Brand>> AddBrand(string BrandName)
        {
            try
            {
                Brand Brand = new();
                Brand.Description = BrandName;
                await _cw.AddAsync(Brand);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            try
            {
                var Brand = await _cr.GetByIdAsync(id);
                if (Brand == null)
                {
                    return NotFound();
                }

                bool response = _cw.Remove(Brand);
                if (response)
                {
                    return Ok();
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }


    }
}