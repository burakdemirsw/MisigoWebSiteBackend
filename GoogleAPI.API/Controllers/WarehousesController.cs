using GoogleAPI.Domain.Models;
using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.NEBIM;
using GoogleAPI.Domain.Models.NEBIM.Invoice;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Product;
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
        // private IOrderService _orderService;
        public WarehousesController(
           GooleAPIDbContext context,
           IOrderService orderService
        )
        {
            //_orderService = orderService;
            _context = context;
        }



        //Ürün Controller'ına taşınması lazım
        [HttpGet("GetBarcodeDetail/{qrCode}")] //?
        public IActionResult GetBarcodeDetail(string qrCode)
        {

            try
            {
                List<BarcodeModel> barcodeModels = _context.BarcodeModels.FromSqlRaw($"usp_QRKontrolSorgula '{qrCode}'").AsEnumerable().ToList();
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                return Ok(barcodeModels);
            }
            catch (Exception ex)
            {

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
                HttpConnectionRequestModel session =
                    JsonConvert.DeserializeObject<HttpConnectionRequestModel>(responseBody);

                string sessionId = session.SessionId;
                return sessionId;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP isteği başarısız: {ex.Message}");
                return null;
            }
        }


        [HttpGet("GetOfficeModel")] //ofiseleri çeker
        public IActionResult GetOfficeModel( )
        {

            try
            {
                List<OfficeModel> officeCodes = _context.ztOfficeModel.FromSqlRaw($"exec usp_MSOfis").AsEnumerable().ToList();
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                return Ok(officeCodes);
            }

            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
      
        [HttpPost("TransferProducts")] //seçilen ürünleri depolar arası transfer eder
        public async Task<IActionResult> TransferProducts(List<WarehouseFormModel> item)
        {
            List<NebimWarehouseTransferLineModel> TransferLineModelList = new List<NebimWarehouseTransferLineModel>();
            foreach (WarehouseFormModel form in item)
            {
                NebimWarehouseTransferLineModel nebimWarehouseTransferLineModel = new NebimWarehouseTransferLineModel()
                {
                    ItemTypeCode = 1,
                    UsedBarcode = form.Barcode,
                    ItemCode = form.ItemCode,
                    ColorCode = form.ColorCode,
                    ItemDim1Code = form.ItemDim1Code,
                    BatchCode = form.Party,
                    Qty1 = form.Inventory,
                    ITAttributes = new List<NebimWarehouseTransferITAttributeModel>()
                            {
                                new NebimWarehouseTransferITAttributeModel()
                                {
                                    AttributeCode = form.ShelfNo,
                                    AttributeTypeCode = 1
                                },

                            }
                };
                TransferLineModelList.Add(nebimWarehouseTransferLineModel);

            }
            try
            {
                NebimWarehouseTransferModel jsonModel = new NebimWarehouseTransferModel()
                {
                    ModelType = 109,
                    InnerNumber = "",
                    OfficeCode = item[0].Office,
                    OperationDate = DateTime.Now.ToString(),
                    StoreCode = "",
                    ToOfficeCode = item[0].OfficeTo,
                    ToStoreCode = "",
                    ToWarehouseCode = item[0].WarehouseTo,
                    WarehouseCode = item[0].Warehouse,
                    CompanyCode = 1,
                    InnerProcessType = 4,
                    IsCompleted = true,
                    IsInnerOrderBase = false,
                    IsLocked = false,
                    IsPostingJournal = true,
                    IsPrinted = false,
                    IsReturn = false,
                    IsTransferApproved = true,
                    Lines = TransferLineModelList

                };
                var json = JsonConvert.SerializeObject(jsonModel);
               

                using (var httpClient = new HttpClient())
                {
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    string sessionID = await ConnectIntegrator();
                    if (sessionID != null)
                    {
                        var response = await httpClient.PostAsync($"http://192.168.2.36:7676/(S({sessionID}))/IntegratorService/post?", content);

                        if (response != null)
                        {
                            var result = await response.Content.ReadAsStringAsync();

                            JObject jsonResponse = JObject.Parse(result);

                            if (jsonResponse != null && (int)jsonResponse["ModelType"] == 0)
                            {
                                    return BadRequest(jsonResponse);
                            }
                            else
                            {
                                return Ok(jsonResponse);
                            }


                        }
                        else
                        {
                            return BadRequest("Response Alınamadı");
                        }

                    }
                    else
                    {
                        return BadRequest("SessionId Alınamadı");
                    }
                    
   
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
        [HttpGet("GetWarehouseModel/{officeCode}")]//verilen ofis koduna göre depoları çeker
        public IActionResult WarehouseModel(string officeCode)
        {

            try
            {
                List<WarehouseOfficeModel> warehouseModels = _context.ztWarehouseModel.FromSqlRaw($"exec [usp_MTOfisDepo] '{officeCode}'").AsEnumerable().ToList();
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                return Ok(warehouseModels);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
        [HttpGet("GetWarehosueOperationList")] 
        public IActionResult GetWarehosueOperationList( )
        {
            try
            {
                List<WarehosueOperationListModel> saleOrderModel = _context.ztWarehosueOperationListModel.FromSqlRaw("select TOP 100* from ztTransferOnayla where IsCompleted = 0 Order by InnerNumber desc").AsEnumerable().ToList();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("GetWarehosueOperationListByFilter")]
        public IActionResult GetWarehosueOperationListByFilter(WarehouseOperationListFilterModel model)
        {
            try
            {
                // Initialize the base query
                string query = "SELECT * FROM ztTransferOnayla WHERE IsCompleted = 0";

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
                List<WarehosueOperationListModel> saleOrderModel = _context.ztWarehosueOperationListModel.FromSqlRaw(query).AsEnumerable().ToList();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }


        [HttpGet("GetWarehouseOperationDetail/{innerNumber}")]
        public IActionResult GetWarehosueOperationDetail(string innerNumber)
        {
            try
            {
                List<ProductOfOrderModel> saleOrderModel = _context.ztProductOfOrderModel.FromSqlRaw($"exec   [Usp_GETTransferOnayla]'{innerNumber}'").AsEnumerable().ToList();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }



        [HttpPost("SendNebımToTransferProduct")]//yapılan depo işlemi işlem numarasına göre direkt transfer eder
        public IActionResult SendNebımToTransferProduct(WarehouseOperationProduct model)
        {
            try
            {
                string sql = "EXECUTE Usp_PostZtMSRAFSTOK {0}, {1}, {2}, {3}, {4}";
                string sql2 = $"update ztTransferOnayla set IsCompleted = 1 where InnerNumber = '{model.InnerNumber}'";
                object[] parameters = { model.Barcode, model.Lot, model.ShelfNumber, model.Quantity, model.Warehouse };

                int number = _context.Database.ExecuteSqlRaw(sql, parameters);

                int number2 = _context.Database.ExecuteSqlRaw(sql2);


                return Ok(number);
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("ConfirmOperation")] // yapılan depo işlemlerin durumunu günceller 
        public IActionResult ConfirmOperation(List<string> InnerNumberList)
        {
            try
            {
                foreach (var item in InnerNumberList)
                {
                    string sql2 = $"update ztTransferOnayla set IsCompleted = 1 where InnerNumber = '{item}'";

                    int number2 = _context.Database.ExecuteSqlRaw(sql2);
                }



                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetOperationWarehousue/{innerNumber}")]//verilen operasyon kodu ile Ürünleri Çeker
        public IActionResult GetOperationWarehousue(string innerNumber)
        {

            try
            {
                List<BarcodeModel> barcodeModels = _context.BarcodeModels.FromSqlRaw($"usp_QRKontrolSorgula '{innerNumber}'").AsEnumerable().ToList();
                return Ok(barcodeModels);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }






    }
}
