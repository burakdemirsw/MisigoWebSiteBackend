using GoogleAPI.Domain.Models.NEBIM.Shelf;
using GoogleAPI.Persistance.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoogleAPI.API.Controllers
{
    [Route("api/Shelves")]
    [ApiController]
    public class ShelvesController : ControllerBase
    {
        private readonly GooleAPIDbContext _context;
        private readonly string ErrorTextBase = "İstek Sırasında Hata Oluştu: ";
        public ShelvesController(
           GooleAPIDbContext context
        )
        {

            _context = context;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShelves(string? id)
        {
            try
            {
                if (id != "{id}")
                {
                    List<ShelfModel>? saleOrderModel = await _context.ztShelves.FromSqlRaw($"select * from ztShelves where Id = '{id}'").ToListAsync();

                    return Ok(saleOrderModel);
                }
                else
                {
                    List<ShelfModel>? saleOrderModel = await _context.ztShelves.FromSqlRaw($"select * from ztShelves ").ToListAsync();


                    return Ok(saleOrderModel);
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
        [HttpGet("qr/{qrCode}")]
        public async Task<IActionResult> GetShelvesByQr(string? qrCode)
        {
            try
            {
                if (qrCode != "{qrCode}")
                {
                    List<ShelfModel> saleOrderModel = await _context.ztShelves.FromSqlRaw($"select * from ztShelves where QrString = '{qrCode}'").ToListAsync();

                    return Ok(saleOrderModel);
                }
                else
                {
                    List<ShelfModel> saleOrderModel = await _context.ztShelves.FromSqlRaw($"select * from ztShelves ").ToListAsync();

                    return Ok(saleOrderModel);
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddShelf(ShelfModel model)
        {
            try
            {

                var addedEntity = _context.Entry(model);

                addedEntity.State =
                    EntityState
                    .Added;
                _context.SaveChanges();

                return Ok(model);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
    }
}
