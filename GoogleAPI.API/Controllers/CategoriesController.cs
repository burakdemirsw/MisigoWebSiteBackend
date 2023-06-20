using GoogleAPI.Domain.Models;
using GoogleAPI.Persistance.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoogleAPI.API.Controllers
{
    [Route("api/Categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly GooleAPIDbContext _context;
        private readonly string ErrorTextBase = "İstek Sırasında Hata Oluştu: ";
        public CategoriesController(
           GooleAPIDbContext context
        )
        {

            _context = context;
        }

        [HttpGet]
        public IActionResult GetSaleOrders( )
        {
            try
            {
                List<CategoryModel> saleOrderModel = _context.CategoryModels.FromSqlRaw("exec usp_GetCategories").AsEnumerable().ToList();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }

        }
    }
}