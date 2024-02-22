using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.NEBIM.Customer;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Drawing;
using System.Reflection;
using System.Text;

namespace GoogleAPI.Persistance.Concretes
{
    public class OrderService : IOrderService // IOrderService bir arayüz olabilir, işlevinizi tanımlar
    {

        private GooleAPIDbContext _context;
        private IGeneralService _gs;
        private ILogService _ls;

        public OrderService(GooleAPIDbContext context, ILogService ls, IGeneralService gs)
        {
            _context = context;
            _ls = ls;
            _gs = gs;
        }

        public Task<Bitmap> GetOrderDetailsFromQrCode(string data)
        {
            throw new NotImplementedException();
        }
        public async Task<List<SaleOrderModel>> GetSaleOrders(int type ) //çalışıyor
        {

            List<SaleOrderModel> saleOrderModel = await _context.SaleOrderModels.FromSqlRaw($"exec GET_MSRAFOrderList3 {type}").ToListAsync();

            return saleOrderModel;

        }
        public async Task<List<SaleOrderModel>> GetSaleOrdersWithMissingItems() //çalışıyor
        {

            List<SaleOrderModel> saleOrderModel = await _context.SaleOrderModels.FromSqlRaw($"exec GET_MSRAFOrderListMissing ").ToListAsync();

            return saleOrderModel;

        }

        public async Task<List<SaleOrderModel>> GetOrdersByFilter(OrderFilterModel model)
        {
            // Initialize the base SQL query
            if (model.StartDate == null)
            {
                model.StartDate = DateTime.Now.AddYears(-10);
            }
            if (model.EndDate == null)
            {
                model.EndDate = DateTime.Now.AddYears(10);
            }

            var query = $"exec GET_MSRAFOrderList2 '{model.OrderNo}','{model.CurrAccCode}','{model.CustomerName}','{model.SellerCode}','{model.StartDate:yyyy-MM-dd}','{model.EndDate:yyyy-MM-dd}'";
            List<SaleOrderModel> saleOrderModel = await _context.SaleOrderModels.FromSqlRaw(query).ToListAsync();

            return saleOrderModel;

        }

        public async Task<List<CustomerModel>> GetCustomerList(int customerType) //çalışıyor
        {

            List<CustomerModel> customerModel = await _context.ztCustomerModel.FromSqlRaw($"ms_GetCustomerList '{customerType}'").ToListAsync(); //3 dicez

            return customerModel;

        }
        public async Task<List<ProductOfOrderModel>> GetPurchaseOrderSaleDetail(string orderNumber)
        {


            List<ProductOfOrderModel> orderSaleDetails = await _context.ztProductOfOrderModel.FromSqlRaw($"GET_MSRAFSalesOrderDetailBP'{orderNumber}'").ToListAsync();
            orderSaleDetails = orderSaleDetails.OrderByDescending(p => p.Quantity).ToList();

            return orderSaleDetails;


        }
        public async Task<List<CollectedProduct>> GetCollectedOrderProducts(string orderNumber) //sayılan ürünler
        {


            List<CollectedProduct>? collectedProduct = await _context.CollectedProducts?.FromSqlRaw($"exec [GET_MSRafCollectedProducts] '{orderNumber}'").ToListAsync();
            return collectedProduct;
            //1-BP-2-117


        }

        public async Task<bool> SetInventoryByOrderNumber(String orderNumber)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
            string query = $"exec GET_MSRAFGetInventoryFromOrderNumber '{orderNumber}'";


            List<CountConfirmData> model1 = await _context.CountConfirmData.FromSqlRaw(query).ToListAsync();
            CountConfirmData data = model1[0];

            if (data.Lines == null)
            {

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Sayım Zaten Eşitlenmiştir");
                return true;

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
                    string sessionID = await _gs.ConnectIntegrator();
                    if (sessionID == null)
                    {

                        await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"SessionId Alınamadı");
                        throw new Exception("SessionId Alınamadı");
                    }

                    var response = await _gs.PostNebimAsync(json, "STOK EKLEME");

                    if (response == null)
                    {

                        //await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Response Alınamadı");
                        throw new Exception("Response Alınamadı");
                    }
                    else
                    {
                        await _ls.LogOrderSuccess($"{methodName} Başarılı", $"{model1.First().Lines}");
                        return true;
                    }



                }

            }
            return true;


        }
        public async Task<List<SaleOrderModel>> GetPurchaseOrders( )
        {

            List<SaleOrderModel> saleOrderModel = await _context.SaleOrderModels.FromSqlRaw("exec GET_MSRAFOrderBPList").ToListAsync();

            return saleOrderModel;

        }
        public async Task<List<SaleOrderModel>> GetPurchaseOrdersByFilter(OrderFilterModel model)
        {

           

            var query = $"exec GET_MSRAFOrderBPList2  '{model.OrderNo}','{model.CurrAccCode}','{model.CustomerName}','{model.StartDate:yyyy-MM-dd}','{model.EndDate:yyyy-MM-dd}'";
            List<SaleOrderModel> saleOrderModel = await _context.SaleOrderModels.FromSqlRaw(query).ToListAsync();

            return saleOrderModel;

        }

        public async Task<List<SaleOrderModel>> GetSaleOrdersById(string id)
        {

            List<SaleOrderModel> saleOrderModel = await _context.SaleOrderModels.FromSqlRaw($"exec GET_MSRAFOrderListID '{id.Split(' ')[0]}' ").ToListAsync();

            return saleOrderModel;

        }

        public async Task<BarcodeAddModel> AddSaleBarcode(BarcodeAddModel model)
        {

            var addedEntity = _context.Entry(model);

            addedEntity.State =
              EntityState
              .Added;
            _context.SaveChanges();

            return model;

        }
        public async Task<List<ProductOfOrderModel>> GetProductsOfOrders(int numberOfList)
        {


            List<ProductOfOrderModel> productModels = await _context.ztProductOfOrderModel.FromSqlRaw($"exec   GET_MSRAFOrderCollect {numberOfList} ").ToListAsync();
            //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
            return productModels;

        }
        public async Task<int> SetStatusOfPackages(List<ProductOfOrderModel> models)
        {

            string query = $"[dbo].[UPDATE_MSRAFPackageUpdate] '{models.First().PackageNo}','false'";
            int count = await _context.Database.ExecuteSqlRawAsync(query);

            return count;


        }
        public async Task<List<ReadyToShipmentPackageModel>> GetReadyToShipmentPackages( )
        {

            List<ReadyToShipmentPackageModel> models = new List<ReadyToShipmentPackageModel>();
            string query = $" [dbo].[GET_MSRAFPackageList] 'false'";
            models = await _context.ztReadyToShipmentPackageModel.FromSqlRaw(query).ToListAsync();

            return models;

        }

        public async Task<int> PutReadyToShipmentPackagesStatusById(string id)
        {


            string query = $" [dbo].[usp_ztMSRafTakipUpdate] '{id}','true'";
            int affectedRows = await _context.Database.ExecuteSqlRawAsync(query);

            return affectedRows;

        }

        public async Task<List<ProductOfOrderModel>> GetOrderSaleDetail(string orderNumber)
        {


            List<ProductOfOrderModel> orderSaleDetails = await _context.ztProductOfOrderModel.FromSqlRaw($"GET_MSRAFSalesOrderDetail'{orderNumber}'").ToListAsync();

            return orderSaleDetails;


        }

        public async Task<List<ProductOfOrderModel>> GetMissingProductsOfOrder(string orderNumber)
        {
            //if (orderNumber.Contains("MIS-"))
            //{
            //    orderNumber = orderNumber.Split("MIS-")[1];
            //}

            var query = $"GET_MSRAFSalesOrderDetailMissing '{orderNumber}'";

            List<ProductOfOrderModel> orderSaleDetails = await _context.ztProductOfOrderModel.FromSqlRaw(query).ToListAsync();

            return orderSaleDetails;


        }


        public async Task<List<ProductOfOrderModel>> GetOrderSaleDetailByPackageId(string PackageId)
        {


            List<ProductOfOrderModel> orderSaleDetails = await _context.ztProductOfOrderModel.FromSqlRaw($"Get_MSSiparisToplaID '{PackageId}'").ToListAsync();
            //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
            return orderSaleDetails;


        }

    }
}

