using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.NEBIM.Address;
using GoogleAPI.Domain.Models.NEBIM.Customer;
using GoogleAPI.Domain.Models.NEBIM.Customer.CreateCustomerModel;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Order.CreateOrderModel;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Domain.Models.NEBIM.Request;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate.Criterion;
using Remotion.Linq.Clauses;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Text;
using ZXing;

namespace GoogleAPI.Persistance.Concretes
{
    public class OrderService : IOrderService // IOrderService bir arayüz olabilir, işlevinizi tanımlar
    {

        private GooleAPIDbContext _context;
        private IGeneralService _gs;
        private ILogService _ls;
        private IConfiguration _congfiguration;
        public OrderService(GooleAPIDbContext context, ILogService ls, IGeneralService gs, IConfiguration congfiguration)
        {
            _context = context;
            _ls = ls;
            _gs = gs;
            _congfiguration = congfiguration;
        }

        public Task<Bitmap> GetOrderDetailsFromQrCode(string data)
        {
            throw new NotImplementedException();
        }
        public async Task<List<SaleOrderModel>> GetSaleOrders(int status, int invoiceStatus) //toplanabilr 
        {

            List<SaleOrderModel> saleOrderModel = await _context.SaleOrderModels.FromSqlRaw($"exec GET_MSRAFOrderList5 {status},'{invoiceStatus}'").ToListAsync();

            return saleOrderModel;

        }
        public async Task<List<SaleOrderModel>> GetSaleOrdersWithMissingItems( ) //çalışıyor
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

        public async Task<List<CustomerList_VM>> GetCustomerList_2(GetCustomerList_CM request)
        {

            var query = "select * from MSG_MusteriList() where ";
            if (!String.IsNullOrEmpty(request.Mail))
            {
                query += $" Mail like '%{request.Mail}%'";
            }
            if (!String.IsNullOrEmpty(request.Phone))
            {
                query += $" Phone like '%{request.Phone}%'";
            }
            if (!String.IsNullOrEmpty(request.CurrAccCode))
            {
                query += $" CurrAccCode like '%{request.CurrAccCode}%' or CurrAccDescription like '%{request.CurrAccCode}%'";
            }
            if (String.IsNullOrEmpty(request.Mail) && String.IsNullOrEmpty(request.Phone) && String.IsNullOrEmpty(request.CurrAccCode))
            {
                query = "select * from MSG_MusteriList()";
            }
            List<CustomerList_VM> customerList = _context.CustomerList_VM.FromSqlRaw(query).ToList();

            return customerList;

        }

        public async Task<List<CustomerAddress_VM>> GetCustomerAddress(GetCustomerAddress_CM request)
        {
            var query = "select * from MSG_MusteriAdres() where ";
            if (!String.IsNullOrEmpty(request.CurrAccCode))
            {
                query += $" CurrAccCode like '%{request.CurrAccCode}%'";
            }
            else
            {
                query = "select * from MSG_MusteriAdres()";
            }


            List<CustomerAddress_VM> addressList = _context.CustomerAddress_VM.FromSqlRaw(query).ToList();

            return addressList;
        }

        public async Task<CreateCustomer_ResponseModel> SendCustomerToNebim(CreateCustomer_CM request)
        {

           
                NebimCustomer customer = new NebimCustomer();
                customer.ModelType = 2;
                customer.CurrAccCode = "";
                customer.CurrAccDescription = request.CurrAccDescription;
                customer.TaxNumber = request.TaxNumber;
                customer.OfficeCode = request.OfficeCode;
                customer.TaxOfficeCode = request.TaxOfficeCode;
                customer.MersisNum = request.MersisNum;
                customer.RetailSalePriceGroupCode = "";
                customer.IdentityNum = "11111111111";
                customer.CreditLimit = 0;
                customer.CurrencyCode = "TRY";
                customer.PostalAddresses = new List<PostalAddress>();
                var postalAddress = request.Address != null ?  await CreatePostallAdress(request.Address,true) : await CreatePostallAdress(request.Address, false);
            
                customer.PostalAddresses.Add(postalAddress);

                customer.Communications = new List<Communication>();
                Communication Communication = new Communication();
                Communication.CommAddress = request.PhoneNumber;
                Communication.CommunicationTypeCode = 7;

                if (Communication.CommAddress != null)
                {
                    customer.Communications.Add(Communication);

                }
                Communication Communication2 = new Communication();
                Communication2.CommAddress = request.Mail;
                Communication2.CommunicationTypeCode = 3;

                if (Communication2.CommAddress != null)
                {
                    customer.Communications.Add(Communication2);

                }

                var json = JsonConvert.SerializeObject(customer);

                var response = await _gs.PostNebimAsync(json, "MÜŞTERİ");
                JObject jsonResponse = JObject.Parse(response);
                string CurrAccCode = jsonResponse["CurrAccCode"].ToString();
                CreateCustomer_ResponseModel responseModel = new CreateCustomer_ResponseModel();
                responseModel.CurrAccCode = CurrAccCode;
                return responseModel;
         



            
        }


        public async Task<PostalAddress> CreatePostallAdress(Address address , bool type )
        {

            if(type == true)
            {
                PostalAddress PostalAddress = new PostalAddress();

                PostalAddress.AddressTypeCode = "1";
                PostalAddress.CountryCode = address.Country;
                PostalAddress.StateCode = address.Region;
                PostalAddress.CityCode = address.Province;
                PostalAddress.DistrictCode = address.District;
                PostalAddress.Address = address.Description;


                return PostalAddress;

            }
            else
            {
                PostalAddress PostalAddress = new PostalAddress();

                PostalAddress.AddressTypeCode = "1";
                PostalAddress.CountryCode = "TR";
                PostalAddress.StateCode = "TR.MR";
                PostalAddress.CityCode = "TR.34";
                PostalAddress.DistrictCode = "TR.03408";
                PostalAddress.Address = "Molla Gürani, Uygar Sokağı No:17 A, Fatih İstanbul Türkiye";

                return PostalAddress;

            }



        }

  

        public async  Task<NebimOrder_RM> CreateOrder(NebimOrder Order)
        {

            string content = Newtonsoft.Json.JsonConvert.SerializeObject(Order);

            var response = await _gs.PostNebimAsync(content, "SİPARİŞ");
            JObject jsonResponse = JObject.Parse(response);

            string OrderNumber = jsonResponse[
                 "OrderNumber"
             ].ToString();
            await UpdateClientOrderNebimInfos(Order.InternalDescription,OrderNumber,true);
            NebimOrder_RM _response = new NebimOrder_RM();
            _response.OrderNumber = OrderNumber;
            _response.Status = true;

            return _response;
        }

        public async Task<ClientOrder_DTO> GetClientOrder(Guid id)
        {
            ClientOrder_DTO clientOrder_DTO = new ClientOrder_DTO();
            clientOrder_DTO.ClientOrder = await _context.msg_ClientOrders.FirstOrDefaultAsync(o => o.Id == id);
            clientOrder_DTO.ClientOrderBasketItems =await  _context.msg_ClientOrderBasketItems.Where(i=>i.OrderId == id).ToListAsync();

            return clientOrder_DTO;
        }

        public async Task<bool> CreateClientOrder( ClientOrder request)
        {
            ClientOrder? clientOrder = await _context.msg_ClientOrders.FirstOrDefaultAsync(o => o.Id == request.Id);
            if (clientOrder == null)
            {
                var response = await _context.msg_ClientOrders.AddAsync(request);
                Boolean state = response.State == EntityState.Added;
                await _context.SaveChangesAsync();
                return state;
            }
            else
            {
                return true;
            }
               
        }
        public async Task<bool> DeleteClientOrder(Guid Id)
        {
            ClientOrder? clientOrder = await _context.msg_ClientOrders.FirstOrDefaultAsync(o => o.Id == Id);
            if (clientOrder != null)
            {
                var response =  _context.msg_ClientOrders.Remove(clientOrder);
                Boolean state = response.State == EntityState.Deleted;
                await _context.SaveChangesAsync();

                List<ClientOrderBasketItem>? clientOrderBasketItems =  _context.msg_ClientOrderBasketItems.Where(o => o.OrderId == Id).ToList();
                
                if (clientOrderBasketItems.Count>0)
                {

                    foreach (var item in clientOrderBasketItems)
                    {
                        var _response = _context.msg_ClientOrderBasketItems.Remove(item);
                        Boolean _state = response.State == EntityState.Deleted;
                        await _context.SaveChangesAsync();

                    }


                    return state;
                }
                else
                {
                    return false;
                }
                

            }
            else
            {
                return false;
            }

        }
        public async Task<bool> CreateClientOrderBasketItem(ClientOrderBasketItem request)
        {
          
                ClientOrder? clientOrder = await _context.msg_ClientOrders.FirstOrDefaultAsync(o => o.Id == request.OrderId);
                if (clientOrder != null)
                {
                ClientOrderBasketItem? clientOrderBasketItem = await _context.msg_ClientOrderBasketItems.FirstOrDefaultAsync(o => o.OrderId == request.OrderId && o.LineId == request.LineId);
                if (clientOrderBasketItem != null)
                {
                    clientOrderBasketItem.Quantity += 1;
                    var response =  _context.msg_ClientOrderBasketItems.Update(clientOrderBasketItem);
                    Boolean state = response.State == EntityState.Modified;
                     _context.SaveChanges();
                    return state;
                }
                else
                {
                    var response = await _context.msg_ClientOrderBasketItems.AddAsync(request);
                    Boolean state = response.State == EntityState.Added;
                     _context.SaveChanges();
                    return state;
                }
              
                }
                else
                {
                    throw new Exception("Bu Siparişe Ait Kayıt Bulunamadı");
                }

           
        
        }

        public async Task<List<ClientCustomer>> GetClientCustomer(string AddedSalesPersonCode)
        {

            List<ClientCustomer>? clientCustomers = await _context.msg_ClientCustomers.Where(c=>c.AddedSellerCode == AddedSalesPersonCode).ToListAsync();


            return clientCustomers;

        }


        public async Task<bool> EditClientCustomer(ClientCustomer request)
        {

            ClientCustomer? clientCustomer = await _context.msg_ClientCustomers.FirstOrDefaultAsync(o => o.Id == request.Id);
            if (clientCustomer != null) //güncelle
            {
                clientCustomer.AddedSellerCode = request.AddedSellerCode;
                clientCustomer.Description = request.Description;
                clientCustomer.CurrAccCode = request.CurrAccCode;
                clientCustomer.BussinesCardPhotoUrl = request.BussinesCardPhotoUrl;
                clientCustomer.StampPhotoUrl = request.StampPhotoUrl;
                var response =  _context.msg_ClientCustomers.Update(request);
                Boolean state = response.State == EntityState.Modified;
                _context.SaveChanges();
                return state;
            }
            else
            {
                var response = await _context.msg_ClientCustomers.AddAsync(request);
                Boolean state = response.State == EntityState.Added;
                _context.SaveChanges();
                return state;
            }



        }


        public async Task<bool> DeleteClientOrderBasketItem(Guid orderId , Guid lineId )
        {
            ClientOrderBasketItem? clientOrderBasketItem = await _context.msg_ClientOrderBasketItems.FirstOrDefaultAsync(o => o.OrderId == orderId && o.LineId ==lineId);
            if (clientOrderBasketItem != null)
            {
                  _context.msg_ClientOrderBasketItems.Remove(clientOrderBasketItem);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new Exception("Bu Siparişe Ait Kayıt Bulunamadı");
            }

        }

        public async Task<bool> UpdateClientOrderBasketItem(Guid orderId, Guid lineId,int quantity,decimal price)
        {
            ClientOrderBasketItem? clientOrderBasketItem = await _context.msg_ClientOrderBasketItems.FirstOrDefaultAsync(o => o.OrderId == orderId && o.LineId == lineId);
            if (clientOrderBasketItem != null)
            {
                clientOrderBasketItem.Quantity = quantity;
                clientOrderBasketItem.Price = price;    
                _context.msg_ClientOrderBasketItems.Update(clientOrderBasketItem);

                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new Exception("Bu Siparişe Ait Kayıt Bulunamadı");
            }

        }

        public async Task<bool> UpdateClientOrderPayment(Guid orderId, string paymetnDescription)
        {
            ClientOrder? clientOrder = await _context.msg_ClientOrders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (clientOrder != null)
            {
                clientOrder.PaymentDescription = paymetnDescription;
              
                _context.msg_ClientOrders.Update(clientOrder);
                    
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new Exception("Bu Siparişe Ait Kayıt Bulunamadı");
            }

        }
        public async Task<bool> UpdateClientOrderNebimInfos(string orderNo,string orderNumber, bool isCompleted)
        {
            ClientOrder? clientOrder = await _context.msg_ClientOrders.FirstOrDefaultAsync(o => o.OrderNo == orderNo);
            if (clientOrder != null)
            {
                clientOrder.OrderNumber = orderNumber;
                clientOrder.IsCompleted = true;
                _context.msg_ClientOrders.Update(clientOrder);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new Exception("Bu Siparişe Ait Kayıt Bulunamadı");
            }

        }


        public async Task<bool> AddCustomerAddress(AddCustomerAddress_CM request)
        {

            string content = Newtonsoft.Json.JsonConvert.SerializeObject(request);

            var response = await _gs.PostNebimAsync(content, "MÜŞTERİ");
            Console.WriteLine(response.ToString());
            return true;
        }
    }
}

