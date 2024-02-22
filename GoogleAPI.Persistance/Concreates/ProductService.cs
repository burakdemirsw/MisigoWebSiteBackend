using GoogleAPI.Domain.Models.NEBIM;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GoogleAPI.Persistance.Concreates
{
    public class ProductService : IProductService
    {
        private GooleAPIDbContext _context;
        private IGeneralService _gs;
        private ILogService _ls;

        public ProductService(GooleAPIDbContext context, ILogService ls, IGeneralService gs)
        {
            _context = context;
            _ls = ls;
            _gs = gs;
        }

        public async Task<List<BarcodeModel>> GetBarcodeDetail(string qrCode)
        {

            
                List<BarcodeModel>? barcodeModels = await _context.BarcodeModels.FromSqlRaw($"usp_QRKontrolSorgula '{qrCode}'").ToListAsync();
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                return barcodeModels;
           
        }


        public async Task<string> GenerateBarcode_A(List<BarcodeModel_A> barcodes)
        {

            foreach (var barcode in barcodes)
            {
                string html = await File.ReadAllTextAsync("C:\\code\\barcode_a.txt");
                //var topBarcodeArea = _gs.ConvertImageToBase64(_gs.QrCode(barcode.TopBarcodeJSON));
                var productBarcodeArea = _gs.ConvertImageToBase64(_gs.QrCode("https://www.davye.com"));
                html = html.Replace("[DATEAREA]", barcode.DateArea?.ToString("dd/MM/yyyy HH:mm"));
                html = html.Replace("[REFAREA]", barcode.RefArea);
                html = html.Replace("[QTYAREA]", barcode.QtyArea.ToString());
                html = html.Replace("[LOTAREA]", barcode.LotArea);

                html = html.Replace("[ADDRESSAREA]", barcode.AddressArea);
                html = html.Replace("[ITEMAREA]", barcode.ItemArea);
                // Barkod resmi için base64 kodunu buraya ekleyin
                html = html.Replace("[BOXAREA]", barcode.BoxArea);
                html = html.Replace("[TOPBARCODEAREA]", $"http://212.156.46.206:7676/qrphoto/{barcode.ItemCode}.jpg");
                html = html.Replace("[PRODUCTBARCODEAREA]", productBarcodeArea);
                // HTML dosyasını C:\code\ klasörüne kaydet

                return html;
                //string filePath = $"C:\\code\\barcodes\\htmlPages\\{Guid.NewGuid().ToString()}.html";
                //File.WriteAllText(filePath, html);

                //await PrintInvoice(filePath, 600, 300, "barcode_a");
            }

            return null;



        }


    }
}
