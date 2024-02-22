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
using NHibernate.Hql.Ast.ANTLR.Tree;
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
        public async Task<bool> CompleteCount(string orderNumber , bool isShelfBased , bool isShelfBased2)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            int orderSaleDetails;
            string addQuery, DeleteQuery;

            if (isShelfBased == true)
            {
                string query = $"Get_MSRAFCompleteCountShelf'{orderNumber}'";
                orderSaleDetails = await _context.Database.ExecuteSqlRawAsync(query);
            }
            
            else
            {
                string query = $"Get_MSRAFCompleteCount'{orderNumber}'";
                orderSaleDetails = await _context.Database.ExecuteSqlRawAsync(query);
            }

            if (orderSaleDetails <= 0)
            {

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"İşlem Yapılmadı");
                throw new Exception("İşlem Yapılmadı");
            }


            DeleteQuery = $"exec usp_GetOrderForInvoiceToplu_SayimCikar '{orderNumber}'";
            List<CountConfirmData> deleteList = await _context.CountConfirmData.FromSqlRaw(DeleteQuery).ToListAsync();

            addQuery = $"exec usp_GetOrderForInvoiceToplu_SayimEkle '{orderNumber}'";

           
            if(deleteList.First().Lines != "")  
            {
                //çıkar boş değilse 
                CountConfirmModel deleteModel = new CountConfirmModel
                {
                    ModelType = deleteList.First().ModelType,
                    OfficeCode = deleteList.First().OfficeCode,
                    StoreCode = deleteList.First().StoreCode,
                    WarehouseCode = deleteList.First().WarehouseCode,
                    CompanyCode = deleteList.First().CompanyCode,
                    InnerProcessType = deleteList.First().InnerProcessType,
                    OperationDate = deleteList.First().OperationDate,
                    Lines = JsonConvert.DeserializeObject<List<MyDataLine>>(deleteList.First().Lines)
                };

                //çıkarı yolla 

                    
                    bool response = await CompleteCountTask(deleteModel,"SAYIM-ÇIKAR"); 
                    if(response)
                    {
                    //çıkar başarılı ise ekleyi yolla


                        List<CountConfirmData> addList = await _context.CountConfirmData.FromSqlRaw(addQuery).ToListAsync();
                        CountConfirmModel addModel = new CountConfirmModel
                        {
                            OfficeCode = addList.First().OfficeCode,
                            ModelType = addList.First().ModelType,
                            StoreCode = addList.First().StoreCode,
                            WarehouseCode = addList.First().WarehouseCode,
                            CompanyCode = addList.First().CompanyCode,
                            InnerProcessType = addList.First().InnerProcessType,
                            OperationDate = addList.First().OperationDate,
                            Lines = JsonConvert.DeserializeObject<List<MyDataLine>>(addList.First().Lines)

                        };

                        bool response2 = await CompleteCountTask(addModel, "SAYIM-EKLE"); 
                        if(response2)
                        {
                        //ekle başarılı ise bitir
                            await _ls.LogWarehouseSuccess($"{methodName} Başarılı", $"Çıkar İşlemi Yapıldı Ekle İşlemi Yapıldı");

                            return true; 
                        }
                        else
                        {

                             //ekle başarısız ise bitir
                             //RUN_DELETE SP
                            await _ls.LogWarehouseSuccess($"{methodName} Başarılı", $"Çıkar İşlemi Yapıldı Ekle İşlemi Yapılmadı");

                            return true; //yapılan çıkar işmini sil
                        }

                    }
                    else
                    {
                        await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Çıkar İşlemi Yapılmadı Ekle İşlemi Yapılmadı");

                        throw new Exception("Ekle Ve Çıkar Başarısız");

                    }


            }
            else
            {
                //çıkar başarısız ise ekleyi yolla
                List<CountConfirmData> addList = await _context.CountConfirmData.FromSqlRaw(addQuery).ToListAsync();

                if (addList.First().Lines == "")
                {
                    //ekle başarısız ise bitir
                    await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Çıkar İşlemi Yapılmadı Ekle İşlemi Yapılmadı");
                    throw new Exception("Ekle Ve Çıkar Başarısız : Çıkar ve Ekle Lines Null");
                   
                }
                else
                {

                    CountConfirmModel addModel = new CountConfirmModel
                    {
                        OfficeCode = addList.First().OfficeCode,
                        ModelType = addList.First().ModelType,
                        StoreCode = addList.First().StoreCode,
                        WarehouseCode = addList.First().WarehouseCode,
                        CompanyCode = addList.First().CompanyCode,
                        InnerProcessType = addList.First().InnerProcessType,
                        OperationDate = addList.First().OperationDate,
                        Lines = JsonConvert.DeserializeObject<List<MyDataLine>>(addList.First().Lines)

                    };

                    bool response = await CompleteCountTask(addModel, "SAYIM-EKLE");
                    if (response)
                    {
                        //ekle başarılı ise bitir
                        //RUN_DELETE SP
                        await _ls.LogWarehouseSuccess($"{methodName} Başarılı", $"Çıkar İşlemi Yapılmadı Ekle İşlemi Yapıldı");

                        return true; // çıkar - ekle +
                    }
                    else
                    {

                        //ekle başarısız ise bitir
                        await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Çıkar İşlemi Yapılmadı Ekle İşlemi Yapılmadı");

                        throw new Exception("Ekle Ve Çıkar Başarısız");

                    }



                }


            }


        }
        public async Task<bool> CompleteCountTask(CountConfirmModel model,string countType )
        {

            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            if (model.Lines == null)
            {
               return false;
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

                var response = await _gs.PostNebimAsync(json, countType);

                if (response == null)
                {

                    return false;
                    //throw new Exception("Response Alınamadı");
                }
                else
                {
                    //await _ls.LogOrderSuccess($"{methodName} Başarılı", "Sayım Başarıyla Eşitlenmiştir");

                    return true;

                }

            }
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


                    throw new Exception("İşlem Yapılmadı");
                }
            }
            else
            {
                string query = $"exec [Delete_MSRAFDeleteProduct] '{model.ItemCode}','{model.OrderNumber}','{model.LineId}'";
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

           var  productCountModel = await _context.ztProductCountModel.FromSqlRaw(query).ToListAsync();

            return productCountModel.FirstOrDefault();
        }
        public async Task<ProductCountModel> CountTransferProduct(WarehouseFormModel model)
        {


            string query = $"exec Get_MSRAFSAYIM6'{model.Barcode}','{model.ShelfNo}',0,'{model.OrderNo}',{model.Quantity},'{model.Warehouse}','','{model.BatchCode}','{model.WarehouseTo}'";
            var  productCountModel = await _context.ztProductCountModel.FromSqlRaw(query).ToListAsync();

            return productCountModel.FirstOrDefault();
            

        }
        public async Task<ProductCountModel> CountProduct3(CountProductRequestModel2 model)
        {

            string query = $"exec Get_MSRAFSAYIM3'{model.Barcode}','{model.ShelfNo}',0,'{model.OrderNo}',{model.Quantity},'{model.WarehouseCode}','{model.CurrAccCode}','{model.BatchCode}','{model.IsReturn}','{model.SalesPersonCode}','{model.TaxTypeCode}'";
           var productCountModel = await _context.ztProductCountModel.FromSqlRaw(query).ToListAsync();
            
            return productCountModel.FirstOrDefault();

        }

        public async Task<ProductCountModel> CountProduct4(CountProductRequestModel3 model) //(2)
        {

            string query = $"exec Get_MSRAFSAYIM7'{model.Barcode}','{model.ShelfNo}',0,'{model.OrderNo}',{model.Quantity},'{model.WarehouseCode}','{model.CurrAccCode}','{model.BatchCode}','{model.IsReturn}','{model.SalesPersonCode}','{model.TaxTypeCode}','{model.LineId}'";
            var productCountModel = await _context.ztProductCountModel.FromSqlRaw(query).ToListAsync();

            return productCountModel.FirstOrDefault();

        }
        public async Task<ProductCountModel> CountProductControl(CountProductRequestModel2 model) //(1)
        {
            string query = $"exec Get_MSRAFSAYIMKONTROL '{model.Barcode}','{model.ShelfNo}',0,'{model.OrderNo}',{model.Quantity},'{model.WarehouseCode}','{model.CurrAccCode}','{model.BatchCode}'";

            List<ProductCountModel> productCountModels = await _context.ztProductCountModel.FromSqlRaw(query).ToListAsync();

            // Now that you have retrieved the data, you can perform further operations on it if needed.
            // For example, you can return the first item if you expect only one result:
            return productCountModels.FirstOrDefault();
        }
        public async Task<ProductCountModel> CountTransfer(WarehouseFormModel model)
        {

            string query = $"exec CountTransfer'";
            ProductCountModel? productCountModel = await _context.ztProductCountModel.FromSqlRaw(query).FirstOrDefaultAsync();
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


        public async Task<List<ProductCountModel3>> GetShelvesOfProduct2(string barcode)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);


            if (barcode.Contains("%20"))
            {
                barcode = barcode.Replace("%20", " ");

            }
            string query = $"exec Get_MSRAFGOSTER3 '{barcode}'";
            List<ProductCountModel3> productCountModel = await _context.ProductCountModel3.FromSqlRaw(query).ToListAsync();
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

        public async Task<int> DeleteCountById(string id)
        {
           
                var affectedRow = _context.Database.ExecuteSqlRaw($"delete from ZTMSRAFSAYIM3 where OrderNumber = '{id}' ");


            return affectedRow;
               
            
        }
    }
}
