using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Domain.Models.NEBIM.Warehouse;
using GoogleAPI.Persistance.Concreates;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection;

namespace GoogleAPI.API.Controllers
{
    [Route("api/Warehouse")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly GooleAPIDbContext _context;
        private readonly string ErrorTextBase = "İstek Sırasında Hata Oluştu: ";
        private readonly IGeneralService _gs;
        private readonly IProductService _ps;
        private readonly ITransferService _ts;
        private readonly ICountService _cs;
        private ILogService _ls;

        public WarehousesController(
          GooleAPIDbContext context,

           ILogService ls, IGeneralService gs,IProductService ps, ITransferService ts, ICountService cs
        )
        {
            _ls = ls;
            _context = context;
            _gs = gs;
            _ps = ps;
            _ts = ts;
            _cs = cs;   
        }

        //Ürün Controller'ına taşınması lazım
        [HttpGet("GetBarcodeDetail/{qrCode}")] //?
        public async Task<IActionResult> GetBarcodeDetail(string qrCode)
        {

            try
            {
                List<BarcodeModel>? barcodeModels = await _ps.GetBarcodeDetail(qrCode);
               
                return Ok(barcodeModels);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
        //Nebim Servislerine taşınması lazım

        [HttpGet("GetOfficeModel")] //ofiseleri çeker
        public async Task<IActionResult> GetOfficeModel( )
        {

            try
            {
                List<OfficeModel> officeCodes = await _context.ztOfficeModel.FromSqlRaw($"exec usp_MSOfis").ToListAsync();
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                return Ok(officeCodes);
            }
            catch (Exception ex)
            {
                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }


        [HttpGet("GetWarehouseModel/{officeCode}")] //verilen ofis koduna göre depoları çeker
        public async Task<IActionResult> GetWarehouseModel(string officeCode)
        {


            try
            {
                List<WarehouseOfficeModel> warehouseModels = await _ts.GetWarehouseModel(officeCode);
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                return Ok(warehouseModels);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetWarehosueOperationList/{status}")]
        public async Task<IActionResult> GetWarehosueOperationList(string status )
        {
            try
            {
                List<WarehosueOperationListModel> saleOrderModel = await _ts.GetWarehosueOperationList(status);

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);


                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }


        [HttpGet("GetWarehosueOperationListByInnerNumber/{innerNumber}")]
        public async Task<ActionResult<WarehosueOperationListModel>> GetWarehosueOperationListByInnerNumber( string innerNumber)
        {
            try
            {
               WarehosueOperationListModel saleOrderModel = await _ts.GetWarehosueOperationListByInnerNumber(innerNumber);

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);


                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("DeleteCountById/{id}")]
        public async Task<ActionResult> DeleteCountById(string id)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            try
            {
                var affectedRow = await _cs.DeleteCountById(id);
                if (affectedRow > 0)
                {
                    await _ls.LogWarehouseSuccess($"{methodName} Başarılı", HttpContext.Request.Path);
                    return Ok(true);
                }
                else
                {


                    await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {


                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("DeleteWarehouseTransferById/{id}")]
        public async Task<IActionResult> DeleteWarehouseTransferByOrderNumber(string id)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {
                var affectedRow = await _ts.DeleteWarehouseTransferByOrderNumber(id);
                if (affectedRow > 0)
                {
                    await _ls.LogWarehouseSuccess($"{methodName} Başarılı", HttpContext.Request.Path);
                    return Ok(true);
                }
                else
                {


                    await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {

                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("GetWarehosueTransferList")]
        public async Task<IActionResult> GetWarehosueTransferList(WarehouseTransferListFilterModel model)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {

                List<WarehosueTransferListModel> warehouseTransferModel = await _ts.GetWarehosueTransferList(model);

                return Ok(warehouseTransferModel);
            }
            catch (Exception ex)
            {

                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("GetWarehosueOperationListByFilter")]
        public async Task<IActionResult> GetWarehosueOperationListByFilter(WarehouseOperationListFilterModel model)
        {

            try
            {

                List<WarehosueOperationListModel> saleOrderModel = await _ts.GetWarehosueOperationListByFilter(model);

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetWarehouseOperationDetail/{innerNumber}")]
        public async Task<IActionResult> GetWarehosueOperationDetail(string innerNumber)
        {
            try
            {
                List<ProductOfOrderModel> saleOrderModel = await _ts.GetWarehosueOperationDetail(innerNumber);

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
        //TransferProducts/{orderNo}
        [HttpGet("TransferProducts/{orderNo}")]
        public async Task<IActionResult> TransferProducts(string orderNo)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {   var response = await _ts.TransferProducts(orderNo);  

                return Ok(response);    
            }
            catch (Exception ex)
            {
                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
        [HttpPost("Transfer")] //yapılan depo işlemi işlem numarasına göre direkt transfer eder
        public async Task<ActionResult> SendNebımToTransferProduct(WarehouseOperationProductModel model)
        {

            try
            {
               

                int number = await _ts.SendNebımToTransferProduct(model);

                // int number2 = _context.Database.ExecuteSqlRaw(sql2);

                return Ok(number);
            }
            catch (Exception ex)
            {
                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
        [HttpPost("ConfirmOperation")] // yapılan depo işlemlerin durumunu günceller 
        public async Task<ActionResult> ConfirmOperation(List<string> InnerNumberList)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {
                var response= await _ts.ConfirmOperation(InnerNumberList);

                await _ls.LogWarehouseSuccess($"{methodName} Başarılı", HttpContext.Request.Path);
                return Ok(response);
            }
            catch (Exception ex)
            {

                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
        [HttpGet("TransferRequestList/{type}")]
        public async Task<IActionResult> GetTransferRequestListModel( string type )
        {
            try
            {                
                TransferRequestListModel model = new TransferRequestListModel();

                List<TransferRequestListModel> list = await _ts.GetTransferRequestListModel(type);

                if (list.Count == 0)
                {
                    return BadRequest("Onaylanacak Ürün Gelmedi");
                }
                else
                {
                    return Ok(list);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet("GetOperationWarehousue/{innerNumber}")] //verilen operasyon kodu ile Ürünleri Çeker
        public async Task<IActionResult> GetOperationWarehousue(string innerNumber)
        {

            try
            {
                List<BarcodeModel> barcodeModels = await  _ts.GetOperationWarehousue(innerNumber);
                return Ok(barcodeModels);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

    }
}