using GoogleAPI.Domain.Models.NEBIM.Invoice;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Request;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PuppeteerSharp;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Text;

namespace GoogleAPI.Persistance.Concreates
{
    public class GeneralService : IGeneralService
    {
        private GooleAPIDbContext _context;
        private ILogService _ls;

        public GeneralService(GooleAPIDbContext context, ILogService ls)
        {
            _context = context;
            _ls = ls;
        }


        public async Task<string> ConnectIntegrator( )
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(
                  "http://192.168.2.36:7676" + "/IntegratorService/Connect"
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                VM_HttpConnectionRequest? session =
                  JsonConvert.DeserializeObject<VM_HttpConnectionRequest>(responseBody);
                if (session != null)
                {
                    if (session.SessionId != null)
                    {
                        string sessionId = session.SessionId;
                        // //Console.WriteLine(responseBody);
                        return sessionId;
                    }
                }
                return null;

            }
            catch (Exception ex)
            {
                await _ls.LogOrderWarn($"NEBIM INTEGRATOR CONNECT ERROR", $"{ex.Message}");
                return null;
            }
        }
        public async Task<string> PostNebimAsync(string content, string OperationType)
        {
            string sessionID = await ConnectIntegrator();
            //Console.WriteLine($" \n\n\nContent :{DateTime.Now}" + content+ " \n\n\n");

            using (var httpClient = new HttpClient())
            {
                var json = new StringContent(
                content,
                Encoding.UTF8,
                "application/json"
            );

                var response = await httpClient.PostAsync(
                    $"http://192.168.2.36:7676/(S({sessionID}))/IntegratorService/post?",
                    json
                );

                // Yanıt alındığında işleme devam et
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();


                    ErrorResponseModel? erm = JsonConvert.DeserializeObject<ErrorResponseModel>(result);

                    if (erm != null)
                    {
                        if (erm.StatusCode == 400)
                        {
                            if (OperationType.Contains("SAYIM"))
                            {
                                await _ls.LogWarehouseWarn($"GELEN HATALI İSTEK", content);
                            }
                            else if (OperationType.Contains("SİPARİŞ"))
                            {
                                await _ls.LogOrderWarn($"GELEN HATALI İSTEK", content);
                            }
                            else if (OperationType.Contains("FATURA"))
                            {
                                await _ls.LogWarehouseWarn($"GELEN HATALI İSTEK", content);
                            }
                            else if (OperationType.Contains("TRANSFER"))
                            {
                                await _ls.LogWarehouseWarn($"GELEN HATALI İSTEK", content);
                            }
                            else if (OperationType.Contains("STOK"))
                            {
                                await _ls.LogWarehouseWarn($"GELEN HATALI İSTEK", content);
                            }
                            throw new Exception(OperationType+":"+ erm.ExceptionMessage);
                        }
                    }
                 

                    if (OperationType.Contains("SAYIM"))
                    {
                        await _ls.LogWarehouseSuccess($"{OperationType} GELEN İSTEK: ", content);
                        await _ls.LogWarehouseSuccess($"{OperationType} NEBIM RESPONSE: ", result);
                    }
                    else if (OperationType.Contains("SİPARİŞ"))
                    {
                        await _ls.LogOrderSuccess($"{OperationType} GELEN İSTEK: ", content);
                        await _ls.LogOrderSuccess($"{OperationType} NEBIM RESPONSE: ", result);
                    }
                    else if (OperationType.Contains("FATURA"))
                    {
                        await _ls.LogInvoiceSuccess($"{OperationType} GELEN İSTEK: ", content);
                        await _ls.LogInvoiceSuccess($"{OperationType} NEBIM RESPONSE: ", result);
                    }
                    else if (OperationType.Contains("TRANSFER"))
                    {
                        await _ls.LogWarehouseSuccess($"{OperationType} GELEN İSTEK: ", content);
                        await _ls.LogWarehouseSuccess($"{OperationType} NEBIM RESPONSE: ", result);
                    }
                    else if (OperationType.Contains("STOK"))
                    {
                        await _ls.LogWarehouseSuccess($"{OperationType} GELEN İSTEK: ", content);
                        await _ls.LogWarehouseSuccess($"{OperationType} NEBIM RESPONSE: ", result);
                    }

                    return result;
                }
                else
                {

                    return null;
                }
            }
        }
        public async Task<string> GetCurrentMethodName(string name)
        {

            int start = name.IndexOf('<');
            int end = name.IndexOf('>');
            string result = name.Substring(start + 1, end - start - 1);
            return result;
        }
        public async Task ClearFolder( )
        {
            string[] htmlFiles = Directory.GetFiles("C:\\code\\", "*.html");
            foreach (string htmlFile in htmlFiles)
            {
                File.Delete(htmlFile);
            }

            // Tüm .png dosyalarını sil
            string[] pngFiles = Directory.GetFiles("C:\\code\\", "*.png");
            foreach (string pngFile in pngFiles)
            {
                File.Delete(pngFile);
            }
        }
        public Task PrintWithoutDialog(Bitmap image)
        {
            try
            {
                return Task.Run(( ) =>
                {
                    System.Drawing.Printing.PrintDocument printDocument = new System.Drawing.Printing.PrintDocument();
                    // Set margins to 0 
                    printDocument.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
                    printDocument.PrintPage += (s, args) =>
                    {
                        args.Graphics.DrawImage(image, new System.Drawing.Point(0, 0));
                    };
                    printDocument.PrintController = new StandardPrintController();
                    printDocument.Print();
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Yazdırma Aşamasında Hata Alındı : {ex.Message}  ");

            }

        }

        public Image QrCode(string id)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(id.ToString(), QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);

            // Özel bir boyut kullanarak QR kodunu oluşturun
            int desiredSize = 60;
            byte[] qrCodeBytes = qrCode.GetGraphic(desiredSize);

            // Oluşturulan QR kodunu bir 50x50 piksel boyutlu bir resme dönüştürün
            using (MemoryStream ms = new MemoryStream(qrCodeBytes))
            {
                Image qrCodeImage = Image.FromStream(ms);
                Bitmap resizedImage = new Bitmap(qrCodeImage, new Size(desiredSize, desiredSize));
                return resizedImage;
            }
        }

        public string ConvertImageToBase64(Image image)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Png);
                byte[] imageBytes = memoryStream.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public async Task<Boolean> GenerateReceipt(List<string> orderNumbers)
        {

            //gelen fatura numaralarını sp den verilerini çek Receipe Model e eşitle
            List<RecepieModel> recepieModels = new List<RecepieModel>();


            try
            {

                foreach (var order in orderNumbers)
                {
                    List<Domain.Models.NEBIM.Order.Invoice> invoiceModel = new List<Domain.Models.NEBIM.Order.Invoice>();

                    invoiceModel = await _context.ztInvoice.FromSqlRaw($"exec GET_MSRAFInvoiceDetail '{order}'").ToListAsync();

                    Image barcodeBitmap = QrCode(order);
                    string barcodeBase64 = ConvertImageToBase64(barcodeBitmap);
                    string EInvoiceNumber = order;

                    RecepieModel recepie = new RecepieModel();
                    recepie.EInvoiceNumber = EInvoiceNumber;
                    recepie.OrderNo = order;
                    recepie.OrderDate = invoiceModel.First().OrderDate;
                    recepie.PaymentType = invoiceModel.First().PaymentType;
                    recepie.ProductRecepieModel = new List<ProductRecepieModel>();
                    recepie.TotalVAT = 0;
                    recepie.CustomerName = invoiceModel.First().CustomerDescription;
                    recepie.OrderNoBase64String = barcodeBase64;
                    recepie.TotalValue = 0;
                    ;
                    foreach (var product in invoiceModel)
                    {

                        ProductRecepieModel productRecepieModel = new ProductRecepieModel();
                        productRecepieModel.ItemCode = product.ItemCode;
                        productRecepieModel.Price = Convert.ToDouble(product.Price);
                        productRecepieModel.Quantity = product.Qty1;
                        recepie.ProductRecepieModel.Add(productRecepieModel);
                    }

                    recepieModels.Add(recepie);
                }

                foreach (var recepie in recepieModels)
                {
                    string html = File.ReadAllText("C:\\code\\outfitRecepie.txt");

                    html = html.Replace("[TarihSaatFişKodu]", recepie.OrderDate.ToString("dd/MM/yyyy HH:mm") + " - " + recepie.OrderNo + " - " + recepie.EInvoiceNumber);

                    string tableRows = "";
                    foreach (var product in recepie.ProductRecepieModel)
                    {
                        string row = "<tr>" +
                          "<td>" + product.ItemCode + "</td>" +
                          "<td>" + product.Quantity + "</td>" +
                          "<td>" + (product.Price / product.Quantity).ToString("0.00") + "₺" + "</td>" +
                          "<td>" + (product.Price).ToString("0.00") + "₺" + "</td>" +
                          "</tr>";
                        tableRows += row;
                    }

                    html = html.Replace("{AliciAdi}", recepie.CustomerName);

                    html = html.Replace("[TabloSatirlari]", tableRows);
                    html = html.Replace("[ToplamTutar]", recepie.TotalValue.ToString() + "₺" + " Toplam KDV :" + recepie.TotalVAT.ToString("0.00") + "₺");
                    html = html.Replace("[BarcodeBase64]", recepie.OrderNoBase64String); // Barkod resmi için base64 kodunu buraya ekleyin

                    // HTML dosyasını C:\code\ klasörüne kaydet

                    string filePath = $"C:\\code\\{recepie.EInvoiceNumber}-{Guid.NewGuid().ToString()}.html";
                    File.WriteAllText(filePath, html);
                    int totalQuantity = (int)Math.Round(recepie.ProductRecepieModel.Sum(o => o.Quantity));
                    await PrintInvoice(filePath, 800, 600, "invoice");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Yazdırma Aşamasında Hata Alındı : {ex.Message}  ");

            }

        }

        public async Task PrintInvoice(string HtmlPath, int width, int height, string type)
        {
            try
            {
                string pngPath = $"C:\\code\\barcodes\\pictures\\{Guid.NewGuid().ToString()}-receipt.png";

                string htmlContent = File.ReadAllText(HtmlPath);

                Bitmap picture = await HtmlToImage(htmlContent, pngPath, width, height, type);

                await PrintWithoutDialog(picture);
            }
            catch (Exception ex)
            {

                throw new Exception($"Yazdırma Aşamasında Hata Alındı (PrintInvoice): {ex.Message}  ");
            }

            //ClearFolder();

        }
        public async Task<Bitmap> HtmlToImage(string html, string path, int width, int height, string type)
        {
            // PuppeteerSharp ayarları için belirli bir tarayıcı indir
            var browserFetcher = new PuppeteerSharp.BrowserFetcher();
            await browserFetcher.DownloadAsync();

            // Yeni bir tarayıcı örneği oluştur
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions());

            // Yeni bir sayfa oluştur
            var page = await browser.NewPageAsync();

            // HTML içeriğini ayarla
            await page.SetContentAsync(html);

            // 'body' elementini seç ve bounding box (sınırlayıcı kutu) bilgisini al
            var bodyElement = await page.QuerySelectorAsync("body");
            var bodyBox = await bodyElement.BoundingBoxAsync();

            // Sayfanın ekran görüntüsünü al, belirtilen bölgeyi kes
            var screenshotOptions = new ScreenshotOptions
            {
                FullPage = true,

            };
            await page.ScreenshotAsync(path, screenshotOptions);
            await browser.CloseAsync();

            // Tarayıcıyı kapat
            if (type == "invoice")
            {
                Bitmap croppedImage = new Bitmap(width, height);


                using (var originalImage = new Bitmap(path))
                {
                    croppedImage = new Bitmap(originalImage.Width - 500, originalImage.Height);
                    using (var graphics = Graphics.FromImage(croppedImage))
                    {
                        graphics.DrawImage(originalImage, new Rectangle(0, 0, croppedImage.Width, croppedImage.Height), 260, 0, croppedImage.Width, croppedImage.Height, GraphicsUnit.Pixel);
                    }
                    croppedImage.Save($"C:\\code\\{Guid.NewGuid().ToString()}.png", ImageFormat.Png);
                }

                return croppedImage;

            }
            else
            {

                var originalImage = new Bitmap(path);
                return originalImage;
            }


        }

    }
}
