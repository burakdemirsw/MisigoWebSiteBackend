using FluentNHibernate.Conventions.Inspections;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Domain.Models.NEBIM.Shelf;
using GoogleAPI.Domain.Models.NEBIM.Warehouse;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
          
                List<FastTransferModel> models =await  _context.FastTransferModels
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
        public async Task<List<InventoryItemModel>> GetInventoryItem( )
        {
          
                List<InventoryItemModel> InventoryItemModels = await _context.InventoryItemModels.FromSqlRaw("GET_MSRafTransferToWarehouse").ToListAsync();

            return InventoryItemModels;

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
                affectedRows += await _context.Database.ExecuteSqlRawAsync(query);

                return affectedRows;

           
        }
    }
}
