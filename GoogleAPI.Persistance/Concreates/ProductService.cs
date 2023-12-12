using GoogleAPI.Domain.Models.NEBIM;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;

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

        public async Task<string> GenerateBarcode_A(List<BarcodeModel_A> barcodes)
        {
            try
            {
                foreach (var barcode in barcodes)
                {
                    string html = await File.ReadAllTextAsync("C:\\code\\barcode_a.txt");
                    var topBarcodeArea = _gs.ConvertImageToBase64(_gs.QrCode(barcode.TopBarcodeJSON));
                    var productBarcodeArea = _gs.ConvertImageToBase64(_gs.QrCode(barcode.ProductBarcodeJSON));
                    html = html.Replace("[DATEAREA]", barcode.DateArea?.ToString("dd/MM/yyyy HH:mm"));
                    html = html.Replace("[REFAREA]", barcode.RefArea);
                    html = html.Replace("[QTYAREA]", barcode.QtyArea.ToString());
                    html = html.Replace("[LOTAREA]", barcode.LotArea);

                    html = html.Replace("[ADDRESSAREA]", barcode.AddressArea);
                    html = html.Replace("[ITEMAREA]", barcode.ItemArea);
                    // Barkod resmi için base64 kodunu buraya ekleyin
                    html = html.Replace("[BOXAREA]", barcode.BoxArea);
                    html = html.Replace("[TOPBARCODEAREA]", topBarcodeArea);
                    html = html.Replace("[PRODUCTBARCODEAREA]", productBarcodeArea);
                    // HTML dosyasını C:\code\ klasörüne kaydet

                    return html;
                    //string filePath = $"C:\\code\\barcodes\\htmlPages\\{Guid.NewGuid().ToString()}.html";
                    //File.WriteAllText(filePath, html);

                    //await PrintInvoice(filePath, 600, 300, "barcode_a");
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;

            }

        }


    }
}
