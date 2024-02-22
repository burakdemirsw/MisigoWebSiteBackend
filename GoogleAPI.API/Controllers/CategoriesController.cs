using GoogleAPI.Persistance.Contexts;
using Microsoft.AspNetCore.Mvc;

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

        //[HttpGet]
        //public async Task<IActionResult> GetCategories( )
        //{
        //    try
        //    {
        //        List<CategoryModel> saleOrderModel = await _context.ztCategories.FromSqlRaw("exec usp_GetCategories").ToListAsync();

        //        return Ok(saleOrderModel);
        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest(ErrorTextBase + ex.Message);
        //    }

        //}

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetCategoriesById(int id)
        //{
        //    try
        //    {
        //        CategoryModel saleOrderModel = _context.ztCategories.First(o => o.Id == id);
        //        if (saleOrderModel == null)
        //        {
        //            return NotFound();
        //        }

        //        return Ok(saleOrderModel);
        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest(ErrorTextBase + ex.Message);
        //    }

        //}

        //[HttpPost()]
        //public async Task<IActionResult> AddCategory(CategoryModel model)
        //{
        //    try
        //    {

        //        var addedEntity = _context.Entry(model);

        //        addedEntity.State =
        //            EntityState
        //            .Added;
        //        _context.SaveChanges();

        //        return Ok(model);
        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest(ErrorTextBase + ex.Message);
        //    }
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateCategory(int id, CategoryModel model)
        //{
        //    try
        //    {
        //        var category = _context.ztCategories?.Find(id);

        //        if (category == null)
        //        {
        //            return NotFound("İlgili Kategori Databasede Bulunmamaktadır!");
        //        }

        //        category.Description = model.Description;
        //        category.TopCategory = model.TopCategory;
        //        category.SubCategory = model.SubCategory;
        //        category.SubCategory2 = model.SubCategory2;
        //        category.SubCategory3 = model.SubCategory3;
        //        category.SubCategory4 = model.SubCategory4;
        //        category.SubCategory5 = model.SubCategory5;

        //        _context.SaveChanges();

        //        return Ok(category);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ErrorTextBase + ex.Message);
        //    }
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteCategory(int id)
        //{
        //    try
        //    {
        //        var category = _context.ztCategories?.First(c => c.Id == id);
        //        if (category == null)
        //            return NotFound();

        //        _context.ztCategories?.Remove(category);
        //        _context.SaveChanges();

        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ErrorTextBase + ex.Message);
        //    }
        //}
    }
}