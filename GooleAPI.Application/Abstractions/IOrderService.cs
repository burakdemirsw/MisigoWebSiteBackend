using GoogleAPI.Domain.Models.NEBIM;
using GoogleAPI.Domain.Models.NEBIM.Invoice;
using GoogleAPI.Domain.Models.NEBIM.Order;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooleAPI.Application.Abstractions
{
    public interface IOrderService
    {
        public Image QrCode(string id);

        public Task<Bitmap> GetOrderDetailsFromQrCode(string data);

        public Task<Bitmap> HtmlToImage(string html, string path, int width, int height, string type);

        public Task PrintWithoutDialog(Bitmap image);
        public void ClearFolder( );

        public Task PrintInvoice(string HtmlPath, int width, int height, string type);

        public  Task<Boolean> GenerateReceipt(List<string> orderNumbers);
        public Task<Boolean> AutoInvoice(string orderNumber, string procedureName, OrderBillingRequestModel model, HttpContext context);
        public Task<string> ConnectIntegrator( );
        public Task<List<SalesPersonModel>> GetAllSalesPersonModels( );
        public  Task<string> GetCurrentMethodName(string name);


        public  Task<string> GenerateBarcode_A(List<BarcodeModel_A> barcodes);

    }
}
