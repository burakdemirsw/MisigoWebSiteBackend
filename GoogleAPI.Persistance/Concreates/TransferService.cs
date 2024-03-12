using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Domain.Models.NEBIM.Shelf;
using GoogleAPI.Domain.Models.NEBIM.Warehouse;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection;

namespace GoogleAPI.Persistance.Concreates
{
    public class TransferService : ITransferService
    {
        private GooleAPIDbContext _context;
        private ILogService _ls;
        private IGeneralService _gs;


        public TransferService(GooleAPIDbContext context, ILogService ls, IGeneralService gs)
        {
            _context = context;
            _ls = ls;
            _gs = gs;
        }
        public async Task<List<TransferModel>> GetProductOfTrasfer(string orderNumber)
        {

            List<TransferModel> collectedProduct = await _context.TransferModel.FromSqlRaw($"exec [Get_ProductOfTrasfer] '{orderNumber}'").ToListAsync();
            return collectedProduct;

        }

        public async Task<List<FastTransferModel>> GetAllFastTransferModels( )
        {
            List<FastTransferModel> models = await _context.FastTransferModels.ToListAsync();
            ;
            return models;


        }

        public async Task<List<FastTransferModel>> GetFastTransferModelsByOperationId(string operationId)
        {

            List<FastTransferModel> models = await _context.FastTransferModels
              .Where(model => model.OperationId == operationId)
              .ToListAsync();

            return models;

        }
        public async Task<FastTransferModel> DeleteProductFromFastTransfer(DeleteProductOfCount deleteModel)
        {

            FastTransferModel? productToDelete = _context.FastTransferModels
              .FirstOrDefault(op => op.OperationId == deleteModel.OrderNumber && op.Barcode == deleteModel.ItemCode);
            return productToDelete;



        }
        public async Task<List<InventoryItemModel>> GetInventoryItem(string type)
        {

            if (type == "0")
            {
                List<InventoryItemModel> InventoryItemModels = await _context.InventoryItemModels.FromSqlRaw("GET_MSRafTransferToWarehouse").ToListAsync();

                return InventoryItemModels;
            }
            else if (type == "1")
            {
                List<InventoryItemModel> InventoryItemModels = await _context.InventoryItemModels.FromSqlRaw("GET_MSRafTransferToWarehouse_Full").ToListAsync();

                return InventoryItemModels;
            }
            else
            {
                return null;
            }


        }
        public async Task<List<CountConfirmData>> GetInventoryFromOrderNumber(String OrderNo)
        {

            List<CountConfirmData> list = new List<CountConfirmData>();
            list = await _context.CountConfirmData.FromSqlRaw($"exec GET_MSRAFGetInventoryFromOrderNumber '{OrderNo}'").ToListAsync();

            return list;


        }

        public async Task<List<AvailableShelf>> GetAvailableShelves( )
        {

            List<AvailableShelf> list = new List<AvailableShelf>();
            list = await _context.AvailableShelfs.FromSqlRaw($"exec Get_MSRAFWillBeCount").ToListAsync();

            return list;



        }

        public async Task<int> FastTransfer(FastTransferModel model)
        {

            int affectedRows = 0;
            var query = $"exec Usp_PostZtMSRAFSTOKTransfer '{model.Barcode}','{model.BatchCode}','{model.ShelfNo}','{model.Quantity}','{model.WarehouseCode}','{model.TargetShelfNo}'";
            Console.WriteLine(query);
            affectedRows += _context.Database.ExecuteSqlRaw(query);

            return affectedRows;


        }
        public async Task<List<OfficeModel>> GetOfficeModel( )
        {


            List<OfficeModel> officeCodes = await _context.ztOfficeModel.FromSqlRaw($"exec usp_MSOfis").ToListAsync();
            //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
            return officeCodes;

        }
        public async Task<List<WarehouseOfficeModel>> GetWarehouseModel(string officeCode)
        {



            List<WarehouseOfficeModel> warehouseModels = await _context.ztWarehouseModel.FromSqlRaw($"exec [usp_MTOfisDepo] '{officeCode}'").ToListAsync();
            //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
            return warehouseModels;

        }
        public async Task<List<WarehosueOperationListModel>> GetWarehosueOperationList(string status)
        {

            List<WarehosueOperationListModel> saleOrderModel = await _context.ztWarehosueOperationListModel.FromSqlRaw($"GET_GetWarehosueOperationList {status}").ToListAsync();

            return saleOrderModel;

        }

        public async Task<WarehosueOperationListModel> GetWarehosueOperationListByInnerNumber(string innerNumber)
        {

            List<WarehosueOperationListModel> saleOrderModels = await _context.ztWarehosueOperationListModel.FromSqlRaw($"GET_GetWarehosueOperationListByInnerNumber '{innerNumber}'").ToListAsync();
            if (saleOrderModels.Count > 0)
            {
                return saleOrderModels.First();

            }
            else
            {
                return null;
            }



        }
        public async Task<int> DeleteWarehouseTransferByOrderNumber(string id)
        {

            var affectedRow = _context.Database.ExecuteSqlRaw($"delete from ZTMSRAFSAYIM6 where OrderNumber = '{id}' ");
            return affectedRow;

        }
        public async Task<List<WarehosueTransferListModel>> GetWarehosueTransferList(WarehouseTransferListFilterModel model)
        {

            // Initialize the base query
            string query = "SELECT TOP 100 MAX(CONVERT(NVARCHAR(10), ItemDate, 120)) as OperationDate, SUM(Quantity) as Quantity, OrderNumber, WarehouseCode, ToWarehouseCode FROM ZTMSRAFSAYIM6";

            // Initialize filter clauses
            List<string> filterClauses = new List<string>();

            // Add filters based on model properties
            if (!string.IsNullOrEmpty(model.OrderNumber))
            {
                filterClauses.Add($"OrderNumber = '{model.OrderNumber}'");
            }
            if (!string.IsNullOrEmpty(model.WarehouseCode))
            {
                filterClauses.Add($"WarehouseCode = '{model.WarehouseCode}'");
            }
            if (!string.IsNullOrEmpty(model.ToWareHouseCode))
            {
                filterClauses.Add($"TOWareHouseCode = '{model.ToWareHouseCode}'");
            }
            if (model.OperationStartDate != null)
            {
                filterClauses.Add($"ItemDate >= '{model.OperationStartDate:yyyy-MM-dd}'");
            }
            if (model.OperationEndDate != null)
            {
                filterClauses.Add($"ItemDate <= '{model.OperationEndDate:yyyy-MM-dd}'");
            }

            // Combine filter clauses
            if (filterClauses.Count > 0)
            {
                string filterConditions = string.Join(" AND ", filterClauses);
                query += " WHERE " + filterConditions;
            }

            // Complete the query
            query += " GROUP BY OrderNumber, WarehouseCode, TOWareHouseCode,CONVERT(NVARCHAR(10), ItemDate, 120)";
            query += " ORDER BY CONVERT(NVARCHAR(10), ItemDate, 120) DESC;";

            // Execute the query and retrieve results
            List<WarehosueTransferListModel> warehouseTransferModel = await _context.WarehosueTransferListModel.FromSqlRaw(query).ToListAsync();

            return warehouseTransferModel;

        }

        public async Task<List<WarehosueOperationListModel>> GetWarehosueOperationListByFilter(WarehouseOperationListFilterModel model)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);


            // Initialize the base query
            string query = "SELECT * FROM ztTransferOnayla WHERE IsCompleted = 1 or  IsCompleted = 0";

            // Initialize filter clauses
            List<string> filterClauses = new List<string>();

            // Add filters based on model properties
            if (!string.IsNullOrEmpty(model.InnerNumber))
            {
                filterClauses.Add($"InnerNumber = '{model.InnerNumber}'");
            }
            if (model.StartDate != null)
            {
                filterClauses.Add($"OperationDate >= '{model.StartDate:yyyy-MM-dd}'");
            }
            if (model.EndDate != null)
            {
                filterClauses.Add($"OperationDate <= '{model.EndDate:yyyy-MM-dd}'");
            }

            // Combine filter clauses
            if (filterClauses.Count > 0)
            {
                string filterConditions = string.Join(" AND ", filterClauses);
                query += " AND " + filterConditions;
            }

            // Complete the query
            query += " ORDER BY InnerNumber DESC;";

            // Execute the query and retrieve results
            List<WarehosueOperationListModel> saleOrderModel = await _context.ztWarehosueOperationListModel.FromSqlRaw(query).ToListAsync();

            return saleOrderModel;

        }

        public async Task<List<ProductOfOrderModel>> GetWarehosueOperationDetail(string innerNumber)
        {
            List<ProductOfOrderModel> saleOrderModel = await _context.ztProductOfOrderModel.FromSqlRaw($"exec   [Usp_GETTransferOnayla]'{innerNumber}'").ToListAsync();

            return saleOrderModel;

        }


        public async Task<bool> TransferProducts(string orderNo)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);


            List<TransferData>? transferDatas = await _context.TransferData.FromSqlRaw($"exec usp_GetOrderForInvoiceToplu_WT '{orderNo}'").ToListAsync();
            TransferData transferData = transferDatas.First();
            if (transferData != null)
            {
                List<TransferItem>? transferItems = JsonConvert.DeserializeObject<List<TransferItem>>(transferData.Lines);
                TransferDataModel transferDataModel = new TransferDataModel
                {
                    ModelType = transferData.ModelType,
                    InnerNumber = transferData.InnerNumber,
                    OfficeCode = transferData.OfficeCode,
                    OperationDate = transferData.OperationDate,
                    StoreCode = transferData.StoreCode,
                    ToOfficeCode = transferData.ToOfficeCode,
                    ToStoreCode = transferData.ToStoreCode,
                    ToWarehouseCode = transferData.ToWarehouseCode,
                    WarehouseCode = transferData.WarehouseCode,
                    CompanyCode = transferData.CompanyCode,
                    InnerProcessType = transferData.InnerProcessType,
                    IsCompleted = Convert.ToBoolean(transferData.IsCompleted),
                    IsInnerOrderBase = Convert.ToBoolean(transferData.IsInnerOrderBase),
                    IsLocked = Convert.ToBoolean(transferData.IsLocked),
                    IsPostingJournal = Convert.ToBoolean(transferData.IsPostingJournal),
                    IsPrinted = Convert.ToBoolean(transferData.IsPrinted),
                    IsReturn = Convert.ToBoolean(transferData.IsReturn),
                    IsTransferApproved = Convert.ToBoolean(transferData.IsTransferApproved),
                    Lines = transferItems
                };

                var json = JsonConvert.SerializeObject(transferDataModel);

                var response = await _gs.PostNebimAsync(json, "TRANSFER");


                if (response != null)
                {
                    await _ls.LogWarehouseSuccess($"{methodName} BAŞARILI", "TRANSFER BAŞARILI");
                    return true;

                }
                else
                {
                    throw new Exception("nebim response null");
                }
            }
            else
            {
                throw new Exception("transferData null");

            }




        }

        public async Task<int> SendNebımToTransferProduct(WarehouseOperationProductModel model)
        {


            string sql = "EXECUTE Usp_PostZtMSRAFSTOK {0}, {1}, {2}, {3}, {4}";
            // string sql2 = $"update ztTransferOnayla set IsCompleted = 1 where InnerNumber = '{model.InnerNumber}'";
            object[] parameters = {
                      model.Barcode,
                      model.BatchCode,
                      model.ShelfNumber,
                      model.Quantity,
                      model.Warehouse
                    };

            int number = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

            // int number2 = _context.Database.ExecuteSqlRaw(sql2);

            return number;

        }

        public async Task<bool> ConfirmOperation(List<string> InnerNumberList)
        {

            foreach (var item in InnerNumberList)
            {
                string sql2 = $"update ztTransferOnayla set IsCompleted = 1 where InnerNumber = '{item}'";

                int number2 = _context.Database.ExecuteSqlRaw(sql2);
            }

            return true;

        }
        public async Task<List<TransferRequestListModel>> GetTransferRequestListModel(string type)
        {

            if (type == "0")
            {
                TransferRequestListModel model = new TransferRequestListModel();



                List<TransferRequestListModel> list = await _context.TransferRequestListModels.FromSqlRaw("[dbo].[Get_MSRAFTRANSFERShelf]").ToListAsync();

                return list;
            }
            else if (type == "1")
            {
                TransferRequestListModel model = new TransferRequestListModel();



                List<TransferRequestListModel> list = await _context.TransferRequestListModels.FromSqlRaw($"[dbo].[Get_MSRAFTRANSFERShelf_Full] '{type}' ").ToListAsync();

                return list;

            }
            else if (type == "2")
            {
                TransferRequestListModel model = new TransferRequestListModel();



                List<TransferRequestListModel> list = await _context.TransferRequestListModels.FromSqlRaw($"[dbo].[Get_MSRAFTRANSFERShelf_canta] ").ToListAsync();

                return list;
            }
            else
            {
                return null;
            }



        }
        public async Task<List<BarcodeModel>> GetOperationWarehousue(string innerNumber)
        {

            List<BarcodeModel> barcodeModels = await _context.BarcodeModels.FromSqlRaw($"usp_QRKontrolSorgula '{innerNumber}'").ToListAsync();
            return barcodeModels;


        }

    }
}