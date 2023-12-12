using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Domain.Models.NEBIM.Shelf;
using GoogleAPI.Domain.Models.NEBIM.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Persistance.Concreates
{
    public interface ITransferService
    {
        Task<List<TransferModel>> GetProductOfTrasfer(string orderNumber);
        Task<int> FastTransfer(FastTransferModel model);
        Task<List<FastTransferModel>> GetAllFastTransferModels( );
        Task<List<FastTransferModel>> GetFastTransferModelsByOperationId(string operationId);
        Task<FastTransferModel> DeleteProductFromFastTransfer(DeleteProductOfCount deleteModel);
        Task<List<InventoryItemModel>> GetInventoryItem( );
        Task<List<CountConfirmData>> GetInventoryFromOrderNumber(String OrderNo);
        Task<List<AvailableShelf>> GetAvailableShelves( );
    }
}
