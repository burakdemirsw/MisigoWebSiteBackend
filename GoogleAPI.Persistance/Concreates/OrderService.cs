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
using System.Runtime.CompilerServices;
using GoogleAPI.Domain.Models.NEBIM.Request;
using System.Text.Json.Nodes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Net.Http.Json;
using GoogleAPI.Domain.Entities.Common;
using System.Reflection;
using System.Linq;
using Microsoft.AspNetCore.Http;
using GoogleAPI.Domain.Models.NEBIM;

namespace GoogleAPI.Persistance.Concretes
{
    public class OrderService : IOrderService // IOrderService bir arayüz olabilir, işlevinizi tanımlar
    {

        private GooleAPIDbContext _context;
        private ILogService _ls;

        public OrderService(GooleAPIDbContext context, ILogService ls)
        {
            _context = context;
            _ls = ls;
        }

        public Task<Bitmap> GetOrderDetailsFromQrCode(string data)
        {
            throw new NotImplementedException();
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
        public async Task<string> GenerateBarcode_A(List<BarcodeModel_A> barcodes)
        {
            try
            {
                foreach (var barcode in barcodes)
                {
                    string html = await File.ReadAllTextAsync("C:\\code\\barcode_a.txt");
                    var topBarcodeArea = ConvertImageToBase64(QrCode(barcode.TopBarcodeJSON));
                    var productBarcodeArea = ConvertImageToBase64(QrCode(barcode.ProductBarcodeJSON));
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
        public Task PrintWithoutDialog(Bitmap image)
        {
            try
            {
                return Task.Run(( ) => {
                    System.Drawing.Printing.PrintDocument printDocument = new System.Drawing.Printing.PrintDocument();
                    // Set margins to 0 
                    printDocument.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
                    printDocument.PrintPage += (s, args) => {
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
        public async void ClearFolder( )
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

        public async Task<bool> AutoInvoice(string orderNumber, string procedureName, OrderBillingRequestModel requestModel,HttpContext context)
        {
            string requestUrl = context.Request.Path + context.Request.QueryString;


            List<OrderData>? OrderDataList = new List<OrderData>();
            List<OrderDataModel> orderDataList = new List<OrderDataModel>();
            try
            {

                string query = $"exec {procedureName} '{orderNumber}'";

               
                    OrderDataList = await _context.ztOrderData
                      .FromSqlRaw(query)
                      .ToListAsync();

                    if (OrderDataList.Count == 0)
                    {
                        
                        throw new Exception("OrderDataList Null Geldi");

                    }
                  
                        
                if (OrderDataList.Count > 0)
                    {
                    if (OrderDataList.First().Lines == "")
                    {
                        await _ls.LogInvoiceError(JsonConvert.SerializeObject(OrderDataList.First()), "Faturalaştırma Sırasında Hata Alındı", "Fatura Ürünleri Boş Geldi", requestUrl);
                        throw new Exception("Fatura Ürünleri Boş Geldi");
                    }
                        // JSON verilerini çekmek için foreach döngüsü
                        foreach (var orderData in OrderDataList)
                        {

                            // Lines verisini JSON'dan List<OrderLine> nesnesine çevirme

                            List<OrderLine>? lines = null;
                            if (orderData.Lines != null)
                            {
                                lines = JsonConvert.DeserializeObject<List<OrderLine>>(
                                  orderData.Lines
                                );
                            }
                            //bu kısımda line değeri 50 den büyükse fatura yollancak orderheader ıd alıncak sorna tekrardan geri kalan ürünler faturalaşcak.

                            List<Payment>? payments = null;
                            if (orderData.Payments != null)
                            {
                                payments = JsonConvert.DeserializeObject<List<Payment>>(
                                  orderData.Payments
                                );
                            }

                            List<CustomerTaxInfo>? customerTaxInfo = null;
                            if (orderData.CustomerTaxInfo != null)
                            {
                                customerTaxInfo = JsonConvert.DeserializeObject<
                                  List<CustomerTaxInfo>
                                  >
                                  (orderData.CustomerTaxInfo);
                            }

                            List<BaseAddress>? billingAddress = null;
                            if (orderData.BillingAddress != null)
                            {
                                billingAddress = JsonConvert.DeserializeObject<List<BaseAddress>>(
                                  orderData.BillingAddress
                                );
                            }

                            List<BaseAddress>? shipmentAddress = null;
                            if (orderData.ShipmentAddress != null)
                            {
                                shipmentAddress = JsonConvert.DeserializeObject<List<BaseAddress>>(
                                  orderData.ShipmentAddress
                                );
                            }

                            List<PostalAddress>? postalAddress = null;
                            if (orderData.PostalAddress != null)
                            {
                                postalAddress = JsonConvert.DeserializeObject<List<PostalAddress>>(
                                  orderData.PostalAddress
                                );
                            }

                            List<Domain.Models.NEBIM.Invoice.Invoice>? invoiceData = null;
                            if (orderData.InvoiceData != null)
                            {
                                invoiceData = JsonConvert.DeserializeObject<List<Domain.Models.NEBIM.Invoice.Invoice>>(
                                  orderData.InvoiceData
                                );
                            }

                            List<Domain.Models.NEBIM.Invoice.Product>? products =
                              null;
                            if (orderData.Products != null)
                            {
                                products = JsonConvert.DeserializeObject<
                                  List<Domain.Models.NEBIM.Invoice.Product>
                                  >
                                  (orderData.Products);
                            }

                            // OrderDataModel nesnesini oluşturma ve verileri atama
                            OrderDataModel orderDataModel = new OrderDataModel
                            {
                                InternalDescription = orderData.InternalDescription,
                                EInvoicenumber = orderData.EInvoicenumber,
                                OrderNo = orderData.OrderNo,
                                TaxTypeCode = orderData.TaxTypeCode,
                                OrderNumber = orderData.OrderNumber,
                                OrderHeaderID = orderData.OrderHeaderID,
                                DocCurrencyCode = orderData.DocCurrencyCode,
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
                            object jsonModel = new();
                            if (requestModel.InvoiceModel == 1) //ALIŞ FATURASI  oluşturma
                            {
                                if (requestModel.InvoiceType == true)
                                {
                                    List<OrderLineBP> lineList = new List<OrderLineBP>();
                                    foreach (OrderLine line in orderData.Lines)
                                    {
                                        OrderLineBP orderLineBP = new OrderLineBP
                                        {
                                            UsedBarcode = line.UsedBarcode,
                                            BatchCode = line.BatchCode,
                                            ITAttributes = line.ITAttributes,
                                            LDisRate1 = line.LDisRate1,
                                            VatRate = line.VatRate,
                                            Price = line.Price,
                                            Amount = line.Amount,
                                            Qty1 = line.Qty1
                                        };

                                        lineList.Add(orderLineBP);
                                    }
                                    var jsonModel1 = new
                                    {
                                        ModelType = 19,
                                        VendorCode = orderData.CurrAccCode,
                                        InvoiceNumber = orderData.OrderNumber,
                                        EInvoicenumber = orderData.EInvoicenumber,
                                        PosTerminalID = 1,
                                        IsReturn = true, //IADE
                                        TaxTypeCode = orderData.TaxTypeCode,
                                        InvoiceDate = DateTime.Now.ToString("yyyy-MM-dd"),
                                        Description = orderData.InternalDescription, //siparisNo
                                        InternalDescription = orderData.InternalDescription, //siparisNo
                                        IsOrderBase = false,
                                        IsCreditSale = true,
                                        ShipmentMethodCode = orderData.ShipmentMethodCode,
                                        CompanyCode = orderData.CompanyCode,
                                        EMailAddress = orderData.EMailAddress,
                                        BillingPostalAddressID = orderData.BillingPostalAddressID,
                                        ShippingPostalAddressID = orderData.ShippingPostalAddressID,
                                        OfficeCode = orderData.OfficeCode,
                                        WareHouseCode = orderData.WareHouseCode,
                                        Lines = lineList,
                                        IsCompleted = true
                                    };
                                    jsonModel = jsonModel1;
                                } //ALIŞ İADE FATURASI 
                                else
                                {
                                    List<OrderLineBP> lineList = new List<OrderLineBP>();
                                    foreach (OrderLine line in orderData.Lines)
                                    {
                                        OrderLineBP orderLineBP = new OrderLineBP
                                        {
                                            UsedBarcode = line.UsedBarcode,
                                            BatchCode = line.BatchCode,
                                            ITAttributes = line.ITAttributes,
                                            LDisRate1 = line.LDisRate1,
                                            VatRate = line.VatRate,
                                            Price = line.Price,
                                            Amount = line.Amount,
                                            Qty1 = line.Qty1
                                        };

                                        lineList.Add(orderLineBP);
                                    }
                                    var jsonModel2 = new
                                    {
                                        ModelType = 19,
                                        VendorCode = orderData.CurrAccCode,
                                        InvoiceNumber = orderData.OrderNumber,
                                        EInvoicenumber = orderData.EInvoicenumber,
                                        PosTerminalID = 1,
                                        TaxTypeCode = orderData.TaxTypeCode,
                                        InvoiceDate = DateTime.Now.ToString("yyyy-MM-dd"),
                                        Description = orderData.InternalDescription, //siparisNo
                                        InternalDescription = orderData.InternalDescription, //siparisNo
                                        IsOrderBase = false,
                                        IsCreditSale = true,
                                        ShipmentMethodCode = orderData.ShipmentMethodCode,

                                        CompanyCode = orderData.CompanyCode,

                                        EMailAddress = orderData.EMailAddress,
                                        BillingPostalAddressID = orderData.BillingPostalAddressID,
                                        ShippingPostalAddressID = orderData.ShippingPostalAddressID,
                                        OfficeCode = orderData.OfficeCode,
                                        WareHouseCode = orderData.WareHouseCode,

                                        Lines = lineList,

                                        IsCompleted = true
                                    };
                                    jsonModel = jsonModel2;
                                } //ALIŞ FATURASI 

                            }
                            else if (requestModel.InvoiceModel == 2)
                            {
                                if (requestModel.InvoiceType == false)
                                {
                                    List<OrderLineBP> lineList = new List<OrderLineBP>();
                                    foreach (OrderLine line in orderData.Lines)
                                    {
                                        OrderLineBP orderLineBP = new OrderLineBP
                                        {
                                            UsedBarcode = line.UsedBarcode,
                                            BatchCode = line.BatchCode,
                                            ITAttributes = line.ITAttributes,
                                            LDisRate1 = line.LDisRate1,
                                            VatRate = line.VatRate,
                                            Price = line.Price,
                                            Amount = line.Amount,
                                            Qty1 = line.Qty1
                                        };

                                        lineList.Add(orderLineBP);
                                    }
                                    var jsonModel3 = new
                                    {
                                        ModelType = 19,
                                        VendorCode = orderData.CurrAccCode,
                                        EInvoicenumber = orderData.EInvoicenumber,

                                        PosTerminalID = 1,
                                        TaxTypeCode = orderData.TaxTypeCode,
                                        InvoiceDate = DateTime.Now.ToString("yyyy-MM-dd"),
                                        Description = orderData.InternalDescription, //siparisNo
                                        InternalDescription = orderData.InternalDescription, //siparisNo
                                        IsOrderBase = false,
                                        IsCreditSale = true,
                                        ShipmentMethodCode = orderData.ShipmentMethodCode,
                                        CompanyCode = orderData.CompanyCode,
                                        EMailAddress = orderData.EMailAddress,
                                        BillingPostalAddressID = orderData.BillingPostalAddressID,
                                        ShippingPostalAddressID = orderData.ShippingPostalAddressID,
                                        OfficeCode = orderData.OfficeCode,
                                        WareHouseCode = orderData.WareHouseCode,

                                        Lines = lineList,

                                        IsCompleted = true
                                    };
                                    jsonModel = jsonModel3;
                                }
                            } //ALIŞ SİPARİŞ FATURASI 
                            else if (requestModel.InvoiceModel == 3)
                            {
                                orderData.TaxTypeCode = Convert.ToInt32(requestModel.Currency);
                                if (orderData.Lines != null)
                                    if (orderData.Lines[0].SalesPersonCode == null)
                                    {

                                        foreach (var item in orderData.Lines)
                                        {
                                            item.SalesPersonCode = requestModel.SalesPersonCode;
                                            //  item.DocCurrencyCode = orderData.DocCurrencyCode;
                                        }
                                    }
                                if (requestModel.InvoiceType == false)
                                {
                                    var jsonModel6 = new
                                    {
                                        ModelType = 7,
                                        CustomerCode = orderData.CurrAccCode,
                                        InvoiceNumber = orderData.OrderNumber,
                                        PosTerminalID = 1,
                                        TaxTypeCode = orderData.TaxTypeCode,
                                        DocCurrencyCode = orderData.DocCurrencyCode,
                                        InvoiceDate = DateTime.Now.ToString("yyyy-MM-dd"),
                                        Description = orderData.Description, //siparisNo
                                        InternalDescription = orderData.InternalDescription, //siparisNo
                                        IsOrderBase = false,
                                        IsCreditSale = true,
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
                                    jsonModel = jsonModel6;

                                }
                                else
                                {
                                    if (requestModel.InvoiceType == true)
                                    {
                                        if (orderData.Lines[0].SalesPersonCode == null)
                                        {
                                            foreach (var item in orderData.Lines)
                                            {
                                                item.SalesPersonCode = requestModel.SalesPersonCode;
                                                item.DocCurrencyCode = orderData.DocCurrencyCode;
                                            }
                                        }

                                        var jsonModel7 = new
                                        {
                                            ModelType = 7,
                                            CustomerCode = orderData.CurrAccCode,
                                            PosTerminalID = 1,
                                            IsReturn = true,
                                            EInvoiceNumber = orderData.EInvoicenumber,
                                            InvoiceNumber = orderData.OrderNumber,
                                            TaxTypeCode = orderData.TaxTypeCode,
                                            DocCurrencyCode = orderData.DocCurrencyCode,
                                            InvoiceDate = DateTime.Now.ToString("yyyy-MM-dd"),
                                            Description = orderData.Description, //siparisNo
                                            InternalDescription = orderData.InternalDescription, //siparisNo
                                            IsOrderBase = false,
                                            IsCreditSale = true,
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
                                        jsonModel = jsonModel7;

                                    }
                                }
                            } //SATIŞ FATURASI  (oluşturulmamış)
                            else if (requestModel.InvoiceModel == 4) //SATIŞ SİPARİŞ FATURASI
                            {
                                if (orderNumber.Contains("WS"))
                                {
                                    if (requestModel.InvoiceType == true)
                                    {
                                        if (orderNumber.Contains("WS") && requestModel.InvoiceType == false)
                                        {
                                            var jsonModel6 = new
                                            {
                                                ModelType = 7,
                                                CustomerCode = orderData.CurrAccCode,
                                                InvoiceNumber = orderData.OrderNumber,
                                                PosTerminalID = 1,
                                                IsUdtReturn = true,
                                                TaxTypeCode = orderData.TaxTypeCode,
                                                InvoiceDate = DateTime.Now.ToString("yyyy-MM-dd"),
                                                Description = orderData.Description, //siparisNo
                                                InternalDescription = orderData.InternalDescription, //siparisNo
                                                IsOrderBase = false,
                                                IsCreditSale = true,
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
                                            jsonModel = jsonModel6;

                                        }
                                    } //IADE
                                    else
                                    {
                                        if (orderNumber.Contains("WS") && requestModel.InvoiceType == false)
                                        {
                                            var jsonModel7 = new
                                            {
                                                ModelType = 7,
                                                CustomerCode = orderData.CurrAccCode,
                                                InvoiceNumber = orderData.OrderNumber,
                                                PosTerminalID = 1,
                                                TaxTypeCode = orderData.TaxTypeCode,
                                                InvoiceDate = DateTime.Now.ToString("yyyy-MM-dd"),
                                                Description = orderData.Description, //siparisNo
                                                InternalDescription = orderData.InternalDescription, //siparisNo
                                                IsOrderBase = false,
                                                IsCreditSale = true,
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
                                            jsonModel = jsonModel7;

                                        }
                                    }
                                }
                                else if (orderNumber.Contains("R"))
                                {
                                    if (requestModel.InvoiceType == true)
                                    {
                                        var jsonModel7 = new
                                        {
                                            ModelType = 8,
                                            CustomerCode = orderData.CurrAccCode,
                                            InvoiceNumber = orderData.OrderNumber,
                                            PosTerminalID = 1,
                                            IsReturn = true,
                                            InvoiceDate = DateTime.Now.ToString("yyyy-MM-dd"),
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
                                                PaymentTypeDescription = orderData.Payments?
                                                .First()
                                                .CreditCardTypeCode,
                                                PaymentTypeCode = orderData.Payments.First().PaymentType,
                                                PaymentAgent = "",
                                                PaymentDate = DateTime.Now.ToString("yyyy-MM-dd"),
                                                SendDate = DateTime.Now.ToString("yyyy-MM-dd")
                                            },
                                            IsCompleted = true
                                        };
                                        jsonModel = jsonModel7;
                                    }
                                    else
                                    {
                                        var jsonModel8 = new
                                        {
                                            ModelType = 8,
                                            CustomerCode = orderData.CurrAccCode,
                                            InvoiceNumber = orderData.OrderNumber,
                                            PosTerminalID = 1,
                                            InvoiceDate = DateTime.Now.ToString("yyyy-MM-dd"),
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
                                                PaymentTypeDescription = orderData.Payments?
                                                .First()
                                                .CreditCardTypeCode,
                                                PaymentTypeCode = orderData.Payments?.First().PaymentType,
                                                PaymentAgent = "",
                                                PaymentDate = DateTime.Now.ToString("yyyy-MM-dd"),
                                                SendDate = DateTime.Now.ToString("yyyy-MM-dd")
                                            },
                                            IsCompleted = true
                                        };
                                        jsonModel = jsonModel8;
                                    }

                                }

                            }

                            var json = JsonConvert.SerializeObject(jsonModel);

                            var response = await PostNebimAsync(json,context);

                            
                            JObject jsonResponse = JObject.Parse(response);

                            string eInvoiceNumber = jsonResponse["EInvoiceNumber"].ToString();

                            string UnofficialInvoiceString = jsonResponse[
                              "UnofficialInvoiceString"
                            ].ToString();


                        
                            EInvoiceModel model = new EInvoiceModel
                            {
                                OrderNo = orderData.OrderNo,
                                EInvoiceNumber = eInvoiceNumber,
                                OrderNumber = orderData.OrderNumber,
                                InvoiceDatetime = DateTime.Now,
                                UnofficialInvoiceString = UnofficialInvoiceString,
                            };
                      
                            var addedEntity = _context.Entry(model);
                            addedEntity.State = Microsoft
                              .EntityFrameworkCore
                              .EntityState
                              .Added;

                            _context.SaveChanges();
                        
                            //if (orderData.OrderNumber.Contains("WS"))
                            //{
                            //    // var affectedRows = _context.Database.ExecuteSqlRaw($"exec usp_MSDeleteOrder '{orderData.OrderNumber}'").ToString();
                            //}

                            //eğer istek başarılı olursa ; 

                            string InvoiceNumber = jsonResponse["InvoiceNumber"].ToString();

                  
                       
                            foreach (var invoice in orderDataList)
                            {
                                foreach (var line in invoice.Lines)
                                {
                                    ZTMSRAFInvoiceDetailBP invoiceDetail = new();
                                    invoiceDetail.UsedBarcode = line.UsedBarcode;
                                    invoiceDetail.BatchCode = line.BatchCode;
                                    invoiceDetail.LDisRate1 = Convert.ToInt32(line.LDisRate1);
                                    invoiceDetail.VatRate = line.VatRate.ToString();
                                    invoiceDetail.Price = line.Price;
                                    invoiceDetail.Amount = line.Amount;
                                    invoiceDetail.Qty1 = line.Qty1;
                                    invoiceDetail.ITAttributes = line.ITAttributes.First().AttributeCode;
                                    invoiceDetail.OrderNumber = InvoiceNumber;
                                    invoiceDetail.OrdernumberRAF = orderNumber;
                                    invoiceDetail.ItemCode = line.ItemCode;
                                    invoiceDetail.InvoiceDate = DateTime.Now;
                                    _context.ZTMSRAFInvoiceDetailBP.Add(invoiceDetail);
                                    await _context.SaveChangesAsync();

                                }
                           }
                        
                        //tam bu alanda fatura içeriğini tekrardan çek eğer lines alanı boşsa işlemi bitir
                      
                            OrderDataList = await _context.ztOrderData
                              .FromSqlRaw(query)
                              .ToListAsync();

                            if (OrderDataList.First().Lines != "")
                            {
                            //throw new Exception($"Fatura Cevabı Boş Olmadığı İçin Yeniden İstek Atıldı");
                            await _ls.LogInvoiceWarn($"Faturalaştırma Aşamasında Hata Alındı", $"Fatura Cevabı Boş Olmadığı İçin Yeniden İstek Atıldı", requestUrl);
                            bool invoiceResponse = await AutoInvoice(orderNumber, procedureName, requestModel,context);
                                if (invoiceResponse)
                                {

                                    return true;
                                }
                                else
                                {
                                    return false;
                                }

                            }
                        
                        
                        return true;

                        }
                    }
                    else
                    {
                        throw new Exception($"OrderDataList Boş Geldi");

                    }
                    return false;
              
            }
            catch (Exception ex)
            {
                 await _ls.LogInvoiceError("",$"Faturalaştırma Aşamasında Hata Alındı", $"{ex.Message}", requestUrl);
                throw new Exception($"Faturalaştırma Aşamasında Hata Alındı : {ex.Message}  ");

            }
        }
        public async Task<string> PostNebimAsync(string content,HttpContext context)
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

                var response =  await httpClient.PostAsync(
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
                        if (erm.StatusCode==400)
                        {
                            await _ls.LogInvoiceError($"{content}","Faturalaştırma Başarısız", erm.ExceptionMessage,context.Request.Path);

                            throw new Exception(erm.ExceptionMessage);
                        }
                    }
                    await _ls.LogInvoiceSuccess("Faturalaştırma Başarılı", content);

                    return result;
                }
                else
                {
                    
                    return null;
                }
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
                 await _ls.LogOrderWarn($"NEBIM INTEGRATOR CONNECT ERROR", $"{ex.Message}","");
                return null;
            }
        }

        public async Task<List<SalesPersonModel>> GetAllSalesPersonModels( )
        {
            try
            {
                List<SalesPersonModel> list = await _context.SalesPersonModels.FromSqlRaw("Select SalespersonCode,FirstLastName from cdSalesperson where IsBlocked = 0 ").ToListAsync();

                return list;

            }
            catch (Exception ex)
            {
                string methodName  =await GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
                 await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}","");
                throw new Exception(ex.ToString());
            }
        }
        public async Task<string> GetCurrentMethodName (string name)
        {
       
            int start = name.IndexOf('<');
            int end = name.IndexOf('>');
            string result = name.Substring(start + 1, end - start - 1);
            return result;
        }

      
        }

    }

