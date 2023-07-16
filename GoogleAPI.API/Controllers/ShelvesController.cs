using GoogleAPI.Domain.Models;
using GoogleAPI.Persistance.Contexts;
using Microsoft.AspNetCore.Http;
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
        public IActionResult GetShelves(string? id)
        {
            try
            {
                if (id != "{id}")
                {
                    List<ShelfModel> saleOrderModel = _context.ztShelves.FromSqlRaw($"select * from ztShelves where Id = '{id}'").AsEnumerable().ToList();

                    return Ok(saleOrderModel);
                }
                else
                {
                    List<ShelfModel> saleOrderModel = _context.ztShelves.FromSqlRaw($"select * from ztShelves ").AsEnumerable().ToList();

                    return Ok(saleOrderModel);
                }
                
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
        [HttpGet("qr/{qrCode}")]
        public IActionResult GetShelvesByQr(string? qrCode)
        {
            try
            {
                if (qrCode != "{qrCode}")
                {
                    List<ShelfModel> saleOrderModel = _context.ztShelves.FromSqlRaw($"select * from ztShelves where QrString = '{qrCode}'").AsEnumerable().ToList();

                    return Ok(saleOrderModel);
                }
                else
                {
                    List<ShelfModel> saleOrderModel = _context.ztShelves.FromSqlRaw($"select * from ztShelves ").AsEnumerable().ToList();

                    return Ok(saleOrderModel);
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("Add")]
        public IActionResult AddShelf(ShelfModel model)
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
