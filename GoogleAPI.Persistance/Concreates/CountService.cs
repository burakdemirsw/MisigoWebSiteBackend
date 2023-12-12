using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.NEBIM.Invoice;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Domain.Models.NEBIM.Warehouse;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection;

namespace GoogleAPI.Persistance.Concreates
{
    public class CountService : ICountService
    {
        private GooleAPIDbContext _context;
        private ILogService _ls;
        private IGeneralService _gs;
        public CountService(GooleAPIDbContext context, ILogService ls, IGeneralService gs)
        {
            _context = context;
            _ls = ls;
            _gs = gs;
        }
        public async Task<bool> CompleteCount(string orderNumber)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

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

                    await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"İşlem Yapılmadı");
                   throw new Exception("İşlem Yapılmadı");
                }

                query2 = $"exec usp_GetOrderForInvoiceToplu_SayimEkle '{orderPrefix}'";
                query3 = $"exec usp_GetOrderForInvoiceToplu_SayimCikar '{orderPrefix}'";

                List<CountConfirmData> model1 = await _context.CountConfirmData.FromSqlRaw(query2).ToListAsync();
                List<CountConfirmData> model2 = await _context.CountConfirmData.FromSqlRaw(query3).ToListAsync();

                if (model1.First().Lines == "" && model2.First().Lines == "")
                {

                    await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Sayım Eşitlenmiştir");
                    throw new Exception("Sayım Eşitlenmiştir");

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

                        string sessionID = await _gs.ConnectIntegrator();
                        if (sessionID == null)
                        {

                            await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"SessionId Alınamadı");
                            throw new Exception("SessionId Alınamadı");
                        }

                        var response = await _gs.PostNebimAsync(json, "SAYIM");

                        if (response == null)
                        {

                            await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Response Alınamadı");
                            throw new Exception("Response Alınamadı");
                        }
                        else
                        {
                            await _ls.LogOrderSuccess($"{methodName} Başarılı","Sayım Başarıyla Eşitlenmiştir");

                            return true;

                        }

                    }

                }
                await _ls.LogOrderSuccess($"{methodName} Başarılı", "Sayım Başarıyla Eşitlenmiştir");

                return true;

           
        }

        public async Task<List<CountedProduct>> GetProductOfCount(string orderNumber)
        {

            List<CountedProduct> collectedProduct = await _context.CountedProducts.FromSqlRaw($"exec [Get_ProductOfCount] '{orderNumber}'").ToListAsync();
                return collectedProduct;
            

        }
        public async Task<int> DeleteProductOfCount(DeleteProductOfCount model)
        {
         
                if (model.OrderNumber.StartsWith("TP"))
                {
                    string query = $"exec [Delete_TransferProduct] '{model.ItemCode}','{model.OrderNumber}'";
                    int collectedProduct = await _context.Database.ExecuteSqlRawAsync(query);
                    if (collectedProduct > 0)
                    {
                        return collectedProduct;
                    }
                    else
                    {

                      
                        throw new Exception ("İşlem Yapılmadı");
                    }
                }
                else
                {
                    string query = $"exec [Delete_MSRAFDeleteProduct] '{model.ItemCode}','{model.OrderNumber}'";
                    int collectedProduct = await _context.Database.ExecuteSqlRawAsync(query);
                    if (collectedProduct > 0)
                    {
                        return collectedProduct;
                    }
                    else
                    {



                        throw new Exception("İşlem Yapılmadı");
                    }
                }

           
        }
        public async Task<List<CountListModel>> GetCountList( )
        {
           
           List<CountListModel> countListModels = await _context.CountListModels.FromSqlRaw($"exec Get_CountList").ToListAsync();

            return countListModels;
            
        }
        public async Task<List<CountListModel>> GetCountListByFilter(CountListFilterModel model)
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

                return countListModels;
           
        }
        public async Task<ProductCountModel> CountTransferProductPuschase(CreatePurchaseInvoice model)
        {
            string query = $"exec Post_MSRAFSTOKEKLE '{model.Barcode}','{model.ShelfNo}',0,'{model.OrderNumber}',{model.Quantity},'{model.WarehouseCode}','{model.CurrAccCode}'";

            // Tek bir ProductCountModel nesnesi döndürmek için FirstOrDefaultAsync kullanılır.
            ProductCountModel productCountModel = await _context.ztProductCountModel.FromSqlRaw(query).FirstOrDefaultAsync();

            return productCountModel;
        }
        public async Task<ProductCountModel> CountTransferProduct(WarehouseFormModel model)
        {

           
                string query = $"exec Get_MSRAFSAYIM6'{model.Barcode}','{model.ShelfNo}',0,'{model.OrderNo}',{model.Quantity},'{model.Warehouse}','','{model.BatchCode}','{model.WarehouseTo}'";
                ProductCountModel? productCountModel = await _context.ztProductCountModel.FromSqlRaw(query).FirstOrDefaultAsync();

            return productCountModel;
              
        }
        public async Task<ProductCountModel?> CountProduct3(CountProductRequestModel2 model)
        {

                string query = $"exec Get_MSRAFSAYIM3'{model.Barcode}','{model.ShelfNo}',0,'{model.OrderNo}',{model.Quantity},'{model.WarehouseCode}','{model.CurrAccCode}','{model.BatchCode}','{model.IsReturn}','{model.SalesPersonCode}','{model.TaxTypeCode}'";
            ProductCountModel? productCountModel =  await _context.ztProductCountModel.FromSqlRaw(query).FirstOrDefaultAsync();
            return productCountModel;


        }
        public async Task<ProductCountModel> CountProductControl(CountProductRequestModel2 model)
        {
           

                string query = $"exec Get_MSRAFSAYIMKONTROL'{model.Barcode}','{model.ShelfNo}',0,'{model.OrderNo}',{model.Quantity},'{model.WarehouseCode}','{model.CurrAccCode}','{model.BatchCode}'";
            ProductCountModel? productCountModel = await _context.ztProductCountModel.FromSqlRaw(query).FirstOrDefaultAsync();
            return productCountModel;


        }
        public async Task<ProductCountModel> CountTransfer(WarehouseFormModel model)
        {
           
                string query = $"exec CountTransfer'";
            ProductCountModel? productCountModel =await  _context.ztProductCountModel.FromSqlRaw(query).FirstOrDefaultAsync();
            return productCountModel;

        }

        public async Task<ProductCountModel> CountProduct(CreatePurchaseInvoice model)
        {
                  
                string query = $"exec Get_MSRAFSAYIM4'{model.Barcode}','{model.ShelfNo}',0,'{model.OrderNumber}',{model.Quantity},'{model.WarehouseCode}','{model.CurrAccCode}'";
                ProductCountModel productCountModel = await _context.ztProductCountModel.FromSqlRaw(query).FirstOrDefaultAsync();

            return productCountModel;
                       
        }
        public async Task<List<ProductCountModel>> GetShelvesOfProduct(string barcode)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

           
                if (barcode.Contains("%20"))
                {
                    barcode = barcode.Replace("%20", " "); 

                }
                string query = $"exec Get_MSRAFGOSTER '{barcode}'";
                List<ProductCountModel> productCountModel = await _context.ztProductCountModel.FromSqlRaw(query).ToListAsync();
                ;
            return productCountModel;


        }

        public async Task<List<ProductCountModel2>> CountProductByBarcode2(string barcode)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

              if (barcode.Contains("%20"))
                {
                    barcode = barcode.Replace("%20", " "); // Örnek düzeltme

                }
                string query = $"exec Get_MSRAFGOSTER2 '{barcode}'";
                List<ProductCountModel2>? productCountModel = await _context.ztProductCountModel2.FromSqlRaw(query).ToListAsync();
                ;
                return productCountModel;
            }



    }
}
