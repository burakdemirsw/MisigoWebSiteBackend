using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.NEBIM;
using GoogleAPI.Domain.Models.NEBIM.Customer;
using GoogleAPI.Domain.Models.NEBIM.Invoice;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Domain.Models.NEBIM.Request;
using GoogleAPI.Domain.Models.NEBIM.Shelf;
using GoogleAPI.Domain.Models.NEBIM.Warehouse;
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

namespace GoogleAPI.API.Controllers
{
    [Route("api/Order")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly GooleAPIDbContext _context;
        private readonly string ErrorTextBase = "İstek Sırasında Hata Oluştu: ";
        private IOrderService _orderService;
        private ILogService _ls;

        public OrdersController(
          GooleAPIDbContext context,
          IOrderService orderService, ILogService ls
        )
        {
            _orderService = orderService;
            _ls = ls;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders( ) //çalışıyor
        {
            try
            {
                List<SaleOrderModel> saleOrderModel = await _context.SaleOrderModels.FromSqlRaw("exec GET_MSRAFOrderList").ToListAsync();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("GetOrdersByFilter")]
        public async Task<IActionResult> GetOrdersByFilter(OrderFilterModel model)
        {
            try
            {
                // Initialize the base SQL query
                string query = "SELECT TOP 100 OrderDate, OrderNumber, AllOrders.CurrAccCode, cdCurrAccDesc.CurrAccDescription, SalespersonCode, Qty1 = SUM(AllOrders.Qty1), Price = CAST(SUM(Doc_AmountVI) AS INT) FROM AllOrders LEFT OUTER JOIN cdCurrAccDesc ON cdCurrAccDesc.CurrAccTypeCode = AllOrders.CurrAccTypeCode AND cdCurrAccDesc.CurrAccCode = AllOrders.CurrAccCode AND cdCurrAccDesc.LangCode = 'TR' INNER JOIN stOrder ON stOrder.OrderLineID = AllOrders.OrderLineID WHERE ProcessCode IN ('WS', 'R')";

                // Initialize filter clauses
                List<string> filterClauses = new List<string>();

                // Add filters based on model properties
                if (!string.IsNullOrEmpty(model.OrderNo))
                {
                    filterClauses.Add($"OrderNumber like '{model.OrderNo}%'");
                }
                if (!string.IsNullOrEmpty(model.CurrAccCode))
                {
                    filterClauses.Add($"AllOrders.CurrAccCode like '{model.CurrAccCode}%'");
                }
                if (!string.IsNullOrEmpty(model.CustomerName))
                {
                    filterClauses.Add($"cdCurrAccDesc.CurrAccDescription LIKE '%{model.CustomerName}%'");
                }
                if (!string.IsNullOrEmpty(model.SellerCode))
                {
                    filterClauses.Add($"SalespersonCode = '{model.SellerCode}'");
                }
                if (model.StartDate != null)
                {
                    filterClauses.Add($"AllOrders.OrderDate >= '{model.StartDate:yyyy-MM-dd}'");
                }
                if (model.EndDate != null)
                {
                    filterClauses.Add($"AllOrders.OrderDate <= '{model.EndDate:yyyy-MM-dd}'");
                }

                // Combine filter clauses
                if (filterClauses.Count > 0)
                {
                    string filterConditions = string.Join(" AND ", filterClauses);
                    query += " AND " + filterConditions;
                }

                // Complete the query
                query += " GROUP BY OrderDate, OrderNumber, AllOrders.CurrAccCode, cdCurrAccDesc.CurrAccDescription, SalespersonCode ORDER BY OrderDate";

                List<SaleOrderModel> saleOrderModel = await _context.SaleOrderModels.FromSqlRaw(query).ToListAsync();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {
                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        #region alış faturası işlemleri

        [HttpGet("CustomerList/{customerType}")]
        public async Task<IActionResult> GetCustomerList(int customerType) //çalışıyor
        {
            try
            {
                List<CustomerModel> customerModel = await _context.ztCustomerModel.FromSqlRaw($"ms_GetCustomerList '{customerType}'").ToListAsync(); //3 dicez

                return Ok(customerModel);
            }
            catch (Exception ex)
            {

                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetPurchaseOrderSaleDetail/{orderNumber}")]
        public async Task<IActionResult> GetPurchaseOrderSaleDetail(string orderNumber)
        {

            try
            {
                List<ProductOfOrderModel> orderSaleDetails = await _context.ztProductOfOrderModel.FromSqlRaw($"GET_MSRAFSalesOrderDetailBP'{orderNumber}'").ToListAsync();
                orderSaleDetails = orderSaleDetails.OrderByDescending(p => p.Quantity).ToList();

                return Ok(orderSaleDetails);
            }
            catch (Exception ex)
            {

                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);

            }

        }

        [HttpGet("CompleteCount/{orderNumber}")]
        public async Task<IActionResult> CompleteCount(string orderNumber)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            try
            {
                string orderPrefix = orderNumber.Split('|')[0];
                string isTrue = orderNumber.Split('|')[1];
                int orderSaleDetails;
                string query2, query3;

                if (isTrue == "true")
                {
                    string query = $"Get_MSRAFCompleteCountShelf'{orderPrefix}'";
                    orderSaleDetails = await _context.Database.ExecuteSqlRawAsync(query);
                }
                else
                {
                    string query = $"Get_MSRAFCompleteCount'{orderPrefix}'";
                    orderSaleDetails = await _context.Database.ExecuteSqlRawAsync(query);
                }

                if (orderSaleDetails <= 0)
                {
                   
                     await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"İşlem Yapılmadı", requestUrl);
                    return BadRequest("İşlem Yapılmadı");
                }

                query2 = $"exec usp_GetOrderForInvoiceToplu_SayimEkle '{orderPrefix}'";
                query3 = $"exec usp_GetOrderForInvoiceToplu_SayimCikar '{orderPrefix}'";

                List<CountConfirmData> model1 = await _context.CountConfirmData.FromSqlRaw(query2).ToListAsync();
                List<CountConfirmData> model2 = await _context.CountConfirmData.FromSqlRaw(query3).ToListAsync();

                if (model1.First().Lines == "" && model2.First().Lines == "")
                {
                  
                     await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Sayım Eşitlenmiştir",requestUrl);
                    return BadRequest("Sayım Eşitlenmiştir");

                }

                List<CountConfirmModel> list = new List<CountConfirmModel>();

                CountConfirmModel model3 = new CountConfirmModel
                {
                    OfficeCode = model1.First().OfficeCode,
                    ModelType = model1.First().ModelType,
                    StoreCode = model1.First().StoreCode,
                    WarehouseCode = model1.First().WarehouseCode,
                    CompanyCode = model1.First().CompanyCode,
                    InnerProcessType = model1.First().InnerProcessType,
                    OperationDate = model1.First().OperationDate,
                    Lines = JsonConvert.DeserializeObject<List<MyDataLine>>(model1.First().Lines)

                };
                list.Add(model3);
                CountConfirmModel model4 = new CountConfirmModel
                {
                    ModelType = model2.First().ModelType,
                    OfficeCode = model2.First().OfficeCode,
                    StoreCode = model2.First().StoreCode,
                    WarehouseCode = model2.First().WarehouseCode,
                    CompanyCode = model2.First().CompanyCode,
                    InnerProcessType = model2.First().InnerProcessType,
                    OperationDate = model2.First().OperationDate,
                    Lines = JsonConvert.DeserializeObject<List<MyDataLine>>(model2.First().Lines)
                };
                list.Add(model4);

                //string json = (model3.Lines != null) ? JsonConvert.SerializeObject(model3) : JsonConvert.SerializeObject(model4);
                foreach (var model in list)
                {
                    if (model.Lines == null)
                    {
                        continue;
                    }
                    string json = JsonConvert.SerializeObject(model);
                    using (var httpClient2 = new HttpClient())
                    {
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        string sessionID = await ConnectIntegrator();
                        if (sessionID == null)
                        {
                           
                             await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"SessionId Alınamadı", requestUrl);
                            return BadRequest("SessionId Alınamadı");
                        }

                        var response = await httpClient2.PostAsync($"http://192.168.2.36:7676/(S({sessionID}))/IntegratorService/post?", content);

                        if (response == null)
                        {
                          
                             await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Response Alınamadı", requestUrl);
                            return BadRequest("Response Alınamadı");
                        }

                        var result = await response.Content.ReadAsStringAsync();
                        JObject jsonResponse = JObject.Parse(result);

                        if (jsonResponse != null && (int)jsonResponse["ModelType"] == 0)
                        {
                          
                             await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{jsonResponse}", requestUrl);
                            return BadRequest(jsonResponse);
                        }

                    }

                }
                await _ls.LogOrderSuccess( $"{methodName} Başarılı", HttpContext.Request.Path); return Ok(true);

            }
            catch (Exception ex)
            {
               
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("SetInventoryByOrderNumber/{orderNumber}")]
        public async Task<IActionResult> SetInventoryByOrderNumber(String orderNumber)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;

            try
            {
               

                string query = $"exec GET_MSRAFGetInventoryFromOrderNumber '{orderNumber}'";
             

                List<CountConfirmData> model1 = await _context.CountConfirmData.FromSqlRaw(query).ToListAsync();
                CountConfirmData data = model1[0];

                if (data.Lines == null)
                {

                    await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Sayım Eşitlenmiştir", requestUrl);
                    return Ok(true);

                }

                List<CountConfirmModel> list = new List<CountConfirmModel>();

                CountConfirmModel countConfirmModel = new CountConfirmModel
                {
                    OfficeCode = model1.First().OfficeCode,
                    ModelType = model1.First().ModelType,
                    StoreCode = model1.First().StoreCode,
                    WarehouseCode = model1.First().WarehouseCode,
                    CompanyCode = model1.First().CompanyCode,
                    InnerProcessType = model1.First().InnerProcessType,
                    OperationDate = model1.First().OperationDate,
                    Lines = JsonConvert.DeserializeObject<List<MyDataLine>?>(model1.First().Lines)

                };
                list.Add(countConfirmModel);
              

                //string json = (model3.Lines != null) ? JsonConvert.SerializeObject(model3) : JsonConvert.SerializeObject(model4);
                foreach (var model in list)
                {
                    if (model.Lines == null)
                    {
                        continue;
                    }
                    string json = JsonConvert.SerializeObject(model);
                    using (var httpClient2 = new HttpClient())
                    {
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        string sessionID = await ConnectIntegrator();
                        if (sessionID == null)
                        {

                            await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"SessionId Alınamadı", requestUrl);
                            return BadRequest("SessionId Alınamadı");
                        }

                        var response = await httpClient2.PostAsync($"http://192.168.2.36:7676/(S({sessionID}))/IntegratorService/post?", content);

                        if (response == null)
                        {

                            await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Response Alınamadı", requestUrl);
                            return BadRequest("Response Alınamadı");
                        }

                        var result = await response.Content.ReadAsStringAsync();
                        JObject jsonResponse = JObject.Parse(result);

                        ErrorResponseModel? erm = JsonConvert.DeserializeObject<ErrorResponseModel>(result);

                        if (erm != null)
                        {
                            if (erm.StatusCode == 400)
                            {


                                await _ls.LogInvoiceError($"{content}", "Sayım Ekleme Başarısız", erm.ExceptionMessage, requestUrl);

                                throw new Exception(erm.ExceptionMessage);
                            }
                            else
                            {
                                return Ok(true);
                            }
                        }



                    }

                }
                await _ls.LogOrderSuccess($"{methodName} Başarılı", HttpContext.Request.Path);
                return Ok(true);

            }
            catch (Exception ex)
            {

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}", requestUrl);
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
                return sessionId;
            }
            catch (HttpRequestException ex)
            {
                //Console.WriteLine($"HTTP isteği başarısız: {ex.Message}");
                return null;
            }
        }

        [HttpGet("GetCollectedOrderProducts/{orderNumber}")]
        public async Task<IActionResult> GetCollectedOrderProducts(string orderNumber)
        {
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            try
            {
                List<CollectedProduct>? collectedProduct = await _context.CollectedProducts?.FromSqlRaw($"exec [GET_MSRafCollectedProducts] '{orderNumber}'").ToListAsync();
                return Ok(collectedProduct);
                //1-BP-2-117
            }
            catch (Exception ex)
            {

                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);

            }

        }
        //MBT
        [HttpGet("GetProductOfCount/{orderNumber}")]
        public async Task<IActionResult> GetProductOfCount(string orderNumber)
        {

            try
            {
                List<CountedProduct> collectedProduct = await _context.CountedProducts.FromSqlRaw($"exec [Get_ProductOfCount] '{orderNumber}'").ToListAsync();
                return Ok(collectedProduct);
            }
            catch (Exception ex)
            {

                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;//            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);

            }

        }

        [HttpGet("GetProductOfTrasfer/{orderNumber}")]
        public async Task<IActionResult> GetProductOfTrasfer(string orderNumber)
        {

            try
            {
                List<TransferModel> collectedProduct = await _context.TransferModel.FromSqlRaw($"exec [Get_ProductOfTrasfer] '{orderNumber}'").ToListAsync();
                return Ok(collectedProduct);
            }
            catch (Exception ex)
            {

                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);

            }

        }

        [HttpGet("GetProductOfInvoice/{invoiceId}")]
        public async Task<IActionResult> GetProductOfInvoice(string invoiceId)
        {

            try
            {
                List<CreatePurchaseInvoice> collectedProduct = await _context.CreatePurchaseInvoices.FromSqlRaw($"exec [Get_ProductOfInvoice] '{invoiceId}'").ToListAsync();
                return Ok(collectedProduct);
            }
            catch (Exception ex)
            {

                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);

            }

        }

        [HttpPost("DeleteProductOfCount")]

        public async Task<IActionResult> DeleteProductOfCount(DeleteProductOfCount model)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;


            try
            {
                if (model.OrderNumber.StartsWith("TP"))
                {
                    string query = $"exec [Delete_TransferProduct] '{model.ItemCode}','{model.OrderNumber}'";
                    int collectedProduct = await _context.Database.ExecuteSqlRawAsync(query);
                    if (collectedProduct > 0)
                    {
                        return Ok(collectedProduct);
                    }
                    else
                    {
                      
                         await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"İşlem Yapılmadı", requestUrl);
                        return BadRequest("İşlem Yapılmadı");
                    }
                }
                else
                {
                    string query = $"exec [Delete_MSRAFDeleteProduct] '{model.ItemCode}','{model.OrderNumber}'";
                    int collectedProduct = await _context.Database.ExecuteSqlRawAsync(query);
                    if (collectedProduct > 0)
                    {
                        return Ok(collectedProduct);
                    }
                    else
                    {
                        
                      
                        await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"İşlem Yapılmadı", requestUrl);
                        return BadRequest("İşlem Yapılmadı");
                    }
                }

            }
            catch (Exception ex)
            {
               
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetInvoiceList")]
        public async Task<IActionResult> GetInvoiceList( )
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;

            try
            {

                List<CountListModel> countListModels = await _context.CountListModels.FromSqlRaw($"exec Get_InvoicesList").ToListAsync();

                return Ok(countListModels);
            }
            catch (Exception ex)
            {

        
                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("GetInvoiceListByFilter")]
        public async Task<IActionResult> GetInvoiceListByFilter(InvoiceFilterModel model)
        {
            try
            {
                // StartDate'i 'yyyy-MM-dd' formatına çevir
                string startDateString = model.StartDate.HasValue ? model.StartDate.Value.ToString("yyyy-MM-dd") : null;

                // EndDate'i 'yyyy-MM-dd' formatına çevir
                string endDateString = model.EndDate.HasValue ? model.EndDate.Value.ToString("yyyy-MM-dd") : null;

                string query = "SELECT MAX(ItemDate) AS LastUpdateDate, SUM(Quantity) AS TotalProduct, OrderNumber AS OrderNo FROM ZTMSRAFSAYIM3 Where len(OrderNumber)>1 query1  GROUP BY OrderNumber query2 ORDER BY LastUpdateDate DESC;";
                string addedQuery = "";
                string addedQuery2 = "";
                if (model.OrderNo != null)
                {
                    addedQuery += $"and orderNumber like '{model.OrderNo}%'";
                }
                if (model.InvoiceType != null)
                {
                    if (model.InvoiceType == "Alış")
                    {
                        addedQuery += $" and OrderNumber like 'BPI%'";

                    }
                    else
                    {
                        addedQuery += $" and OrderNumber like 'WSI%'";

                    }
                }
                if (model.StartDate != null)
                {

                    addedQuery2 += $"having MAX(ItemDate) >= {startDateString}  ";
                }
                if (model.EndDate != null)
                {
                    if (addedQuery2.Contains("having"))
                    {
                        addedQuery2 += $"and MAX(ItemDate) <= '{endDateString}'  ";
                    }
                    else
                    {
                        addedQuery2 += $" having MAX(ItemDate) <= '{endDateString}'  ";

                    }

                }
                query = query.Replace("query1", addedQuery);
                query = query.Replace("query2", addedQuery2);

                List<CountListModel> countListModels = await _context.CountListModels.FromSqlRaw(query).ToListAsync();

                return Ok(countListModels);
            }
            catch (Exception ex)
            {

                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetCountList")]
        public async Task<IActionResult> GetCountList( )
        {
            try
            {
                List<CountListModel> countListModels = await _context.CountListModels.FromSqlRaw($"exec Get_CountList").ToListAsync();

                return Ok(countListModels);
            }
            catch (Exception ex)
            {

                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("GetCountListByFilter")]
        public async Task<IActionResult> GetCountListByFilter(CountListFilterModel model)
        {
            try
            {
                // StartDate'i 'yyyy-MM-dd' formatına çevir
                string startDateString = model.StartDate.HasValue ? model.StartDate.Value.ToString("yyyy-MM-dd") : null;

                // EndDate'i 'yyyy-MM-dd' formatına çevir
                string endDateString = model.EndDate.HasValue ? model.EndDate.Value.ToString("yyyy-MM-dd") : null;

                string query = $"SELECT TOP 100 MAX(ItemDate) AS LastUpdateDate, COUNT(Barcode) AS TotalProduct, OrderNumber AS OrderNo  FROM ZTMSRAFSAYIM3 Where OrderNumber != ''  ";
                if (model.OrderNo != null)
                {
                    query += $"and OrderNumber like '{model.OrderNo}%'";
                }
                if (model.TotalProduct != null)
                {
                    query += $"and  COUNT(Barcode)>='{model.TotalProduct}'";

                }
                query += " group by OrderNumber ";
                if (model.StartDate != null)
                {
                    query += $"Having MAX(ItemDate)>='{startDateString}'";
                }
                if (model.EndDate != null)
                {
                    if (query.Contains("Having"))
                    {
                        query += $"and Having  MAX(ItemDate)<='{endDateString}'";

                    }
                    else
                    {
                        query += $" Having  MAX(ItemDate)<='{endDateString}'";

                    }

                }
                query += " Order by MAX(ItemDate) desc ";
                List<CountListModel> countListModels = await _context.CountListModels.FromSqlRaw(query).ToListAsync();

                return Ok(countListModels);
            }
            catch (Exception ex)
            {
                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetPurchaseOrders")]
        public async Task<IActionResult> GetPurchaseOrders( )
        {
            try
            {
                List<SaleOrderModel> saleOrderModel = await _context.SaleOrderModels.FromSqlRaw("exec GET_MSRAFOrderBPList").ToListAsync();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("GetPurchaseOrdersByFilter")]
        public async Task<IActionResult> GetPurchaseOrdersByFilter(OrderFilterModel model)
        {
            try
            {
                // Initialize the base SQL query
                string query = "SELECT OrderDate, OrderNumber, AllOrders.CurrAccCode, cdCurrAccDesc.CurrAccDescription, SalespersonCode, Qty1 = SUM(AllOrders.Qty1), Price = CAST(SUM(Doc_AmountVI) AS INT) FROM AllOrders LEFT OUTER JOIN cdCurrAccDesc ON cdCurrAccDesc.CurrAccTypeCode = AllOrders.CurrAccTypeCode AND cdCurrAccDesc.CurrAccCode = AllOrders.CurrAccCode AND cdCurrAccDesc.LangCode = 'TR' INNER JOIN stOrder ON stOrder.OrderLineID = AllOrders.OrderLineID WHERE ProcessCode IN ('BP')";

                // Initialize filter clauses
                List<string> filterClauses = new List<string>();

                // Add filters based on model properties
                if (!string.IsNullOrEmpty(model.OrderNo))
                {
                    filterClauses.Add($"OrderNumber like '{model.OrderNo}%'");
                }
                if (!string.IsNullOrEmpty(model.CurrAccCode))
                {
                    filterClauses.Add($"AllOrders.CurrAccCode like '{model.CurrAccCode}%'");
                }
                if (!string.IsNullOrEmpty(model.CustomerName))
                {
                    filterClauses.Add($"cdCurrAccDesc.CurrAccDescription LIKE '%{model.CustomerName}%'");
                }
                if (!string.IsNullOrEmpty(model.SellerCode))
                {
                    filterClauses.Add($"SalespersonCode = '{model.SellerCode}'");
                }
                if (model.StartDate != null)
                {
                    filterClauses.Add($"AllOrders.OrderDate >= '{model.StartDate:yyyy-MM-dd}'");
                }
                if (model.EndDate != null)
                {
                    filterClauses.Add($"AllOrders.OrderDate <= '{model.EndDate:yyyy-MM-dd}'");
                }

                // Combine filter clauses
                if (filterClauses.Count > 0)
                {
                    string filterConditions = string.Join(" AND ", filterClauses);
                    query += " AND " + filterConditions;
                }

                // Complete the query
                query += " GROUP BY OrderDate, OrderNumber, AllOrders.CurrAccCode, cdCurrAccDesc.CurrAccDescription, SalespersonCode ORDER BY OrderDate";

                List<SaleOrderModel> saleOrderModel = await _context.SaleOrderModels.FromSqlRaw(query).ToListAsync();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {
                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        #endregion alış faturası işlemleri

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSaleOrdersById(string id)
        {
            try
            {
                List<SaleOrderModel> saleOrderModel = await _context.SaleOrderModels.FromSqlRaw($"exec GET_MSRAFOrderListID '{id.Split(' ')[0]}' ").ToListAsync();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
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

                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetProductsOfOrders/{numberOfList}")]
        public async Task<IActionResult> GetProductsOfOrders(int numberOfList)
        {

            try
            {
                List<ProductOfOrderModel> productModels = await _context.ztProductOfOrderModel.FromSqlRaw($"exec   GET_MSRAFOrderCollect {numberOfList} ").ToListAsync();
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                if (productModels != null)
                {

                    return Ok(productModels);
                }
                else
                {
                    string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                     await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ErrorTextBase}",requestUrl);
                    return BadRequest(ErrorTextBase + "Null Object!");

                }
            }
            catch (Exception ex)
            {

                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
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
                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
        [HttpPost("TryPrintPicture")]
        public async Task<IActionResult> TryPrintPicture([FromBody] PrinterInvoiceRequestModel model)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            try
            {
                // Download the image from the provided URL
                Image response = _orderService.QrCode(model.PrinterName);

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
                    await _ls.LogOrderSuccess( $"{methodName} Başarılı", HttpContext.Request.Path); return Ok(true);
                }
                else
                {
                
                     await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Image could not be saved.", requestUrl);
                    return BadRequest("Image could not be saved.");
                }
            }
            catch (Exception ex)
            {
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
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

        [HttpGet("GetReadyToShipmentPackages")] //Paketlerin Statulerini True yada False olarak güncellettiriyor

        public async Task<IActionResult> GetReadyToShipmentPackages( )
        {
            try
            {
                List<ReadyToShipmentPackageModel> models = new List<ReadyToShipmentPackageModel>();
                string query = $" [dbo].[GET_MSRAFPackageList] 'false'";
                models = await _context.ztReadyToShipmentPackageModel.FromSqlRaw(query).ToListAsync();

                return Ok(models);
            }
            catch (Exception ex)
            {
                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPut("PutReadyToShipmentPackagesStatusById/{id}")]
        public async Task<IActionResult> PutReadyToShipmentPackagesStatusById(string id)
        {
            try
            {

                string query = $" [dbo].[usp_ztMSRafTakipUpdate] '{id}','true'";
                int affectedRows = await _context.Database.ExecuteSqlRawAsync(query);

                return Ok(affectedRows);
            }
            catch (Exception ex)
            {
                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetOrderSaleDetail/{orderNumber}")]
        public async Task<IActionResult> GetOrderSaleDetail(string orderNumber)
        {

            try
            {
                List<ProductOfOrderModel> orderSaleDetails = await _context.ztProductOfOrderModel.FromSqlRaw($"GET_MSRAFSalesOrderDetail'{orderNumber}'").ToListAsync();
                return Ok(orderSaleDetails);
            }
            catch (Exception ex)
            {

                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);

            }

        }

        [HttpGet("GetOrderSaleDetailById/{PackageId}")]
        public async Task<IActionResult> GetOrderSaleDetailByPackageId(string PackageId)
        {

            try
            {
                List<ProductOfOrderModel> orderSaleDetails = await _context.ztProductOfOrderModel.FromSqlRaw($"Get_MSSiparisToplaID '{PackageId}'").ToListAsync();
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                return Ok(orderSaleDetails);
            }
            catch (Exception ex)
            {

                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);

            }

        }
        #region raf - barkod doğrulama alanları
        //Post_MSRAFSTOKEKLE

        [HttpPost("CountTransferProductPuschase")]

        public async Task<ActionResult<string>> CountTransferProductPuschase(CreatePurchaseInvoice model)
        {
            try
            {
                string query = $"exec Post_MSRAFSTOKEKLE '{model.Barcode}','{model.ShelfNo}',0,'{model.OrderNumber}',{model.Quantity},'{model.WarehouseCode}','{model.CurrAccCode}'";
                ProductCountModel productCountModel = _context.ztProductCountModel.FromSqlRaw(query).AsEnumerable().First();
                if (productCountModel != null)
                {
                    return Ok(productCountModel);
                }
                else
                {
                    string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                     await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ErrorTextBase}",requestUrl);
                    return BadRequest(ErrorTextBase);
                }

            }
            catch (Exception ex)
            {

                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("CountProductPurchase")] //ZTMSRAFSAYIM3'e sayım yapar

        public async Task<ActionResult<string>> CountProduct(CreatePurchaseInvoice model)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            try
            {
                string query = $"exec Get_MSRAFSAYIM4'{model.Barcode}','{model.ShelfNo}',0,'{model.OrderNumber}',{model.Quantity},'{model.WarehouseCode}','{model.CurrAccCode}'";
                ProductCountModel productCountModel = _context.ztProductCountModel.FromSqlRaw(query).AsEnumerable().First();
                if (productCountModel != null)
                {
                    return Ok(productCountModel);
                }
                else
                {
                 
                    await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Eşleşme Sağlanamadı", requestUrl);
                    return BadRequest("Eşleşme Sağlanamadı");
                }

            }
            catch (Exception ex)
            {
                
                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }

        }

        [HttpPost("CountProduct")]

        public async Task<ActionResult<string>> CountProduct(CountProductRequestModel model)
        {
            try
            {
                string query = $"exec Get_MSRAFSAYIM2'{model.Barcode}','{model.ShelfNo}',0,'{model.OrderNumber}',{model.Qty}";
                ProductCountModel productCountModel = _context.ztProductCountModel.FromSqlRaw(query).AsEnumerable().First();
                if (productCountModel != null)
                {
                    return Ok(productCountModel);
                }
                else
                {
                    string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                     await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ErrorTextBase}",requestUrl);
                    return BadRequest(ErrorTextBase);
                }

            }
            catch (Exception ex)
            {
                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("CountTransferProduct")] //sayımda kullanılan

        public async Task<ActionResult<string>> CountTransferProduct(WarehouseFormModel model)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            try
            {
                string query = $"exec Get_MSRAFSAYIM6'{model.Barcode}','{model.ShelfNo}',0,'{model.OrderNo}',{model.Quantity},'{model.Warehouse}','','{model.BatchCode}','{model.WarehouseTo}'";
                ProductCountModel productCountModel = _context.ztProductCountModel.FromSqlRaw(query).AsEnumerable().First();
                if (productCountModel != null)
                {
                    return Ok(productCountModel);
                }
                else
                {
                    
                    await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"ErrorTextBase",requestUrl);
                    return BadRequest(ErrorTextBase);
                }

            }
            catch (Exception ex)
            {
              
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("CountProduct3")] //sayımda kullanılan sp

        public async Task<ActionResult<string>> CountProduct3(CountProductRequestModel2 model)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = Request.GetDisplayUrl();
            string baseUrl = string.Format("{0}:{1}{2}", Request.Scheme, Request.Host, Request.PathBase);


            ;
            try
            {
                //throw new Exception(baseUrl);

                string query = $"exec Get_MSRAFSAYIM3'{model.Barcode}','{model.ShelfNo}',0,'{model.OrderNo}',{model.Quantity},'{model.WarehouseCode}','{model.CurrAccCode}','{model.BatchCode}','{model.IsReturn}','{model.SalesPersonCode}','{model.TaxTypeCode}'";
                ProductCountModel productCountModel = _context.ztProductCountModel.FromSqlRaw(query).AsEnumerable().First();
                if (productCountModel != null)
                {
                    return Ok(productCountModel);
                }
                else
                {
  
                     await _ls.LogOrderWarn($"{methodName} productCountModel Boş Geldi", $"{ErrorTextBase}",requestUrl);
                    return BadRequest(ErrorTextBase);
                }

            }
            catch (Exception ex)
            {
      
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("CountProductControl")] //sayımda kullanılan

        public async Task<ActionResult<string>> CountProductControl(CountProductRequestModel2 model)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            try
            {
              
                string query = $"exec Get_MSRAFSAYIMKONTROL'{model.Barcode}','{model.ShelfNo}',0,'{model.OrderNo}',{model.Quantity},'{model.WarehouseCode}','{model.CurrAccCode}','{model.BatchCode}'";
                ProductCountModel productCountModel = _context.ztProductCountModel.FromSqlRaw(query).AsEnumerable().First();
                if (productCountModel != null)
                {
                    return Ok(productCountModel);
                }
                else
                {
                    
                     await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ErrorTextBase}",requestUrl);
                    return BadRequest(ErrorTextBase);
                }

            }
            catch (Exception ex)
            {
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("CountTransfer")] //sayımda kullanılan

        public async Task<ActionResult<string>> CountTransfer(WarehouseFormModel model)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
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
                     await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ErrorTextBase}",requestUrl);
                    return BadRequest(ErrorTextBase);
                }

            }
            catch (Exception ex)
            {
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("CountProductByBarcode/{barcode}")] //sadece rafları ddöndürür

        public async Task<IActionResult> CountProductByBarcode(string barcode)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            try
            {
                if (barcode.Contains("%20"))
                {
                    barcode = barcode.Replace("%20", " "); // Örnek düzeltme

                }
                string query = $"exec Get_MSRAFGOSTER '{barcode}'";
                List<ProductCountModel> productCountModel = await _context.ztProductCountModel.FromSqlRaw(query).ToListAsync();
                ;
                if (productCountModel != null)
                {
                    return Ok(productCountModel);
                }
                else
                {
                    
                     await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ErrorTextBase}",requestUrl);
                    return BadRequest(ErrorTextBase);
                }

            }
            catch (Exception ex)
            {
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("CountProductByBarcode2/{barcode}")]

        public async Task<IActionResult> CountProductByBarcode2(string barcode)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            try
            {
                if (barcode.Contains("%20"))
                {
                    barcode = barcode.Replace("%20", " "); // Örnek düzeltme

                }
                string query = $"exec Get_MSRAFGOSTER2 '{barcode}'";
                List<ProductCountModel2> productCountModel = await _context.ztProductCountModel2.FromSqlRaw(query).ToListAsync();
                ;
                if (productCountModel != null)
                {
                    return Ok(productCountModel);
                }
                else
                {
                     await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ErrorTextBase}",requestUrl);
                    return BadRequest(ErrorTextBase);
                }

            }
            catch (Exception ex)
            {
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }


        [HttpPost("CollectAndPack/{orderNo}")]
        public async Task<IActionResult> BillingOrder(OrderBillingRequestModel model)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            try
            {
                List<string> productIds = new List<string> { model.OrderNo };

                bool result2;

                switch (model.InvoiceModel)
                {
                    case 1: // ALIŞ FATURASI (oluşturulmamış)
                        result2 = await _orderService.AutoInvoice(model.OrderNo.ToString(), "usp_GetOrderForInvoiceToplu_BP2", model,HttpContext);
                        break;

                    case 2: // alış sipariş
                        if (model.OrderNo.Contains("BP") && !model.InvoiceType)
                        {
                            result2 = await _orderService.AutoInvoice(model.OrderNo, "usp_GetOrderForInvoiceToplu_BP", model, HttpContext);
                        }
                        else
                        {
                            result2 = false; // Handle other cases if needed
                        }
                        break;

                    case 3: // satış faturası
                        result2 = await _orderService.AutoInvoice(model.OrderNo, "usp_GetOrderForInvoiceToplu_WS2", model, HttpContext);
                        break;

                    case 4: // satış sipariş faturası
                        if (model.OrderNo.Contains("WS") && !model.InvoiceType)
                        {
                            result2 = await _orderService.AutoInvoice(model.OrderNo, "usp_GetOrderForInvoiceToplu_WS", model, HttpContext);
                        }
                        else if (model.OrderNo.Contains("R") && !model.InvoiceType)
                        {
                            result2 = await _orderService.AutoInvoice(model.OrderNo, "usp_GetOrderForInvoiceToplu_R", model, HttpContext);
                        }
                        else
                        {
                            result2 = false; // Handle other cases if needed
                        }
                        break;

                    default:
                        result2 = false; // Handle other cases if needed
                        break;
                }

                if (result2)
                {
                    await _ls.LogOrderSuccess( $"{methodName} Başarılı", HttpContext.Request.Path); return Ok(true);
                    // Continue with additional processing if needed
                }
                else
                {
                 
                    await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Obje Null Değer Döndürdü", requestUrl);
                    throw new Exception("Result 2 Is NULL");
                }
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

                List<RemainingProductsModel>? returnedObject = null;

                try
                {
                    returnedObject = await _context.ztRemainingProductsModel?.FromSqlRaw(query).ToListAsync();
                }
                catch (InvalidOperationException)
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
                    List<InvoiceNumberModel>? returnedObject2 = await _context.ztInvoiceNumberModel?.FromSqlRaw(query).ToListAsync();
                    if (returnedObject2 is List<InvoiceNumberModel>)
                    {
                        InvoiceNumberModel invoiceNumberModel = (InvoiceNumberModel)returnedObject2.First();
                        string invoiceNumber = invoiceNumberModel.InvoiceNumber;
                        return Ok(invoiceNumberModel);
                    }
                    else
                    {
                        string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                         await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ErrorTextBase}",requestUrl);
                        return BadRequest("Okutma Hatalı");
                    }

                }   

            }
            catch (Exception ex)
            {

                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);

            }

        }

        [HttpGet("GetSalesPersonModels")]
        public async Task<IActionResult> GetSalesPersonModels( )
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;

            try
            {
                List<SalesPersonModel> list = await _orderService.GetAllSalesPersonModels();
                if (list.Count < 1)
                {
        
                    await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Satış Elemanlarının Listesi Boş Geldi", requestUrl);
                    return BadRequest("Satış Elemanlarının Listesi Boş Geldi");
                }
                else
                {
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {

             
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost(nameof(FastTransfer))]
        public async Task<IActionResult> FastTransfer(FastTransferModel model)
        {
            try
            {
                int affectedRows = 0;
                var query = $"exec Usp_PostZtMSRAFSTOKTransfer '{model.Barcode}','{model.BatchCode}','{model.ShelfNo}','{model.Quantity}','{model.WarehouseCode}','{model.TargetShelfNo}'";
                affectedRows += await _context.Database.ExecuteSqlRawAsync(query);

                return Ok(affectedRows);

            }
            catch (Exception ex)
            {
                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        

        [HttpGet("GetAllFastTransferModels")]
        public async Task<IActionResult> GetAllFastTransferModels( )
        {
            try
            {
                List<FastTransferModel> models = await _context.FastTransferModels.ToListAsync();
                ;
                return Ok(models);
            }
            catch (Exception ex)
            {
                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetFastTransferModel/{operationId}")]
        public async Task<IActionResult> GetFastTransferModelsByOperationId(string operationId)
        {
            try
            {
                var models = _context.FastTransferModels
                  .Where(model => model.OperationId == operationId)
                  .ToListAsync();

                return Ok(models);
            }
            catch (Exception ex)
            {
                string methodName  =await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("DeleteProductFromFastTransfer")]
        public async Task<IActionResult> DeleteProductFromFastTransfer(DeleteProductOfCount deleteModel)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            try
            {
                // Veritabanında silme işlemi yapmadan önce gerekli kontrolü yapabilirsiniz.
                FastTransferModel? productToDelete = _context.FastTransferModels
                  .FirstOrDefault(op => op.OperationId == deleteModel.OrderNumber && op.Barcode == deleteModel.ItemCode);

                if (productToDelete == null)
                {
                    return NotFound();
                }

                _context.FastTransferModels.Remove(productToDelete);
                await _context.SaveChangesAsync();

                await _ls.LogOrderSuccess( $"{methodName} Başarılı", HttpContext.Request.Path); return Ok(true);
            }
            catch (Exception ex)
            {
               
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("InventoryItems")]
        public async Task<IActionResult> GetInventoryItem( )
        {
            try
            {
                List<InventoryItemModel> list = await _context.InventoryItemModels.FromSqlRaw("GET_MSRafTransferToWarehouse").ToListAsync();

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



        [HttpGet("DeleteInvoiceProducts/{orderNumber}")]
        public async Task<IActionResult> DeleteInvoiceProducts(string orderNumber)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            try
            {
                var affectedRow = await _context.Database.ExecuteSqlRawAsync($"delete ZTMSRAFInvoiceDetailBP WHERE  OrdernumberRaf ='{orderNumber}' ");
                if (affectedRow > 0)
                {
                    await _ls.LogOrderSuccess( $"{methodName} Başarılı", HttpContext.Request.Path); return Ok(true);
                }
                else
                {
                   
                  
                     await _ls.LogOrderWarn($"{methodName} Sırasında Silinecek Sipariş İçeriği Bulunamadı", $"{HttpContext.Request.Path}",requestUrl);
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
               
             
                 await _ls.LogOrderError($"{HttpContext.Request.Path}",$"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }
     

        [HttpGet("GetInventoryFromOrderNumber/{OrderNo}")]
        public async Task<IActionResult> GetInventoryFromOrderNumber(String OrderNo)
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            try
            {
                List<CountConfirmData> list = new List<CountConfirmData>();
                list= await _context.CountConfirmData.FromSqlRaw($"exec GET_MSRAFGetInventoryFromOrderNumber '{OrderNo}'").ToListAsync();

                return Ok(list);


            }
            catch (Exception ex)
            {


                await _ls.LogOrderError($"{HttpContext.Request.Path}", $"{methodName} Sırasında Hata Alındı", $"{ex.Message}",requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetAvailableShelves")]
        public async Task<IActionResult> GetAvailableShelves()
        {
            string methodName = await _orderService.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string requestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            try
            {
                List<AvailableShelf> list = new List<AvailableShelf>();
                list = await _context.AvailableShelfs.FromSqlRaw($"exec Get_MSRAFWillBeCount").ToListAsync();

                return Ok(list);


            }
            catch (Exception ex)
            {


                await _ls.LogOrderError($"{HttpContext.Request.Path}", $"{methodName} Sırasında Hata Alındı", $"{ex.Message}", requestUrl);
                return BadRequest(ErrorTextBase + ex.Message);
            }
        }



    }
}
#endregion
