
using GoogleAPI.Domain.Models.NEBIM;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Xml;

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
           GooleAPIDbContext context, IOrderService orderService, ILogService logService, IGeneralService gs, IProductService ps
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
                    List<ProductList_VM> products = await _context.ProductListModel.FromSqlRaw($" [dbo].[Get_MSRAFSearch2] '{model.Barcode}'").ToListAsync();

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
                List<QrCode>? qrCodes = await _context.ztQrCodes.Where(q => q.UniqueId == model.UniqueId).ToListAsync();
                if (qrCodes.Count > 0)
                {
                    if (model.ShelfNo != qrCodes.First().ShelfNo)
                    {
                        return BadRequest("Bir Kutu Birden Fazla Rafta Bulunamaz");
                    }

                    QrCode? qrCode = qrCodes.FirstOrDefault(q => q.BatchCode == model.BatchCode && q.Quantity == model.Quantity);
                    if (qrCode != null)
                    {
                        return BadRequest("Bu Kutu İçindeki Ürün Aynı Parti Ve Aynı Miktar İle Daha Önce Eklendi");
                    }


                    var response = await _context.ztQrCodes.AddAsync(model);
                    await _context.SaveChangesAsync();
                    return Ok(true);
                }
                else
                {
                    var response = await _context.ztQrCodes.AddAsync(model);
                    await _context.SaveChangesAsync();
                    return Ok(true);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("get-qr/{id}")]
        public async Task<ActionResult<List<QrCode>>> AddQr(string id)
        {
            try
            {
                Guid idAsGuid = Guid.Parse(id);

                List<QrCode> response = await _context.ztQrCodes.Where(q => q.UniqueId == idAsGuid).ToListAsync();

                return Ok(response);



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
                string query = $"select * from usp_QRKontrol3('{qrControlCommandModel.Qr}','1')"; //19.02 kadir

                QrControlModel? model = _context.QrControlModel?.FromSqlRaw(query).ToList().First();

                if (model != null)
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

        [HttpPost("qr-operation")]
        public async Task<IActionResult> QrOperation(QrOperationModel model)
        {
            try
            {
                string query = $"Get_MSRAFBoxQR '{model.QrBarcode}','{model.ShelfNo}','{model.Barcode}','{model.BatchCode}','{model.Qty}','{model.ProcessCode}','{model.IsReturn}'";

               QrOperationResponse? response = _context.QrOperationResponse?.FromSqlRaw(query).ToList().First();
                if (response != null)
                {
                    if (response.Status == 1)
                    {
                        return Ok(response.Status);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                else
                {
                    return Ok(false);
                }
                


            }
            catch (Exception ex)
            {

                return Ok(false);
            }
        }

     

        [HttpPost("qr-operation2")]
        public async Task<IActionResult> QrOperation2(QrOperationModel2 model)
        {
            try
            {
                string query = $"Get_MSRAFBoxQR2 '{model.QrBarcode}','{model.ShelfNo}','{model.Barcode}','{model.BatchCode}','{model.Qty}','{model.ProcessCode}','{model.IsReturn}','{model.ToWarehouseCode}'";

                QrOperationResponse? response = _context.QrOperationResponse?.FromSqlRaw(query).ToList().First();
                if (response != null)
                {
                    if (response.Status == 1)
                    {
                        return Ok(response.Status);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                else
                {
                    return Ok(false);
                }



            }
            catch (Exception ex)
            {

                return Ok(false);
            }
        }






    }
}
