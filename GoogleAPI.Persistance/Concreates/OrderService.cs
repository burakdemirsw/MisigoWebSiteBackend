using GooleAPI.Application.Abstractions;
using System.Drawing;
using System.Threading.Tasks;
using QRCoder;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using ZXing;
using System.Drawing.Imaging;
using PuppeteerSharp;
using System.Drawing.Printing;
using ZXing.Rendering;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Remotion.Linq.Clauses;
using System.Net;
using System.Text;
using GoogleAPI.Domain.Models.NEBIM.Invoice;

namespace GoogleAPI.Persistance.Concretes
{
    public class OrderService : IOrderService  // IOrderService bir arayüz olabilir, işlevinizi tanımlar
    {

        


        public Task<Bitmap> GetOrderDetailsFromQrCode(string data)
        {
            throw new NotImplementedException();
        }

        public Image QrCode(string id)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(id.ToString(), QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);

            byte[] qrCodeBytes = qrCode.GetGraphic(20);
            Bitmap qrCodeImage = new Bitmap(new MemoryStream(qrCodeBytes));

            return qrCodeImage;
        }

        private string ConvertImageToBase64(Image image)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Png);
                byte[] imageBytes = memoryStream.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
        public async Task<Bitmap> HtmlToImage(string html, string path)
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
            Bitmap croppedImage = new Bitmap(800, 600);

            // Tarayıcıyı kapat
            await browser.CloseAsync();
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

        public Task PrintWithoutDialog(Bitmap image)
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

        public  async void ClearFolder( )
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
        public async  void PrintInvoice(string HtmlPath)
        {
            string pngPath = $"C:\\code\\{Guid.NewGuid().ToString()}-receipt.png";

            string htmlContent = File.ReadAllText(HtmlPath);

            Bitmap picture = await HtmlToImage(htmlContent, pngPath);

            await PrintWithoutDialog(picture);


            ClearFolder();

        }
        public async Task<Boolean> GenerateReceipt(List<string> orderNumbers)
        {

            //gelen fatura numaralarını sp den verilerini çek Receipe Model e eşitle
            List<RecepieModel> recepieModels = new List<RecepieModel>();


            try
                {
                DbContextOptionsBuilder<GooleAPIDbContext> dbContextBuilder = new();
                dbContextBuilder.UseSqlServer("Data Source=192.168.2.36;Initial Catalog=BDD2017;User ID=sa;Password=8969;TrustServerCertificate=True;");
                GooleAPIDbContext _context = new GooleAPIDbContext(dbContextBuilder.Options);
                foreach (var order in orderNumbers)
                    {
                    List<Domain.Models.NEBIM.Order.Invoice> invoiceModel = new List<Domain.Models.NEBIM.Order.Invoice>();

                    invoiceModel = await _context.ztInvoice.FromSqlRaw($"exec GET_MSRAFInvoiceDetail '{order}'").ToListAsync();


                    Image barcodeBitmap = QrCode(order);
                    string barcodeBase64 = ConvertImageToBase64(barcodeBitmap);

                    string EInvoiceNumber = order;
                        ;

                    RecepieModel recepie = new RecepieModel();
                    recepie.EInvoiceNumber = EInvoiceNumber;
                    recepie.OrderNo = order;
                    recepie.OrderDate = invoiceModel.First().OrderDate;
                    recepie.PaymentType = invoiceModel.First().PaymentType;
                    recepie.ProductRecepieModel = new List<ProductRecepieModel>();
                    recepie.TotalVAT = 0;
                    recepie.CustomerName = invoiceModel.First().CustomerDescription;
                    recepie.OrderNoBase64String = barcodeBase64;
                    recepie.TotalValue =0;
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
                    PrintInvoice(filePath);
                }
                return true;
            }
                catch (Exception ex)
                {
                return false;
                }

        }
        public async Task<Boolean> AutoInvoice(string orderNumber,string procedureName)
        {
            List<OrderData> OrderDataList = new List<OrderData>();
            try
            {
                DbContextOptionsBuilder<GooleAPIDbContext> dbContextBuilder = new();
                dbContextBuilder.UseSqlServer("Data Source=192.168.2.36;Initial Catalog=BDD2017;User ID=sa;Password=8969;TrustServerCertificate=True;");
                GooleAPIDbContext _context = new GooleAPIDbContext(dbContextBuilder.Options);

                //iyileştirme yapılcak

                string query = $"exec {procedureName} '{orderNumber}'";
                using (var _db = _context)
                {
                    OrderDataList = _db.ztOrderData
                        .FromSqlRaw(query)
                        .ToList();



                    List<OrderDataModel> orderDataList = new List<OrderDataModel>();

                    if (OrderDataList.Count > 0)
                    {
                        // JSON verilerini çekmek için foreach döngüsü
                        foreach (var orderData in OrderDataList)
                        {


                            // Lines verisini JSON'dan List<OrderLine> nesnesine çevirme

                        

                            List<OrderLine> lines = null;
                            if (orderData.Lines != null)
                            {
                                lines = JsonConvert.DeserializeObject<List<OrderLine>>(
                                    orderData.Lines
                                );
                            }

                            List<Payment> payments = null;
                            if (orderData.Payments != null)
                            {
                                payments = JsonConvert.DeserializeObject<List<Payment>>(
                                    orderData.Payments
                                );
                            }

                            List<CustomerTaxInfo> customerTaxInfo = null;
                            if (orderData.CustomerTaxInfo != null)
                            {
                                customerTaxInfo = JsonConvert.DeserializeObject<
                                    List<CustomerTaxInfo>
                                >(orderData.CustomerTaxInfo);
                            }

                            List<BaseAddress> billingAddress = null;
                            if (orderData.BillingAddress != null)
                            {
                                billingAddress = JsonConvert.DeserializeObject<List<BaseAddress>>(
                                    orderData.BillingAddress
                                );
                            }

                            List<BaseAddress> shipmentAddress = null;
                            if (orderData.ShipmentAddress != null)
                            {
                                shipmentAddress = JsonConvert.DeserializeObject<List<BaseAddress>>(
                                    orderData.ShipmentAddress
                                );
                            }

                            List<PostalAddress> postalAddress = null;
                            if (orderData.PostalAddress != null)
                            {
                                postalAddress = JsonConvert.DeserializeObject<List<PostalAddress>>(
                                    orderData.PostalAddress
                                );
                            }

                            List<Domain.Models.NEBIM.Invoice.Invoice> invoiceData = null;
                            if (orderData.InvoiceData != null)
                            {
                                invoiceData = JsonConvert.DeserializeObject<List<Domain.Models.NEBIM.Invoice.Invoice>>(
                                    orderData.InvoiceData
                                );
                            }

                            List<Domain.Models.NEBIM.Invoice.Product> products =
                                null;
                            if (orderData.Products != null)
                            {
                                products = JsonConvert.DeserializeObject<
                                    List<Domain.Models.NEBIM.Invoice.Product>
                                >(orderData.Products);
                            }

                            // OrderDataModel nesnesini oluşturma ve verileri atama
                            OrderDataModel orderDataModel = new OrderDataModel
                            {
                                InternalDescription = orderData.InternalDescription,
                                OrderNo = orderData.OrderNo,
                                TaxTypeCode = orderData.TaxTypeCode,
                                OrderNumber = orderData.OrderNumber,
                                OrderHeaderID = orderData.OrderHeaderID,
                                Lines = lines,
                                Payments = payments,
                                CurrAccCode = orderData.CurrAccCode,
                                CustomerTaxInfo = customerTaxInfo,
                                EMailAddress = orderData.EMailAddress,
                                Description = orderData.Description,
                                BillingAddress = billingAddress,
                                ShipmentAddress = shipmentAddress,
                                BillingPostalAddressID = orderData.BillingPostalAddressID,
                                ShippingPostalAddressID = orderData.ShippingPostalAddressID,
                                DeliveryCompanyCode = orderData.DeliveryCompanyCode,
                                CompanyCode = orderData.CompanyCode.ToString(), // decimal tipini stringe çevirme
                                IsSalesViaInternet = orderData.IsSalesViaInternet.ToString(), // bool tipini stringe çevirme
                                OfficeCode = orderData.OfficeCode,
                                StoreCode = orderData.StoreCode,
                                WareHouseCode = orderData.WareHouseCode,
                                PostalAddress = postalAddress,
                                ShipmentMethodCode = orderData.ShipmentMethodCode,
                                InvoiceData = invoiceData,
                                Products = products
                            };

                            // OrderDataModel nesnesini listeye ekleme
                            orderDataList.Add(orderDataModel);
                        }

                        foreach (var orderData in orderDataList)
                        {
                            object jsonModel;


                            if (orderNumber.Contains("WS"))
                            {
                                var jsonModel2 = new
                                {
                                    ModelType = 7,
                                    CustomerCode = orderData.CurrAccCode,
                                    PosTerminalID = 1,
                                    TaxTypeCode = orderData.TaxTypeCode,
                                    InvoiceDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Description = orderData.InternalDescription, //siparisNo
                                    InternalDescription = orderData.InternalDescription, //siparisNo
                                    IsOrderBase = false,
                                    ShipmentMethodCode = orderData.ShipmentMethodCode,

                                    CompanyCode = orderData.CompanyCode,

                                    EMailAddress = orderData.EMailAddress,
                                    BillingPostalAddressID = orderData.BillingPostalAddressID,
                                    ShippingPostalAddressID = orderData.ShippingPostalAddressID,
                                    OfficeCode = orderData.OfficeCode,
                                    WareHouseCode = orderData.WareHouseCode,

                                    Lines = orderData.Lines,

                                    IsCompleted = true
                                };
                                jsonModel = jsonModel2;

                            }
                            else
                            {
                                var jsonModel1 = new
                                {
                                    ModelType = 8,
                                    CustomerCode = orderData.CurrAccCode,
                                    PosTerminalID = 1,
                                    InvoiceDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Description = orderData.InternalDescription, //siparisNo
                                    InternalDescription = orderData.InternalDescription, //siparisNo
                                    IsOrderBase = true,
                                    ShipmentMethodCode = orderData.ShipmentMethodCode,
                                    DeliveryCompanyCode = orderData.DeliveryCompanyCode,
                                    CompanyCode = orderData.CompanyCode,
                                    IsSalesViaInternet = true,
                                    SendInvoiceByEMail = true,
                                    EMailAddress = orderData.EMailAddress,
                                    BillingPostalAddressID = orderData.BillingPostalAddressID,
                                    ShippingPostalAddressID = orderData.ShippingPostalAddressID,
                                    OfficeCode = orderData.OfficeCode,
                                    WareHouseCode = orderData.WareHouseCode,
                                    ApplyCampaign = false,
                                    SuppressItemDiscount = false,
                                    Lines = orderData.Lines,
                                    SalesViaInternetInfo = new
                                    {
                                        SalesURL = "www.davye.com",
                                        PaymentTypeDescription = orderData.Payments
                                      .First()
                                      .CreditCardTypeCode,
                                        PaymentTypeCode = orderData.Payments.First().PaymentType,
                                        PaymentAgent = "",
                                        PaymentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        SendDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                    },
                                    IsCompleted = true
                                };
                                jsonModel = jsonModel1;
                            }
                           

                           

                            var json = JsonConvert.SerializeObject(jsonModel);

                            string sessionID = await ConnectIntegrator();

                            using (var httpClient = new HttpClient())
                            {
                                var content = new StringContent(
                                    json,
                                    Encoding.UTF8,
                                    "application/json"
                                );
                                var response = await httpClient.PostAsync(
                                    $"http://192.168.2.36:7676/(S({sessionID}))/IntegratorService/post?",
                                    content
                                );
                                var result = await response.Content.ReadAsStringAsync();
                                JObject jsonResponse = JObject.Parse(result);
                                string eInvoiceNumber = jsonResponse["EInvoiceNumber"].ToString();
                                string UnofficialInvoiceString = jsonResponse[
                                    "UnofficialInvoiceString"
                                ].ToString();
                                OrderErrorModel errorModel =
                                    JsonConvert.DeserializeObject<OrderErrorModel>(result);
                                if (
                                    response.StatusCode == HttpStatusCode.BadRequest
                                    || errorModel.StatusCode == 400
                                )
                                {
                                    return false;
                                }
                                if (response.IsSuccessStatusCode)
                                {
                                    string successMessage =
                                        $"{orderData.OrderNumber}  {eInvoiceNumber}  {DateTime.Now.ToString()}: Success";

                                    // Başarılı mesajı successInvoices.txt dosyasına yaz

                                   

                                    using (var context = _context)
                                    {
                                        EInvoiceModel model = new EInvoiceModel
                                        {
                                            OrderNo = orderData.OrderNo,
                                            EInvoiceNumber = eInvoiceNumber,
                                            OrderNumber = orderData.OrderNumber,
                                            InvoiceDatetime = DateTime.Now,
                                            UnofficialInvoiceString = UnofficialInvoiceString,
                                        };

                                        var addedEntity = context.Entry(model);
                                        addedEntity.State = Microsoft
                                            .EntityFrameworkCore
                                            .EntityState
                                            .Added;

                                 

                                        try
                                        {
                                            context.SaveChanges();
                                            if (orderData.OrderNumber.Contains("WS"))
                                            {
                                                var affectedRows = _context.Database.ExecuteSqlRaw($"exec usp_MSDeleteOrder '{orderData.OrderNumber}'").ToString();
                                            }
                                            //exec sp_deleteInvoiceTrans çalıştırılcak
                                            return true;
                                        }
                                        catch (Exception ex)
                                        {
                                            return false;
                                        }


                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
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
                VM_HttpConnectionRequest session =
                    JsonConvert.DeserializeObject<VM_HttpConnectionRequest>(responseBody);

                string sessionId = session.SessionId;
                // Console.WriteLine(responseBody);
                return sessionId;
            }
            catch (HttpRequestException ex)
            {

                return null;
            }
        }

    }






}
