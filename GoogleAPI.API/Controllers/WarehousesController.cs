using GoogleAPI.Domain.Models;
using GoogleAPI.Domain.Models.NEBIM;
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
       // private IOrderService _orderService;
        public WarehousesController(
           GooleAPIDbContext context,
           IOrderService orderService
        )
        {
            //_orderService = orderService;
            _context = context;
        }



        //GetOperationWarehousue
        [HttpGet("GetBarcodeDetail/{qrCode}")]
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



        [HttpGet("GetOfficeModel")]
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
        private readonly string IpAdresi = "http://192.168.2.36:7676";

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
                // Console.WriteLine(responseBody);
                return sessionId;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP isteği başarısız: {ex.Message}");
                return null;
            }
        }

        [HttpPost("TransferProducts")]
        public async Task<IActionResult> TransferProducts(WarehouseFormModel item)
        {
            try
            {

                NebimWarehouseTransferModel jsonModel = new NebimWarehouseTransferModel()
                {
                    ModelType = 109,
                    InnerNumber = "",
                    OfficeCode = item.Office,
                    OperationDate = DateTime.Now.ToString(),
                    StoreCode = "",
                    ToOfficeCode = item.OfficeTo,
                    ToStoreCode = "",
                    ToWarehouseCode = item.WarehouseTo,
                    WarehouseCode = item.Warehouse,
                    CompanyCode = 1,
                    InnerProcessType = 4,
                    IsCompleted = true,
                    IsInnerOrderBase = false,
                    IsLocked = false,
                    IsPostingJournal = true,
                    IsPrinted = false,
                    IsReturn = false,
                    IsTransferApproved = true,
                    Lines = new List<NebimWarehouseTransferLineModel>()
                    {
                        new NebimWarehouseTransferLineModel()
                        {
                            ItemTypeCode = 1,
                            UsedBarcode = item.Barcode,
                            ItemCode = item.ItemCode,
                            ColorCode = item.ColorCode,
                            ItemDim1Code = item.ItemDim1Code,
                            BatchCode =  item.Party,
                            Qty1 = 10,
                            ITAttributes = new List<NebimWarehouseTransferITAttributeModel>()
                            {
                                new NebimWarehouseTransferITAttributeModel()
                                {
                                    AttributeCode = item.ShelfNo,
                                    AttributeTypeCode = 1
                                },

                            }
                        }
                    }

                };
                var json = JsonConvert.SerializeObject(jsonModel);
                string sessionID = await ConnectIntegrator();

                using (var httpClient = new HttpClient())
                {
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync($"http://192.168.2.36:7676/(S({sessionID}))/IntegratorService/post?", content);

                    var result = await response.Content.ReadAsStringAsync();

                    JObject jsonResponse = JObject.Parse(result);
                }

                return Ok();

            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
        [HttpGet("GetWarehouseModel/{officeCode}")]
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
                List<WarehosueOperationListModel> saleOrderModel = _context.ztWarehosueOperationListModel.FromSqlRaw("select * from ztTransferOnayla where IsCompleted = 0").AsEnumerable().ToList();

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
                List<WarehosueOperationDetailModel> saleOrderModel = _context.ztWarehosueOperationDetail.FromSqlRaw($"exec   [Usp_GETTransferOnayla]'{innerNumber}'").AsEnumerable().ToList();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }


        [HttpPost("SendNebımToTransferProduct")]
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

        [HttpPost("ConfirmOperation")]
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



        [HttpGet("GetOperationWarehousue/{innerNumber}")]
        public IActionResult GetOperationWarehousue(string innerNumber)
        {

            try
            {
                List<BarcodeModel> barcodeModels = _context.BarcodeModels.FromSqlRaw($"usp_QRKontrolSorgula '{innerNumber}'").AsEnumerable().ToList();
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                return Ok(barcodeModels);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }






    }
}
