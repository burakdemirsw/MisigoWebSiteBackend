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
        public IActionResult GenerateQRCode( )
        {
            try
            {
                Guid guid = Guid.NewGuid();
                Image qrCodeImage = _orderService.QrCode(guid);
                qrCodeImage.Save(@$"C:\\code\{guid.ToString()}.png");
                return Ok(qrCodeImage);
            }
            catch (Exception ex)
            {
                return BadRequest("QR kodu oluşturulamadı: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetSaleOrdersById(string id)
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



        [HttpGet("GetProductsOfOrders/{numberOfList}")]
        public IActionResult GetProductsOfOrders( int numberOfList)
        {

            try
            {
                List<ProductOfOrderModel> productModels = _context.ztProductOfOrderModel.FromSqlRaw($"exec  Get_MSSiparisTopla '{numberOfList.ToString()}' ").AsEnumerable().ToList();
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                    return Ok(productModels);
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
                string query = $"[dbo].[usp_ztMSRafTakipUpdate] '{models.First().PackageNo}','{true}'";
                int count = await _context.Database.ExecuteSqlRawAsync(query); // Use ExecuteSqlRawAsync for EF Core 5.0 and later
                                                                               // For EF Core 3.x or earlier, use _context.Database.ExecuteSqlCommandAsync(query);

                return Ok(count);
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        //[httppost("getreadypackages")]
        //public async task<iactionresult> setstatusofpackages(list<productofordermodel> models)
        //{
        //    try
        //    {
        //        string query = $"[dbo].[usp_ztmsraftakipupdate] '{models.first().packageno}','{true}'";
        //        int count = await _context.database.executesqlrawasync(query); // use executesqlrawasync for ef core 5.0 and later
        //                                                                       // for ef core 3.x or earlier, use _context.database.executesqlcommandasync(query);

        //        return ok(count);
        //    }
        //    catch (exception ex)
        //    {
        //        return badrequest(errortextbase + ex.message);
        //    }
        //}

        //exec Get_MSSiparisToplaID '4052373c-914d-4ef4-b11e-16765d842c16'
        [HttpGet("GetOrderSaleDetail/{orderNumber}")]
            public IActionResult GetOrderSaleDetail(string orderNumber)
            {

                try
                {
                    List<OrderSaleDetailModel> orderSaleDetails = _context.OrderSaleDetails.FromSqlRaw($"Get_ZTMSSatisSiparisDetay '{orderNumber}'").AsEnumerable().ToList();
                    //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                    return Ok(orderSaleDetails);
                }
                catch (Exception ex)
                {

                    return BadRequest(ErrorTextBase + ex.Message);

                }

            }

        [HttpGet("GetOrderSaleDetailById/{PackageId}")]
        public IActionResult GetOrderSaleDetailByPackageId(string PackageId)
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

    }
    }

