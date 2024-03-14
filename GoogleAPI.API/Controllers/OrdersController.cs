using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.NEBIM.Customer;
using GoogleAPI.Domain.Models.NEBIM.Customer.CreateCustomerModel;
using GoogleAPI.Domain.Models.NEBIM.Invoice;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Order.CreateOrderModel;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Domain.Models.NEBIM.Shelf;
using GoogleAPI.Domain.Models.NEBIM.Warehouse;
using GoogleAPI.Domain.Models.Raport;
using GoogleAPI.Persistance.Concreates;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using GooleAPI.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Printing;
using System.Net;
using System.Reflection;

namespace GoogleAPI.API.Controllers
{
    [Route("api/Order")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly GooleAPIDbContext _context;
        private readonly string ErrorTextBase = "İstek Sırasında Hata Oluştu: ";
        private IOrderService _os;
        private ILogService _ls;
        private IGeneralService _gs;
        private IInvoiceService _is;
        private ITransferService _ts;
        private ICountService _cs;
   
        public OrdersController(
          GooleAPIDbContext context,
          IOrderService orderService, ILogService ls, IGeneralService gs, IInvoiceService invoiceService, ITransferService ts, ICountService cs

        )
        {
            _os = orderService;
            _ls = ls;
            _context = context;
            _gs = gs;
            _is = invoiceService;
            _ts = ts;
            _cs = cs;
        }

        #region SIPARIS-------------------------------------------------------------------
        [HttpGet("get-sale-orders/{type}/{invoiceStatus}")]
        public async Task<IActionResult> GetSaleOrders(int type, int invoiceStatus) // 1-> toplanabilir 2->toplanamaz
        {
            try
            {
                    List<SaleOrderModel> saleOrderModel = await _os.GetSaleOrders( type,  invoiceStatus);

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("get-orders-with-missing-items")]
        public async Task<IActionResult> GetSaleOrdersWithMissingItems() // 1-> toplanabilir 2->toplanamaz
        {
            try
            {
                List<SaleOrderModel> saleOrderModel = await _os.GetSaleOrdersWithMissingItems();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }


        [HttpPost("GetOrdersByFilter")]
        public async Task<IActionResult> GetOrdersByFilter(OrderFilterModel model)
        {
            try
            {
                List<SaleOrderModel> saleOrderModel = await _os.GetOrdersByFilter(model);

                return Ok(saleOrderModel);


            }
            catch (Exception ex)
            {
                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("CustomerList/{customerType}")]
        public async Task<IActionResult> GetCustomerList(int customerType) //çalışıyor
        {
            try
            {
                List<CustomerModel> customerModel = await _os.GetCustomerList(customerType);

                return Ok(customerModel);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetPurchaseOrderSaleDetail/{orderNumber}")]
        public async Task<IActionResult> GetPurchaseOrderSaleDetail(string orderNumber)
        {

            try
            {
                List<ProductOfOrderModel> orderSaleDetails = await _os.GetPurchaseOrderSaleDetail(orderNumber);

                return Ok(orderSaleDetails);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);

            }

        }

        [HttpGet("GetCollectedOrderProducts/{orderNumber}")]
        public async Task<IActionResult> GetCollectedOrderProducts(string orderNumber)
        {

            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {
                List<CollectedProduct>? collectedProduct = await _os.GetCollectedOrderProducts(orderNumber);
                return Ok(collectedProduct);
                //1-BP-2-117
            }
            catch (Exception ex)
            {

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);

            }

        }
        [HttpGet("SetInventoryByOrderNumber/{orderNumber}")]
        public async Task<IActionResult> SetInventoryByOrderNumber(String orderNumber)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);


            try
            {


                var response = await _os.SetInventoryByOrderNumber(orderNumber);

                if (response)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }

            }
            catch (Exception ex)
            {

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetPurchaseOrders")]
        public async Task<IActionResult> GetPurchaseOrders( )
        {
            try
            {
                List<SaleOrderModel> saleOrderModel = await _os.GetPurchaseOrders();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("GetPurchaseOrdersByFilter")]
        public async Task<IActionResult> GetPurchaseOrdersByFilter(OrderFilterModel model)
        {
            try
            {


                List<SaleOrderModel> saleOrderModel = await _os.GetPurchaseOrdersByFilter(model);

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {
                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSaleOrdersById(string id)
        {
            try
            {
                List<SaleOrderModel> saleOrderModel = await _os.GetSaleOrdersById(id);

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddSaleBarcode(BarcodeAddModel model)
        {
            try
            {

                var addedEntity = _os.AddSaleBarcode(model);

                return Ok(model);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetProductsOfOrders/{numberOfList}")]
        public async Task<IActionResult> GetProductsOfOrders(int numberOfList)
        {

            try
            {
                List<ProductOfOrderModel> productModels = await _os.GetProductsOfOrders(numberOfList);
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                if (productModels != null)
                {

                    return Ok(productModels);
                }
                else
                {
                    string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                    await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ErrorTextBase}");
                    return BadRequest(ErrorTextBase + "Null Object!");

                }
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }

        }

        [HttpPost("SetStatusOfPackages")]
        public async Task<IActionResult> SetStatusOfPackages(List<ProductOfOrderModel> models)
        {
            try
            {
                int count = await _os.SetStatusOfPackages(models);

                return Ok(count);
            }
            catch (Exception ex)
            {
                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }


        [HttpGet("GetReadyToShipmentPackages")] //Paketlerin Statulerini True yada False olarak güncellettiriyor
        public async Task<IActionResult> GetReadyToShipmentPackages( )
        {
            try
            {
                List<ReadyToShipmentPackageModel> models = await _os.GetReadyToShipmentPackages();

                return Ok(models);
            }
            catch (Exception ex)
            {
                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPut("PutReadyToShipmentPackagesStatusById/{id}")]
        public async Task<IActionResult> PutReadyToShipmentPackagesStatusById(string id)
        {
            try
            {


                int affectedRows = await _os.PutReadyToShipmentPackagesStatusById(id);

                return Ok(affectedRows);
            }
            catch (Exception ex)
            {
                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetOrderSaleDetail/{orderNumber}")]
        public async Task<IActionResult> GetOrderSaleDetail(string orderNumber)
        {

            try
            {
                List<ProductOfOrderModel> orderSaleDetails = await _os.GetOrderSaleDetail(orderNumber);
                return Ok(orderSaleDetails);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);

            }

        }

        [HttpGet("get-missing-products-of-order/{orderNumber}")]
        public async Task<IActionResult> GetMissingProductsOfOrder(string orderNumber)
        {

            try
            {
                List<ProductOfOrderModel> orderSaleDetails = await _os.GetMissingProductsOfOrder(orderNumber);
                return Ok(orderSaleDetails);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);

            }

        }


        [HttpGet("GetOrderSaleDetailById/{PackageId}")]
        public async Task<IActionResult> GetOrderSaleDetailByPackageId(string PackageId)
        {

            try
            {
                List<ProductOfOrderModel> orderSaleDetails = await _os.GetPurchaseOrderSaleDetail(PackageId);
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                return Ok(orderSaleDetails);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);

            }

        }

        #endregion -----------------------------------------------------------------------

        #region FATURA-------------------------------------------------------------------
        [HttpGet("GetInvoiceList")]
        public async Task<IActionResult> GetInvoiceList( )
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);


            try
            {
                List<CountListModel> countListModels = await _is.GetInvoiceList();


                return Ok(countListModels);
            }
            catch (Exception ex)
            {


                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("GetInvoiceListByFilter")]
        public async Task<IActionResult> GetInvoiceListByFilter(InvoiceFilterModel model)
        {
            try
            {
                List<CountListModel> countListModels = await _is.GetInvoiceListByFilter(model);
                return Ok(countListModels);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("DeleteInvoiceProducts/{orderNumber}")]
        public async Task<IActionResult> DeleteInvoiceProducts(string orderNumber)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {
                bool affectedRow = await _is.DeleteInvoiceProducts(orderNumber);
                if (affectedRow)
                {

                    return Ok(true);
                }
                else
                {


                    await _ls.LogOrderWarn($"{methodName} Sırasında Silinecek Sipariş İçeriği Bulunamadı", $"{HttpContext.Request.Path}");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {


                await _ls.LogOrderError($"{HttpContext.Request.Path}", $"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("CollectAndPack/{orderNo}")]
        public async Task<IActionResult> BillingOrder(OrderBillingRequestModel model)
        {


            try
            {
                var response = await _is.BillingOrder(model, HttpContext);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetSalesPersonModels")]
        public async Task<IActionResult> GetSalesPersonModels( )
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);


            try
            {
                List<SalesPersonModel> list = await _is.GetAllSalesPersonModels();
                if (list.Count < 1)
                {

                    await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Satış Elemanlarının Listesi Boş Geldi");
                    return BadRequest("Satış Elemanlarının Listesi Boş Geldi");
                }
                else
                {
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {


                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
        [HttpGet("GetProductOfInvoice/{invoiceId}")]
        public async Task<IActionResult> GetProductOfInvoice(string invoiceId)
        {

            try
            {
                List<CreatePurchaseInvoice> collectedProduct = await _is.GetProductOfInvoice(invoiceId);
                return Ok(collectedProduct);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);

            }

        }


        //[HttpPost("GetRemainingsProducts")]
        //public async Task<IActionResult> GetRemainingsProducts(GetRemainingsProductsRequest model)
        //{

        //    string query = $"exec POST_MSRafOrderPicking '{model.PackageId}','{model.Barcode}','{model.Quantity}'";
        //    try
        //    {

        //        List<RemainingProductsModel>? returnedObject = null;

        //        try
        //        {
        //            returnedObject = await _context.ztRemainingProductsModel?.FromSqlRaw(query).ToListAsync();
        //        }
        //        catch (InvalidOperationException)
        //        {

        //            returnedObject = null;
        //        }
        //        if (returnedObject is List<RemainingProductsModel>)
        //        {
        //            RemainingProductsModel remainingProductsModel = (RemainingProductsModel)returnedObject.First();
        //            return Ok(remainingProductsModel);
        //        }
        //        else
        //        {
        //            List<InvoiceNumberModel>? returnedObject2 = await _context.ztInvoiceNumberModel?.FromSqlRaw(query).ToListAsync();
        //            if (returnedObject2 is List<InvoiceNumberModel>)
        //            {
        //                InvoiceNumberModel invoiceNumberModel = (InvoiceNumberModel)returnedObject2.First();
        //                string invoiceNumber = invoiceNumberModel.InvoiceNumber;
        //                return Ok(invoiceNumberModel);
        //            }
        //            else
        //            {
        //                string methodName  =await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
        //                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ErrorTextBase}",requestUrl);
        //                return BadRequest("Okutma Hatalı");
        //            }

        //        }   

        //    }
        //    catch (Exception ex)
        //    {

        //        string methodName  =await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
        //         await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
        //        return BadRequest(ErrorTextBase + ex.Message);

        //    }

        //}


        #endregion -----------------------------------------------------------------------

        #region SAYIM-------------------------------------------------------------------

        [HttpGet("CompleteCount/{orderNumber}/{isShelfBased}/{isShelfBased2}")]
        public async Task<IActionResult> CompleteCount(string orderNumber, bool isShelfBased, bool isShelfBased2)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {
                bool response = await _cs.CompleteCount(orderNumber, isShelfBased, isShelfBased2);
                return Ok(response);

            }
            catch (Exception ex)
            {

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }


        //MBT
        [HttpGet("GetProductOfCount/{orderNumber}")]
        public async Task<IActionResult> GetProductOfCount(string orderNumber)
        {

            try
            {
                List<CountedProduct> collectedProduct = await _cs.GetProductOfCount(orderNumber);
                return Ok(collectedProduct);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
                //            

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);

            }

        }

        [HttpPost("DeleteProductOfCount")]

        public async Task<IActionResult> DeleteProductOfCount(DeleteProductOfCount model)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);



            try
            {
                var response = await _cs.DeleteProductOfCount(model);
                return Ok(response);

            }
            catch (Exception ex)
            {

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }


        [HttpGet("GetCountList")]
        public async Task<IActionResult> GetCountList( )
        {
            try
            {
                List<CountListModel> countListModels = await _cs.GetCountList();

                return Ok(countListModels);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("GetCountListByFilter")]
        public async Task<IActionResult> GetCountListByFilter(CountListFilterModel filter)
        {
            try
            {

                List<CountListModel> countListModels = await _cs.GetCountListByFilter(filter);

                return Ok(countListModels);
            }
            catch (Exception ex)
            {
                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }


        [HttpPost("CountTransferProductPuschase")]

        public async Task<ActionResult<string>> CountTransferProductPuschase(CreatePurchaseInvoice model)
        {
            try
            {
                ProductCountModel productCountModel = await _cs.CountTransferProductPuschase(model);
                if (productCountModel != null)
                {
                    return Ok(productCountModel);
                }
                else
                {
                    string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                    await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ErrorTextBase}");
                    return BadRequest(ErrorTextBase);
                }

            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("CountProductPurchase")]

        public async Task<ActionResult<string>> CountProduct(CreatePurchaseInvoice model)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {

                ProductCountModel productCountModel = await _cs.CountProduct(model);
                if (productCountModel != null)
                {
                    return Ok(productCountModel);
                }
                else
                {

                    await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Eşleşme Sağlanamadı");
                    return BadRequest("Eşleşme Sağlanamadı");
                }

            }
            catch (Exception ex)
            {

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }

        }


        [HttpPost("CountTransferProduct")]

        public async Task<ActionResult<string>> CountTransferProduct(WarehouseFormModel model)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {

                ProductCountModel productCountModel = await _cs.CountTransferProduct(model);
                if (productCountModel != null)
                {
                    return Ok(productCountModel);
                }
                else
                {

                    await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"ErrorTextBase");
                    return BadRequest(ErrorTextBase);
                }

            }
            catch (Exception ex)
            {

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("CountProduct3")] //sayımda kullanılan sp

        public async Task<ActionResult<string>> CountProduct3(CountProductRequestModel2 model)
        {
            try
            {

                ProductCountModel? productCountModel = await _cs.CountProduct3(model);
                return Ok(productCountModel);

            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
                string errorModelJson = JsonConvert.SerializeObject(model);
                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message + errorModelJson }");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("CountProduct4")] //sayımda kullanılan sp

        public async Task<ActionResult<string>> CountProduct4(CountProductRequestModel3 model)
        {
            try
            {

                ProductCountModel? productCountModel = await _cs.CountProduct4(model);
                return Ok(productCountModel);

            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
                string errorModelJson = JsonConvert.SerializeObject(model);
                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message + errorModelJson}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("CountProductControl")] //sayımda kullanılan

        public async Task<ActionResult<string>> CountProductControl(CountProductRequestModel2 model)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {


                ProductCountModel productCountModel = await _cs.CountProductControl(model);

                return Ok(productCountModel);


            }
            catch (Exception ex)
            {
                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("CountTransfer")] //sayımda kullanılan

        public async Task<ActionResult> CountTransfer(WarehouseFormModel model)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {
                string query = $"exec CountTransfer'";
                ProductCountModel productCountModel = _context.ztProductCountModel.FromSqlRaw(query).AsEnumerable().First();
                if (productCountModel != null)
                {
                    return Ok(productCountModel);
                }
                else
                {
                    await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ErrorTextBase}");
                    return BadRequest(ErrorTextBase);
                }

            }
            catch (Exception ex)
            {
                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetShelvesOfProduct/{barcode}")] //sadece rafları ddöndürür

        public async Task<IActionResult> GetShelvesOfProduct(string barcode)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {
                List<ProductCountModel> productCountModel = await _cs.GetShelvesOfProduct(barcode);
                ;
                if (productCountModel != null)
                {
                    return Ok(productCountModel);
                }
                else
                {

                    await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ErrorTextBase}");
                    return BadRequest(ErrorTextBase);
                }

            }
            catch (Exception ex)
            {
                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("GetShelvesOfProduct2")] //sadece rafları ddöndürür

        public async Task<IActionResult> GetShelvesOfProduct2(QrControlCommandModel model)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {
               
                List<ProductCountModel3> productCountModel = await _cs.GetShelvesOfProduct2(model.Qr);
                
                if (productCountModel != null)
                {
                    return Ok(productCountModel);
                }
                else
                {

                    await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ErrorTextBase}");
                    return BadRequest(ErrorTextBase);
                }

            }
            catch (Exception ex)
            {
                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("CountProductByBarcode2/{barcode}")]

        public async Task<IActionResult> CountProductByBarcode2(string barcode)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {

                List<ProductCountModel2> productCountModel = await _cs.CountProductByBarcode2(barcode);

                if (productCountModel != null)
                {
                    return Ok(productCountModel);
                }
                else
                {
                    await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ErrorTextBase}");
                    return BadRequest(ErrorTextBase);
                }

            }
            catch (Exception ex)
            {
                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }


        [HttpPost("destroy-item")]
        public async Task<IActionResult> DestroyItem(ProductOfOrderModel model)
        {
            try
            {
                string query = $"Get_MSGEksikUrun '{model.ShelfNo}','','{model.Barcode}','{model.ItemCode}','{model.ColorCode}','{model.ItemDim1Code}','{model.Quantity}','','','','','{model.LineId}'";

                DestroyItem_Response? response = _context.DestroyItem_Response?.FromSqlRaw(query).ToList().First();
                if (response != null)
                {
                    if (Convert.ToBoolean(response.State) == true)
                    {
                        return Ok(true);
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
        [HttpGet("delete-missing-item/{orderNumber}/{barcode}")]
        public async Task<IActionResult> DestroyItem(string orderNumber , string barcode)
        {
            try
            {
                string query = $"Delete_MissingProductFromOrder '{orderNumber}','{barcode}'";

                int affectedRow = await _context.Database.ExecuteSqlRawAsync(query);
                Console.WriteLine(affectedRow);
                if (affectedRow > 0 || affectedRow == -1)
                {

                    return Ok(true);

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

        //[HttpPost("CountProduct")]

        //public async Task<ActionResult<string>> CountProduct(CountProductRequestModel model)
        //{
        //    try
        //    {
        //        string query = $"exec Get_MSRAFSAYIM2'{model.Barcode}','{model.ShelfNo}',0,'{model.OrderNumber}',{model.Qty}";
        //        ProductCountModel productCountModel = _context.ztProductCountModel.FromSqlRaw(query).AsEnumerable().First();
        //        if (productCountModel != null)
        //        {
        //            return Ok(productCountModel);
        //        }
        //        else
        //        {
        //            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
        //            
        //            await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ErrorTextBase}");
        //            return BadRequest(ErrorTextBase);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
        //        
        //        await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
        //        return BadRequest(ErrorTextBase + ex.Message);
        //    }
        //}



        #endregion -----------------------------------------------------------------------

        #region TRANSFER-------------------------------------------------------------------
        [HttpGet("GetProductOfTrasfer/{orderNumber}")]
        public async Task<IActionResult> GetProductOfTrasfer(string orderNumber)
        {

            try
            {
                List<TransferModel> collectedProduct = await _ts.GetProductOfTrasfer(orderNumber);

                return Ok(collectedProduct);
            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);

            }

        }
        [HttpPost(nameof(FastTransfer))]
        public async Task<IActionResult> FastTransfer(FastTransferModel model)
        {
            try
            {
                int affectedRows = await _ts.FastTransfer(model);

                return Ok(affectedRows);

            }
            catch (Exception ex)
            {
                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetAllFastTransferModels")]
        public async Task<IActionResult> GetAllFastTransferModels( )
        {
            try
            {
                List<FastTransferModel> models = await _ts.GetAllFastTransferModels();
                return Ok(models);
            }
            catch (Exception ex)
            {
                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetFastTransferModel/{operationId}")]
        public async Task<IActionResult> GetFastTransferModelsByOperationId(string operationId)
        {
            try
            {
                var models = _ts.GetFastTransferModelsByOperationId(operationId);

                return Ok(models);
            }
            catch (Exception ex)
            {
                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("DeleteProductFromFastTransfer")]
        public async Task<IActionResult> DeleteProductFromFastTransfer(DeleteProductOfCount deleteModel)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {
                // Veritabanında silme işlemi yapmadan önce gerekli kontrolü yapabilirsiniz.
                FastTransferModel? productToDelete = await _ts.DeleteProductFromFastTransfer(deleteModel);

                if (productToDelete == null)
                {
                    return NotFound();
                }

                _context.FastTransferModels.Remove(productToDelete);
                await _context.SaveChangesAsync();

                await _ls.LogOrderSuccess($"{methodName} Başarılı", HttpContext.Request.Path);
                return Ok(true);
            }
            catch (Exception ex)
            {

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("InventoryItems/{type}")]
        public async Task<IActionResult> GetInventoryItem(string type)
        {
            try
            {
                List<InventoryItemModel> list = await _ts.GetInventoryItem(type);

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

        [HttpGet("GetInventoryFromOrderNumber/{OrderNo}")]
        public async Task<IActionResult> GetInventoryFromOrderNumber(String OrderNo)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {
                List<CountConfirmData> list = await _ts.GetInventoryFromOrderNumber(OrderNo);

                return Ok(list);


            }
            catch (Exception ex)
            {


                await _ls.LogOrderError($"{HttpContext.Request.Path}", $"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetAvailableShelves")]
        public async Task<IActionResult> GetAvailableShelves( )
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {
                List<AvailableShelf> list = await _ts.GetAvailableShelves();

                return Ok(list);


            }
            catch (Exception ex)
            {


                await _ls.LogOrderError($"{HttpContext.Request.Path}", $"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("get-invoices-of-customer/{orderNumber}")]
        public async Task<IActionResult> GetInvoicesOfCustomer( string orderNumber)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {
                List<InvoiceOfCustomer_VM> list = _context.InvoiceOfCustomer_VM.FromSqlRaw($"exec MSG_GetInboxEInvoice '{orderNumber}'").ToList();

                return Ok(list);


            }
            catch (Exception ex)
            {


                await _ls.LogOrderError($"{HttpContext.Request.Path}", $"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        #endregion -----------------------------------------------------------------------

        #region DIĞER -------------------------------------------------------------------
        [HttpPost("Qr")]
        public async Task<IActionResult> PrintQr2(QrRequest model)
        {
            if (string.IsNullOrEmpty(model.ImageCode))
            {
                return BadRequest();
            }

            try
            {
                for (int i = 1; i <= model.PrintCount; i++)
                {
                    byte[] imageBytes = Convert.FromBase64String(model.ImageCode);




                    using (MemoryStream stream = new MemoryStream(imageBytes))
                    {
                        // MemoryStream'den Bitmap oluştur
                        Bitmap image = new Bitmap(stream);

                        // Resmi 10 cm genişliğindeki bir kağıda sığdırmak için gerekli ölçüleri hesaplayın
                        int targetWidth = (int)(10 * 96 / 2.54); // 10 cm'yi piksel cinsinden hesaplayın (1 inç = 96 piksel)
                        int targetHeight = (int)((float)image.Height / image.Width * targetWidth);

                        // Resmi yeni boyuta dönüştürün
                        Bitmap resizedImage = new Bitmap(image, targetWidth, targetHeight);

                        System.Drawing.Printing.PrintDocument printDocument = new System.Drawing.Printing.PrintDocument();
                        printDocument.PrinterSettings.PrinterName = "SEWOO LK-P4XX Label"; // Yazıcı adını buraya ekleyin

                        // Dikey olarak yazdırmak için kağıdı döndürün
                        PaperSize customPaperSize = new PaperSize("Custom Label Size", targetWidth, targetHeight);
                        printDocument.DefaultPageSettings.PaperSize = customPaperSize;
                        printDocument.PrintPage += (s, args) =>
                        {
                            // Resmi sayfanın ortasına yerleştirin
                            int centerX = (args.PageBounds.Width - resizedImage.Width) / 2;
                            int centerY = (args.PageBounds.Height - resizedImage.Height) / 2;

                            // Resmi merkezlenmiş konumda çiz
                            args.Graphics.DrawImage(resizedImage, centerX, centerY);
                        };
                        printDocument.PrintController = new System.Drawing.Printing.StandardPrintController();


                        printDocument.Print();




                    }

                }
                // Base64 veriyi byte dizisine çevir

                return Ok(true);
            }
            catch (Exception ex)
            {
                // Hata durumunda BadRequest döndür
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("TryPrintPicture")]
        public async Task<IActionResult> TryPrintPicture([FromBody] PrinterInvoiceRequestModel model)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {
                // Download the image from the provided URL
                Image response = _gs.QrCode(model.PrinterName);

                // Generate a unique ID for the image file
                string uniqueId = Guid.NewGuid().ToString();

                // Save the image to C:/code with the unique ID as the filename
                string imagePath = SaveImage(response, $"C:/code/{uniqueId}.png");

                // Check if the image was successfully saved
                if (!string.IsNullOrEmpty(imagePath))
                {
                    using (var webClient = new WebClient())
                    {
                        byte[] imageData = await webClient.DownloadDataTaskAsync(new Uri("https://www.destekalani.com/images/desteklogo.png"));



                        // Create a MemoryStream to hold the logo image data
                        using (var logoStream = new System.IO.MemoryStream(imageData))
                        {
                            // Create an Image object from the logo stream
                            using (var logoImage = Image.FromStream(logoStream))
                            {
                                // Create a print document and set up the PrintPage event handler
                                var printDocument = new PrintDocument();
                                printDocument.PrinterSettings.PrinterName = model.PrinterName;

                                printDocument.PrintPage += (s, e) =>
                                {
                                    // Print the logo image on the print page
                                    e.Graphics.DrawImage(logoImage, e.MarginBounds.Left, e.MarginBounds.Top, 100, 100);

                                    // Print the saved image on the print page
                                    e.Graphics.DrawImage(Image.FromFile(imagePath), e.MarginBounds.Left + 100, e.MarginBounds.Top);
                                };

                                // Send the print job to the default printer
                                printDocument.Print();
                            }
                        }
                    }

                    // Return a success response
                    await _ls.LogOrderSuccess($"{methodName} Başarılı", HttpContext.Request.Path);
                    return Ok(true);
                }
                else
                {

                    await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Image could not be saved.");
                    return BadRequest("Image could not be saved.");
                }
            }
            catch (Exception ex)
            {
                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                return BadRequest(ErrorTextBase + ex.Message);
            }

        }

        // Method to save the image to a file and return the file path
        private string SaveImage(Image image, string filePath)
        {
            try
            {
                image.Save(filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error saving image: " + ex.Message);
                return null;
            }
        }

        #endregion -----------------------------------------------------------------------

        #region SATIŞ

        [HttpPost("get-customer-list-2")]
  
        public async Task<ActionResult<List<CustomerList_VM>>> GetCustomerList_2(GetCustomerList_CM request)
        {

            List<CustomerList_VM> orderList = await _os.GetCustomerList_2(request);
            return Ok(orderList);
        }
            
        [HttpPost("get-customer-address")]
        public async Task<ActionResult<List<CustomerAddress_VM>>> GetCustomerAddress(GetCustomerAddress_CM request)
        {

            List<CustomerAddress_VM> orderList = await _os.GetCustomerAddress(request);
            return Ok(orderList);
        }

        [HttpPost("create-customer")]
        public async Task<ActionResult> GetCustomerAddress(CreateCustomer_CM request)
        {

            var response = await _os.SendCustomerToNebim(request);
            return Ok(response);
        }
        [HttpPost("create-order")]
        public async Task<ActionResult> GetOrder(NebimOrder order)
        {

            var response = await _os.CreateOrder(order);
            return Ok(response);
        }

        [HttpGet("get-client-order/{id}")]
        public async Task<ActionResult<ClientOrder_DTO>> GetClientOrder(Guid id)
        {

            var response = await _os.GetClientOrder(id);
            return Ok(response);
        }


        [HttpPost("create-client-order")]
        public async Task<ActionResult> CreateClientOrder(ClientOrder request)
        {

            var response = await _os.CreateClientOrder(request);
            return Ok(response);
        }

        [HttpPost("create-client-order-basket-item")]
        public async Task<ActionResult> CreateClientOrderBasketItem(ClientOrderBasketItem request)
        {

            var response = await _os.CreateClientOrderBasketItem(request);
            return Ok(response);
        }

        [HttpGet("update-client-order-basket-item/{id}/{lineId}/{quantity}/{price}")]
        public async Task<ActionResult<ClientOrder_DTO>> UpdateClientOrderBasketItem(Guid id, Guid lineId,int quantity, decimal price)
        {

            var response = await _os.UpdateClientOrderBasketItem(id, lineId,quantity,price);
            return Ok(response);
        }

        [HttpGet("update-client-order-payment/{orderId}/{paymentDescription}")]
        public async Task<ActionResult<ClientOrder_DTO>> UpdateClientOrderPayment(Guid orderId, string paymentDescription)
        {

            var response = await _os.UpdateClientOrderPayment(orderId,paymentDescription);
            return Ok(response);
        }

        [HttpPost("edit-client-customer")]
        public async Task<ActionResult> CreateClientCustomer(ClientCustomer request)
        {

            var response = await _os.EditClientCustomer(request);
            return Ok(response);
        }
        [HttpGet("get-client-customer/{addedSalesPersonCode}")]
        public async Task<ActionResult<ClientOrder_DTO>> GetClientCustomer(string addedSalesPersonCode)
        {

            var response = await _os.GetClientCustomer(addedSalesPersonCode);
            return Ok(response);
        }
        [HttpGet("get-order-detail/{orderNumber}")]
        public async Task<ActionResult<OrderDetail>> GetOrderDetail(string orderNumber)
        {

            string query = $"exec msg_GetOrderDetail '{orderNumber}' ";
            OrderDetail_Model? OrderDetail= _context.OrderDetail_Model.FromSqlRaw(query).AsEnumerable().FirstOrDefault();
            if (OrderDetail != null)
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    OrderDate =Convert.ToDateTime( OrderDetail.OrderDate),  
                    TotalPrice  = OrderDetail.TotalPrice,
                    SalespersonCode = OrderDetail.SalespersonCode,
                    OrderNumber = OrderDetail.OrderNumber,
                    Description = OrderDetail.Description,
                    CurrAccCode = OrderDetail.CurrAccCode,
                    Phone = OrderDetail.Phone,
                    Mail = OrderDetail.Mail,
                    Customer = OrderDetail.Customer,
                    City = OrderDetail.City,
                    District = OrderDetail.District,
                    Address = OrderDetail.Address,
                    Products = JsonConvert.DeserializeObject<List<BasketProductSummary>>(
                              OrderDetail.Products
                            )
                };
                return Ok(orderDetail);

            }

            return Ok();
        }

        [HttpPost("add-customer-address")]
        public async Task<ActionResult> AddCustomerAddress(AddCustomerAddress_CM request)
        {

            var response = await _os.AddCustomerAddress(request);
            return Ok(response);
        }


        #endregion

        #region RAPORTS
        [HttpGet("get-raports/{day}")]
        public async Task<ActionResult<Raport_CR>> GetOrderDetail(int day)
        {
            Raport_CR response = new Raport_CR();
            var query_1 = $"exec msg_Raport_1 '{day}' ";

            Raport_1? raport_1 = _context.Raport_1.FromSqlRaw(query_1).AsEnumerable().FirstOrDefault();
            response.Raport_1 = raport_1;
            var query_2 = $"exec msg_Raport_2  ";
            List<Raport_2>? raport_2 = _context.Raport_2.FromSqlRaw(query_2).AsEnumerable().ToList();
            response.Raport_2 = raport_2;
            var query_3 = $"exec msg_Raport_3  ";
            List<Raport_3>? raport_3 = _context.Raport_3.FromSqlRaw(query_3).AsEnumerable().ToList();
            response.Raport_3 = raport_3;
            var query_4 = $"exec msg_Raport_4 '{day}' ";
            List<Raport_4>? raport_4 = _context.Raport_4.FromSqlRaw(query_4).AsEnumerable().ToList();
            response.Raport_4 = raport_4;


            return Ok(response);
        }
        #endregion
    }
}
