
using GoogleAPI.Domain.Entities;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace GoogleAPI.API.Controllers
{
    [Route("api/Categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly GooleAPIDbContext _context;
        private readonly IMainCategoryWriteRepository _cw;
        private readonly IMainCategoryReadRepository _cr;

        private readonly string ErrorTextBase = "İstek Sırasında Hata Oluştu: ";
        public CategoriesController(
           GooleAPIDbContext context, IMainCategoryWriteRepository cw, IMainCategoryReadRepository cr
        )
        {
            _cw = cw;
            _cr = cr;
            _context = context;
        }
         // Tüm kategorileri getiren endpoint
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MainCategory>>> GetCategories()
        {
            try
            {
                List<MainCategory> categories = _cr.GetAll().AsEnumerable().ToList();
                return categories;
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        // Kategori ekleyen endpoint
        [HttpPost]
        public async Task<ActionResult<MainCategory>> AddCategory(string categoryName)
        {
            try
            {
                MainCategory category = new();
                category.Description = categoryName;
                await _cw.AddAsync(category);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        // Kategori ID'ye göre silen endpoint
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _cr.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                bool response = _cw.Remove(category);
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