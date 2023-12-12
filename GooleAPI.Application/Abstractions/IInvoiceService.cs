using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.NEBIM.Invoice;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Product;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Persistance.Concreates
{
    public interface IInvoiceService
    {
        Task<bool> AutoInvoice(string orderNumber, string procedureName, OrderBillingRequestModel requestModel, HttpContext context);
        Task<List<SalesPersonModel>> GetAllSalesPersonModels( );
        Task<List<SalesPersonModel>> GetSalesPersonModels( );
        Task<List<CountListModel>> GetInvoiceList( );
        Task<List<CountListModel>> GetInvoiceListByFilter(InvoiceFilterModel model);
        Task<List<CreatePurchaseInvoice>> GetProductOfInvoice(string invoiceId);
        Task<bool> BillingOrder(OrderBillingRequestModel model, HttpContext httpContext);
        Task<bool> DeleteInvoiceProducts(string orderNumber);
    }
}
