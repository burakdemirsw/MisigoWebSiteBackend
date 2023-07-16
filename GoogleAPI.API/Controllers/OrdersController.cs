using GoogleAPI.Domain.Models;
using GoogleAPI.Domain.Models.NEBIM;
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
    [Route("api/Order")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly GooleAPIDbContext _context;
        private readonly string ErrorTextBase = "İstek Sırasında Hata Oluştu: ";
        private IOrderService _orderService;
        public OrdersController(
           GooleAPIDbContext context,
           IOrderService orderService
        )
        {
            _orderService =orderService ;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetSaleOrders( )
        {
            try
            {
                List<SaleOrderModel> saleOrderModel = _context.SaleOrderModels.FromSqlRaw("exec Get_ZTMSSatisSiparis").AsEnumerable().ToList();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("OrderBillings/{orderNo}")]
        public IActionResult GetOrderBillingModels(string orderNo)
        {
            try
            {
               
                List<OrderBillingModel> saleOrderModel = _context.ztOrderBillingModel.FromSqlRaw($" ").AsEnumerable().ToList();
                List<OrderBillingListModel> OrderBillingListModels = new List<OrderBillingListModel>();
                foreach (var item in saleOrderModel)
                {
                    OrderBillingListModel orderBillingListModel = new OrderBillingListModel();
                    orderBillingListModel.ItemBillingModels = JsonConvert.DeserializeObject<List<ItemBillingModel>>(item.Json);
                    orderBillingListModel.TotalValue = item.TotalValue;
                    OrderBillingListModels.Add(orderBillingListModel);

                }
                return Ok(OrderBillingListModels);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }


        [HttpGet("GenerateQRCode")]
        public  IActionResult GenerateQRCode()
        {
            try
            {
                Guid guid = Guid.NewGuid(); 
                Image qrCodeImage =  _orderService.QrCode(guid);
                qrCodeImage.Save(@$"C:\\code\{guid.ToString()}.png");
                return Ok(qrCodeImage);
            }
            catch (Exception ex)
            {
                return BadRequest("QR kodu oluşturulamadı: " + ex.Message);
            }
        }




        [HttpGet("{id}")]
        public IActionResult GetSaleOrdersById(string id  )
        {
            try
            {
                List<SaleOrderModel> saleOrderModel = _context.SaleOrderModels.FromSqlRaw($"exec Get_ZTMSSatisSiparisById '{id.Split(' ')[0]}' ").AsEnumerable().ToList();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("Add")]
        public IActionResult AddSaleBarcode(BarcodeAddModel model)
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

        [HttpGet("GetBarcodeDetail/{qrCode}")]
        public IActionResult GetBarcodeDetail(string qrCode ) 
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
        public IActionResult GetOfficeModel()
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
                VM_HttpConnectionRequest session =
                    JsonConvert.DeserializeObject<VM_HttpConnectionRequest>(responseBody);

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
                        CompanyCode = 2,
                        InnerProcessType = 3,
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
                            ItemTypeCode = 4,
                            UsedBarcode = item.Barcode,
                            ItemCode = item.ItemCode,
                            ColorCode = item.ColorCode,
                            ItemDim1Code = item.ItemDim1Code,
                            BatchCode = "",
                            Qty1 = 10,
                            ITAttributes = new List<NebimWarehouseTransferITAttributeModel>()
                            {
                                new NebimWarehouseTransferITAttributeModel()
                                {
                                    AttributeCode = item.ShelfNo,
                                    AttributeTypeCode = 5
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





        [HttpGet("GetProductsOfOrders")]
        public IActionResult GetProductsOfOrders()
        {

            try
            {
                List<ProductOfOrderModel> productModels = _context.ztProductOfOrderModel.FromSqlRaw($"exec Get_MSSiparisTopla").AsEnumerable().ToList();
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                return Ok(productModels);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetOrderSaleDetail/{orderNumber}")]
        public IActionResult GetOrderSaleDetail( string orderNumber)
        {


            try
            {
                List<OrderSaleDetail> orderSaleDetails = _context.OrderSaleDetails.FromSqlRaw($"Get_ZTMSSatisSiparisDetay '{orderNumber}'").AsEnumerable().ToList();
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                return Ok(orderSaleDetails);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
           
        }




    }
}
