using GoogleAPI.Domain.Models;
using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.NEBIM;
using GoogleAPI.Domain.Models.NEBIM.Invoice;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Domain.Models.NEBIM.Request;
using GoogleAPI.Domain.Models.NEBIM.Warehouse;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Text;

namespace GoogleAPI.API.Controllers
{
    [Route("api/Warehouse")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly GooleAPIDbContext _context;
        private readonly string ErrorTextBase = "İstek Sırasında Hata Oluştu: ";
        private readonly string IpAdresi = "http://192.168.2.36:7676";
        private IOrderService _orderService;
        private ILogService _ls;
        // private IOrderService _orderService;
        public WarehousesController(
          GooleAPIDbContext context,

          IOrderService orderService, ILogService ls
        )
        {
            _orderService = orderService;
            _ls = ls;
            _context = context;
        }

        //Ürün Controller'ına taşınması lazım
        [HttpGet("GetBarcodeDetail/{qrCode}")] //?
        public async Task<IActionResult> GetBarcodeDetail(string qrCode)
        {

            try
            {
                List<BarcodeModel>? barcodeModels = await _context.BarcodeModels?.FromSqlRaw($"usp_QRKontrolSorgula '{qrCode}'").ToListAsync();
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                return Ok(barcodeModels);
            }
            catch (Exception ex)
            {

                string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
                string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}", requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
        //Nebim Servislerine taşınması lazım

        private async Task<string> ConnectIntegrator( )
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(
                  IpAdresi + "/IntegratorService/Connect"
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                HttpConnectionRequestModel? session =
                  JsonConvert.DeserializeObject<HttpConnectionRequestModel>(responseBody);

                string sessionId = session.SessionId;
                return sessionId;
            }
            catch (HttpRequestException ex)
            {
                //Console.WriteLine($"HTTP isteği başarısız: {ex.Message}");
                return null;
            }
        }

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
                string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
                string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}", requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
        //TransferProducts/{orderNo}
        [HttpGet("TransferProducts/{orderNo}")]
        public async Task<IActionResult> TransferProducts(string orderNo)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            try
            {

                TransferData? transferData = new TransferData();

                transferData = _context.TransferData?.FromSqlRaw($"exec usp_GetOrderForInvoiceToplu_WT '{orderNo}'").AsEnumerable().First();
                if (transferData != null)
                {
                    List<TransferItem>? transferItems = JsonConvert.DeserializeObject<List<TransferItem>>(transferData.Lines);
                    TransferDataModel transferDataModel = new TransferDataModel
                    {
                        ModelType = transferData.ModelType,
                        InnerNumber = transferData.InnerNumber,
                        OfficeCode = transferData.OfficeCode,
                        OperationDate = transferData.OperationDate,
                        StoreCode = transferData.StoreCode,
                        ToOfficeCode = transferData.ToOfficeCode,
                        ToStoreCode = transferData.ToStoreCode,
                        ToWarehouseCode = transferData.ToWarehouseCode,
                        WarehouseCode = transferData.WarehouseCode,
                        CompanyCode = transferData.CompanyCode,
                        InnerProcessType = transferData.InnerProcessType,
                        IsCompleted = Convert.ToBoolean(transferData.IsCompleted),
                        IsInnerOrderBase = Convert.ToBoolean(transferData.IsInnerOrderBase),
                        IsLocked = Convert.ToBoolean(transferData.IsLocked),
                        IsPostingJournal = Convert.ToBoolean(transferData.IsPostingJournal),
                        IsPrinted = Convert.ToBoolean(transferData.IsPrinted),
                        IsReturn = Convert.ToBoolean(transferData.IsReturn),
                        IsTransferApproved = Convert.ToBoolean(transferData.IsTransferApproved),
                        Lines = transferItems
                    };

                    var json = JsonConvert.SerializeObject(transferDataModel);

                    using (var httpClient = new HttpClient())
                    {
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        string sessionID = await ConnectIntegrator();
                        if (sessionID != null)
                        {
                            var response = await httpClient.PostAsync($"http://192.168.2.36:7676/(S({sessionID}))/IntegratorService/post?", content);

                            var result = await response.Content.ReadAsStringAsync();
                            JObject jsonResponse = JObject.Parse(result);

                            ErrorResponseModel? erm = JsonConvert.DeserializeObject<ErrorResponseModel>(result);

                            if (erm != null)
                            {
                                if (erm.StatusCode == 400)
                                {
                                    await _ls.LogWarehouseError($"{content}", "Transfer Başarısız", erm.ExceptionMessage,requestUrl);

                                    throw new Exception(erm.ExceptionMessage);
                                }
                            }
                           
                            await _ls.LogWarehouseSuccess( $"{methodName} Başarılı", HttpContext.Request.Path); return Ok(true);

                        }
                        else
                        {
                            await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"", requestUrl);
                            return BadRequest("SessionId Alınamadı");
                        }

                    }

                }
                else
                {
                    await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"", requestUrl);
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}", requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetWarehouseModel/{officeCode}")] //verilen ofis koduna göre depoları çeker
        public async Task<IActionResult> WarehouseModel(string officeCode)
        {
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;

            try
            {
                List<WarehouseOfficeModel> warehouseModels = await _context.ztWarehouseModel.FromSqlRaw($"exec [usp_MTOfisDepo] '{officeCode}'").ToListAsync();
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                return Ok(warehouseModels);
            }
            catch (Exception ex)
            {

                string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetWarehosueOperationList")]
        public async Task<IActionResult> GetWarehosueOperationList( )
        {
            try
            {
                List<WarehosueOperationListModel> saleOrderModel = await _context.ztWarehosueOperationListModel.FromSqlRaw("select TOP 100* from ztTransferOnayla where IsCompleted = 0 and WarehouseCode in ('MD','UD') and ToWarehouseCode in ('MD','UD') Order by InnerNumber desc").ToListAsync();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
                string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;

                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}", requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("DeleteCountById/{id}")]
        public async Task<ActionResult> DeleteCountById(string id)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            try
            {
                var affectedRow = _context.Database.ExecuteSqlRaw($"delete from ZTMSRAFSAYIM3 where OrderNumber = '{id}' ");
                if (affectedRow > 0)
                {
                    await _ls.LogWarehouseSuccess( $"{methodName} Başarılı", HttpContext.Request.Path); return Ok(true);
                }
                else
                {
                    string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;

                    await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"", requestUrl);
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;

                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}", requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("DeleteWarehouseTransferById/{id}")]
        public async Task<IActionResult> DeleteWarehouseTransferByOrderNumber(string id)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            try
            {
                var affectedRow = _context.Database.ExecuteSqlRaw($"delete from ZTMSRAFSAYIM6 where OrderNumber = '{id}' ");
                if (affectedRow > 0)
                {
                    await _ls.LogWarehouseSuccess( $"{methodName} Başarılı", HttpContext.Request.Path); return Ok(true);
                }
                else
                {
                  

                    await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"", requestUrl);
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
           
                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("GetWarehosueTransferList")]
        public async Task<IActionResult> GetWarehosueTransferList(WarehouseTransferListFilterModel model)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            try
            {
                // Initialize the base query
                string query = "SELECT TOP 100 MAX(ItemDate) as OperationDate, SUM(Quantity) as Quantity, OrderNumber, WarehouseCode, ToWarehouseCode FROM ZTMSRAFSAYIM6";

                // Initialize filter clauses
                List<string> filterClauses = new List<string>();

                // Add filters based on model properties
                if (!string.IsNullOrEmpty(model.OrderNumber))
                {
                    filterClauses.Add($"OrderNumber = '{model.OrderNumber}'");
                }
                if (!string.IsNullOrEmpty(model.WarehouseCode))
                {
                    filterClauses.Add($"WarehouseCode = '{model.WarehouseCode}'");
                }
                if (!string.IsNullOrEmpty(model.ToWareHouseCode))
                {
                    filterClauses.Add($"TOWareHouseCode = '{model.ToWareHouseCode}'");
                }
                if (model.OperationStartDate != null)
                {
                    filterClauses.Add($"ItemDate >= '{model.OperationStartDate:yyyy-MM-dd}'");
                }
                if (model.OperationEndDate != null)
                {
                    filterClauses.Add($"ItemDate <= '{model.OperationEndDate:yyyy-MM-dd}'");
                }

                // Combine filter clauses
                if (filterClauses.Count > 0)
                {
                    string filterConditions = string.Join(" AND ", filterClauses);
                    query += " WHERE " + filterConditions;
                }

                // Complete the query
                query += " GROUP BY OrderNumber, WarehouseCode, TOWareHouseCode";
                query += " ORDER BY OrderNumber DESC;";

                // Execute the query and retrieve results
                List<WarehosueTransferListModel> warehouseTransferModel = await _context.WarehosueTransferListModel.FromSqlRaw(query).ToListAsync();

                return Ok(warehouseTransferModel);
            }
            catch (Exception ex)
            {

                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}", requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("GetWarehosueOperationListByFilter")]
        public async Task<IActionResult> GetWarehosueOperationListByFilter(WarehouseOperationListFilterModel model)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            try
            {
                // Initialize the base query
                string query = "SELECT * FROM ztTransferOnayla WHERE IsCompleted = 1 or  IsCompleted = 0";

                // Initialize filter clauses
                List<string> filterClauses = new List<string>();

                // Add filters based on model properties
                if (!string.IsNullOrEmpty(model.InnerNumber))
                {
                    filterClauses.Add($"InnerNumber = '{model.InnerNumber}'");
                }
                if (model.StartDate != null)
                {
                    filterClauses.Add($"OperationDate >= '{model.StartDate:yyyy-MM-dd}'");
                }
                if (model.EndDate != null)
                {
                    filterClauses.Add($"OperationDate <= '{model.EndDate:yyyy-MM-dd}'");
                }

                // Combine filter clauses
                if (filterClauses.Count > 0)
                {
                    string filterConditions = string.Join(" AND ", filterClauses);
                    query += " AND " + filterConditions;
                }

                // Complete the query
                query += " ORDER BY InnerNumber DESC;";

                // Execute the query and retrieve results
                List<WarehosueOperationListModel> saleOrderModel = await _context.ztWarehosueOperationListModel.FromSqlRaw(query).ToListAsync();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {
  
                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}", requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetWarehouseOperationDetail/{innerNumber}")]
        public async Task<IActionResult> GetWarehosueOperationDetail(string innerNumber)
        {
            try
            {
                List<ProductOfOrderModel> saleOrderModel = await _context.ztProductOfOrderModel.FromSqlRaw($"exec   [Usp_GETTransferOnayla]'{innerNumber}'").ToListAsync();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
                string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}", requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("Transfer")] //yapılan depo işlemi işlem numarasına göre direkt transfer eder
        public async Task<ActionResult> SendNebımToTransferProduct(WarehouseOperationProductModel model)
        {
  
            try
            {
                string sql = "EXECUTE Usp_PostZtMSRAFSTOK {0}, {1}, {2}, {3}, {4}";
                // string sql2 = $"update ztTransferOnayla set IsCompleted = 1 where InnerNumber = '{model.InnerNumber}'";
                object[] parameters = {
                      model.Barcode,
                      model.BatchCode,
                      model.ShelfNumber,
                      model.Quantity,
                      model.Warehouse
                    };

                int number = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                // int number2 = _context.Database.ExecuteSqlRaw(sql2);

                return Ok(number);
            }
            catch (Exception ex)
            {
                string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
                string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}", requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("ConfirmOperation")] // yapılan depo işlemlerin durumunu günceller 
        public async Task<ActionResult> ConfirmOperation(List<string> InnerNumberList)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            try
            {
                foreach (var item in InnerNumberList)
                {
                    string sql2 = $"update ztTransferOnayla set IsCompleted = 1 where InnerNumber = '{item}'";

                    int number2 = _context.Database.ExecuteSqlRaw(sql2);
                }

                await _ls.LogWarehouseSuccess( $"{methodName} Başarılı", HttpContext.Request.Path); return Ok(true);
            }
            catch (Exception ex)
            {
              
                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }


        [HttpGet("TransferRequestList")]
        public async Task<IActionResult> GetTransferRequestListModel( )
        {
            try
            {
                TransferRequestListModel model = new TransferRequestListModel();



                List<TransferRequestListModel> list = await _context.TransferRequestListModels.FromSqlRaw("[dbo].[Get_MSRAFTRANSFERShelf]").ToListAsync();

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
                List<BarcodeModel> barcodeModels = await _context.BarcodeModels.FromSqlRaw($"usp_QRKontrolSorgula '{innerNumber}'").ToListAsync();
                return Ok(barcodeModels);
            }
            catch (Exception ex)
            {

                string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
                string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                await _ls.LogWarehouseWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}", requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

    }
}