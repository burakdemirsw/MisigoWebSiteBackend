using GoogleAPI.Domain.Models.NEBIM;
using GoogleAPI.Domain.Models.NEBIM.Invoice;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Drawing.Printing;
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
            _orderService = orderService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetSaleOrders( ) //çalışıyor
        {
            try
            {
                List<SaleOrderModel> saleOrderModel = _context.SaleOrderModels.FromSqlRaw("exec GET_MSRAFOrderList").AsEnumerable().ToList();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("OrderBillings/{orderNo}")]
        public async Task<IActionResult> GetOrderBillingModels(string orderNo)
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
        //[HttpGet("GenerateQRCode")]
        //public async Task<IActionResult> GenerateQRCode( )
        //{
        //    try
        //    {
        //        Guid guid = Guid.NewGuid();
        //        Image qrCodeImage = _orderService.QrCode(guid);
        //        qrCodeImage.Save(@$"C:\\code\{guid.ToString()}.png");
        //        return Ok(qrCodeImage);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("QR kodu oluşturulamadı: " + ex.Message);
        //    }
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSaleOrdersById(string id)
        {
            try
            {
                List<SaleOrderModel> saleOrderModel = _context.SaleOrderModels.FromSqlRaw($"exec GET_MSRAFOrderListID '{id.Split(' ')[0]}' ").AsEnumerable().ToList();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddSaleBarcode(BarcodeAddModel model)
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



        [HttpGet("GetProductsOfOrders/{numberOfList}")]
        public async Task<IActionResult> GetProductsOfOrders(int numberOfList)
        {

            try
            {   
                List<ProductOfOrderModel> productModels = _context.ztProductOfOrderModel.FromSqlRaw($"exec   GET_MSRAFOrderCollect {numberOfList} ").AsEnumerable().ToList();
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                if (productModels != null)
                {

                return Ok(productModels);
                }
                else
                {
                    return BadRequest(ErrorTextBase + "Null Object!");

                }
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }

        }

        [HttpPost("SetStatusOfPackages")]
        public async Task<IActionResult> SetStatusOfPackages(List<ProductOfOrderModel> models)
        {
            try
            {
                string query = $"[dbo].[UPDATE_MSRAFPackageUpdate] '{models.First().PackageNo}','false'";
                int count = await _context.Database.ExecuteSqlRawAsync(query); 
                                                                      

                return Ok(count);
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
        [HttpPost("TryPrintPicture")]
        public async Task<IActionResult> TryPrintPicture([FromBody] PrinterInvoiceRequestModel model)
        {
            try
            {
                // Download the image from the provided URL
                using (var webClient = new WebClient())
                {
                    byte[] imageData = await webClient.DownloadDataTaskAsync(new Uri(model.Url));

                    // Create a MemoryStream to hold the image data
                    using (var stream = new System.IO.MemoryStream(imageData))
                    {
                        // Create an Image object from the stream
                        using (var image = Image.FromStream(stream))
                        {
                            // Create a print document and set up the PrintPage event handler
                            var printDocument = new PrintDocument();
                            printDocument.PrinterSettings.PrinterName = model.PrinterName;
                            ;
                            printDocument.PrintPage += (s, e) =>
                            {
                                // Print the image on the print page
                                e.Graphics.DrawImage(image, e.MarginBounds);
                            };

                            // Send the print job to the default printer
                            printDocument.Print();
                        }
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
        [HttpGet("GetReadyToShipmentPackages")] //Paketlerin Statulerini True yada False olarak güncellettiriyor

        public async Task<IActionResult> GetReadyToShipmentPackages( )
        {
            try
            {
                List<ReadyToShipmentPackageModel> models = new List<ReadyToShipmentPackageModel>();
                string query = $" [dbo].[GET_MSRAFPackageList] 'false'";
                models = _context.ztReadyToShipmentPackageModel.FromSqlRaw(query).AsEnumerable().ToList();


                return Ok(models);
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPut("PutReadyToShipmentPackagesStatusById/{id}")]
        public async Task<IActionResult> PutReadyToShipmentPackagesStatusById(string id)
        {
            try
            {

                string query = $" [dbo].[usp_ztMSRafTakipUpdate] '{id}','true'";
                int affectedRows = _context.Database.ExecuteSqlRaw(query);


                return Ok(affectedRows);
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }




        [HttpGet("GetOrderSaleDetail/{orderNumber}")]
        public async Task<IActionResult> GetOrderSaleDetail(string orderNumber)
        {

            try
            {
                List<ProductOfOrderModel> orderSaleDetails = _context.ztProductOfOrderModel.FromSqlRaw($"GET_MSRAFSalesOrderDetail'{orderNumber}'").AsEnumerable().ToList();
                return Ok(orderSaleDetails);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);

            }

        }



        [HttpGet("GetOrderSaleDetailById/{PackageId}")]
        public async Task<IActionResult> GetOrderSaleDetailByPackageId(string PackageId)
        {

            try
            {
                List<ProductOfOrderModel> orderSaleDetails = _context.ztProductOfOrderModel.FromSqlRaw($"Get_MSSiparisToplaID '{PackageId}'").AsEnumerable().ToList();
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                return Ok(orderSaleDetails);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);

            }

        }


        [HttpPost("CountProduct")]

        public async Task<ActionResult<string>> CountProduct(CountProductRequestModel model)
        {
            try
            {
               
                ProductCountModel productCountModel =  _context.ztProductCountModel.FromSqlRaw($"exec Get_MSRAFSAYIM2 '{model.Barcode}','{model.ShelfNo}',0,'{model.OrderNumber}',{model.Qty}").AsEnumerable().First();
                if (productCountModel != null)
                {
                    return Ok(productCountModel); 
                }
                else
                {
                    return BadRequest(ErrorTextBase);
                }
           
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
        [HttpPost("CollectAndPack/{orderNo}")] //http://localhost:4200/order-operation/1-R-2-57
        public async Task<IActionResult> CollectAndPack( string orderNo)
        {
            List<ProductCountModel> result = new List<ProductCountModel>();
            List<string> productIds = new List<string>();   
            productIds.Add(orderNo);
            try
            {
                //gelen ordeNo içinde ws varsa yoksa usp_GetOrderForInvoiceToplu_R spsini çalıştır
                if (orderNo.Contains("WS"))
                {
                    bool result2 = await _orderService.AutoInvoice(orderNo, "usp_GetOrderForInvoiceToplu_WS ");
                    if (result2)
                    {

                        //FATURALAŞTIRMA İŞLEMİ YAPILMASI LAZIM
                        await _orderService.GenerateReceipt(productIds); // SİPARİŞ FATURASI YAZDIRILIYOR...
                        return Ok();
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    bool result2 = await _orderService.AutoInvoice(orderNo, "usp_GetOrderForInvoiceToplu_R ");
                    if (result2)
                    {

                        //FATURALAŞTIRMA İŞLEMİ YAPILMASI LAZIM
                        await _orderService.GenerateReceipt(productIds); // SİPARİŞ FATURASI YAZDIRILIYOR...
                        return Ok();
                    }
                    else
                    {
                        return BadRequest();
                    }
                }


                return BadRequest();

            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }


        [HttpPost("GetRemainingsProducts")]
        public async Task<IActionResult> GetRemainingsProducts(GetRemainingsProductsRequest model)
        {

            string query = $"exec POST_MSRafOrderPicking '{model.PackageId}','{model.Barcode}','{model.Quantity}'";
            try
            {

                List<RemainingProductsModel> returnedObject = null;

                try
                {
                    returnedObject =_context.ztRemainingProductsModel?.FromSqlRaw(query).ToList();
                }
                catch (InvalidOperationException ex)
                {
                    
                    returnedObject = null;
                }
                if (returnedObject is List<RemainingProductsModel>)
                {
                    RemainingProductsModel remainingProductsModel = (RemainingProductsModel)returnedObject.First();
                    return Ok(remainingProductsModel);
                }
                else
                {
                    List<InvoiceNumberModel> returnedObject2 = _context.ztInvoiceNumberModel?.FromSqlRaw(query).AsEnumerable().ToList();
                    if (returnedObject2 is List<InvoiceNumberModel>)
                    {
                        InvoiceNumberModel invoiceNumberModel = (InvoiceNumberModel)returnedObject2.First();
                        string invoiceNumber = invoiceNumberModel.InvoiceNumber;
                        return Ok(invoiceNumberModel);
                    }
                    else
                    {
                        return BadRequest("Okutma Hatalı");
                    }

                }


            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);

            }

        }

    }
    }

