
using GoogleAPI.Domain.Models.NEBIM;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Domain.Models.NEBIM.Shelf;
using GoogleAPI.Persistance.Concretes;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GoogleAPI.API.Controllers
{
    [Route("api/Products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly GooleAPIDbContext _context;
        private readonly string ErrorTextBase = "İstek Sırasında Hata Oluştu: ";
        private readonly IOrderService _orderService;
        private readonly ILogService _ls;
        private readonly IGeneralService _gs;
        private readonly IProductService _ps;
        public ProductsController(
           GooleAPIDbContext context, IOrderService orderService,ILogService logService,IGeneralService gs,IProductService ps
        )
        {
            _ls = logService;
            _orderService = orderService;
            _context = context;
            _gs = gs;
            _ps = ps;
        }
        [HttpPost("SearchProduct")]
        public async Task<IActionResult> SearchProduct(BarcodeSearch_RM model)
        {
            try
            {
                if (model.Barcode != null)
                {
                    List<ProductList_VM> products = await _context.ProductListModel.FromSqlRaw($"exec Get_MSRafSearch '{model.Barcode}'").ToListAsync();

                    return Ok(products);
                }
                else
                {
                    return BadRequest("Barkod Boş Geldi");
                }





            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("AddQr")]
        public async Task<IActionResult> AddQr(QrCode model)
        {
            try
            {
                string query = $"exec InsertQrCode '{model.Barcode}','{model.ShelfNo}','{model.Quantity}','{model.Id}','{model.BatchCode}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'";

                int qrCode = _context.Database.ExecuteSqlRaw(query);
                if (qrCode == -1)
                {
                    return Ok(true);
                }
                else
                {
                    return BadRequest(false);
                }


            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("GenerateBarcode_A")]
        public async Task<IActionResult> GenerateBarcode_A(BarcodeModel_A model)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            
            try
            {
                List<BarcodeModel_A> list = new List<BarcodeModel_A>();
                list.Add(model);
                string page = await _ps.GenerateBarcode_A(list);
                BarcodeModelResponse barcodeModelResponse = new BarcodeModelResponse();
                barcodeModelResponse.Page = page;   
                return Ok(barcodeModelResponse);
            }
            catch (Exception ex)
            {
                await _ls.LogOrderError($"{HttpContext.Request.Path}", $"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }


        [HttpPost("QrControl")]
        public async Task<IActionResult> QrControl(QrControlCommandModel qrControlCommandModel)
        {
            try
            {
                string query = $"select * from usp_QRKontrol3('{qrControlCommandModel.Qr}','1')";

                QrControlModel? model = _context.QrControlModel?.FromSqlRaw(query).ToList().First();
                
                if (model!=null)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest(false);
                }


            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }




    }
}
