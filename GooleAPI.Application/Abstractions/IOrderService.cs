using GoogleAPI.Domain.Models.NEBIM.Invoice;
using GoogleAPI.Domain.Models.NEBIM.Order;
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

        public Task<Bitmap> HtmlToImage(string html, string path);

        public Task PrintWithoutDialog(Bitmap image);
        public void ClearFolder( );

        public void PrintInvoice(string HtmlPath);

        public  Task<Boolean> GenerateReceipt(List<string> orderNumbers);
        public Task<Boolean> AutoInvoice(string orderNumber,string procedureName,OrderBillingRequestModel model);
        public Task<string> ConnectIntegrator( );
        public Task<List<SalesPersonModel>> GetAllSalesPersonModels( );




    }
}
