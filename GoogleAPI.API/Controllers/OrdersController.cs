using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.NEBIM;
using GoogleAPI.Domain.Models.NEBIM.Customer;
using GoogleAPI.Domain.Models.NEBIM.Invoice;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Domain.Models.NEBIM.Request;
using GoogleAPI.Domain.Models.NEBIM.Shelf;
using GoogleAPI.Domain.Models.NEBIM.Warehouse;
using GoogleAPI.Persistance.Concreates;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Drawing;
using System.Drawing.Printing;
using System.Net;
using System.Reflection;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        private readonly string IpAdresi = "http://192.168.2.36:7676";
        public OrdersController(
          GooleAPIDbContext context,
          IOrderService orderService, ILogService ls,IGeneralService gs, IInvoiceService invoiceService,ITransferService ts,ICountService cs
        
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
        [HttpGet]
        public async Task<IActionResult> GetOrders( ) //çalışıyor
        {
            try
            {
                List<SaleOrderModel> saleOrderModel = await _os.GetOrders();

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

                if(response)
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
                List<SaleOrderModel> saleOrderModel =await  _os.GetPurchaseOrders();

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

        [HttpGet("GetOrderSaleDetailById/{PackageId}")]
        public async Task<IActionResult> GetOrderSaleDetailByPackageId(string PackageId)
        {

            try
            {
                List<ProductOfOrderModel> orderSaleDetails = await  _os.GetPurchaseOrderSaleDetail(PackageId);
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
                List<CountListModel> countListModels = await  _is.GetInvoiceList();
       

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
        [HttpGet("CompleteCount/{orderNumber}")]
        public async Task<IActionResult> CompleteCount(string orderNumber)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            
            try
            {
               bool response = await _cs.CompleteCount(orderNumber);
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

                ProductCountModel? productCountModel =await _cs.CountProduct3(model);
                return Ok(productCountModel);

            }
            catch (Exception ex)
            {

                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
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

        [HttpGet("InventoryItems")]
        public async Task<IActionResult> GetInventoryItem( )
        {
            try
            {
                List<InventoryItemModel> list = await _ts.GetInventoryItem();

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

        #endregion -----------------------------------------------------------------------

        #region DIĞER-------------------------------------------------------------------

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

                                printDocument.PrintPage += (s, e) => {
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

    }
}
