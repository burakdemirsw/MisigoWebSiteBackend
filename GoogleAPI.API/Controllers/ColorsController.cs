
using GoogleAPI.Domain.Entities;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace GoogleAPI.API.Controllers
{
    [Route("api/Colors")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly GooleAPIDbContext _context;
        private readonly IColorWriteRepository _cw;
        private readonly IColorReadRepository _cr;

        private readonly string ErrorTextBase = "İstek Sırasında Hata Oluştu: ";
        public ColorsController(
           GooleAPIDbContext context, IColorWriteRepository cw, IColorReadRepository cr
        )
        {
            _cw = cw;
            _cr = cr;
            _context = context;
        }


        [HttpPost]
        public async Task<ActionResult<Color>> AddColor(string ColorName)
        {
            try
            {
                Color Color = new();
                Color.Description = ColorName;
                await _cw.AddAsync(Color);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColor(int id)
        {
            try
            {
                var Color = await _cr.GetByIdAsync(id);
                if (Color == null)
                {
                    return NotFound();
                }

                bool response = _cw.Remove(Color);
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