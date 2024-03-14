using GoogleAPI.Domain.Models.Cargo.Mng.Request;
using GoogleAPI.Domain.Models.Cargo.Mng.Response;
using GoogleAPI.Domain.Models.Cargo.Response;
using GoogleAPI.Persistance.Contexts;
using GoogleAPI.Persistance.Helper;
using GooleAPI.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Drawing;
using System.Text;
using System.Drawing.Imaging;
using ZXing.Common;
using GooleAPI.Application.IRepositories.UserAndCommunication;
using GoogleAPI.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using Nancy.Json;

namespace GoogleAPI.Persistance.Concreates
{
    public class MngCargoService : IMNGCargoService
    {

        GooleAPIDbContext _context;
        IConfiguration _configuriation;
        ICargoBarcodeWriteRepository _cargoBarcodeWriteRepository;
        public MngCargoService(GooleAPIDbContext context, IConfiguration configuriation, ICargoBarcodeWriteRepository cargoBarcodeWriteRepository)
        {

            _context = context;
            _configuriation = configuriation;
            _cargoBarcodeWriteRepository = cargoBarcodeWriteRepository;
        }
        public string GenerateRandomNumber( )
        {
            Random rand = new Random();
            StringBuilder sb = new StringBuilder();

            // İlk rakamın 0 olmaması için ayrı bir işlem yapılır
            sb.Append(rand.Next(1, 10));

            // Kalan 14 hane için döngü
            for (int i = 1; i < 15; i++)
            {
                sb.Append(rand.Next(0, 10));
            }

            string randomNumberString = sb.ToString();

            return randomNumberString;

            // Gerekiyorsa büyük sayılarla çalışmak için bu string'i BigInteger'a çevirebilirsiniz.
            // Örneğin:
            // BigInteger bigInt = BigInteger.Parse(randomNumberString);
            // Console.WriteLine($"BigInteger olarak: {bigInt}");
        }
        public async Task<CreatePackage_MNG_RR> CreateCargo(CreatePackage_MNG_Request request)
        {

            string url = "https://api.mngkargo.com.tr/mngapi/api/standardcmdapi/createOrder";
            string clientId = _configuriation["Cargo:ApiKey"];
            string clientSecret = _configuriation["Cargo:ApiSecretKey"];


            CreateTokenResponse_MNG token = await CreateToken();
            if (token == null)
            {
                throw new Exception("Token Null");
            }

            string json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            Console.WriteLine(json);
            using (HttpClient client = new HttpClient())
            {

                client.DefaultRequestHeaders.Add("X-IBM-Client-Id", clientId.Trim());
                client.DefaultRequestHeaders.Add("X-IBM-Client-Secret", clientSecret.ToString());
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Jwt.ToString());

                Console.WriteLine(token.Jwt);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);
                string responseContent = await response.Content.ReadAsStringAsync();


                if (response.IsSuccessStatusCode)
                {

                    CreatePackage_MNG_Response[]? tokenResponse = JsonConvert.DeserializeObject<CreatePackage_MNG_Response[]>(responseContent);
                    if (tokenResponse != null && tokenResponse.Length > 0)
                    {
                        CreatePackage_MNG_RR _response = new CreatePackage_MNG_RR();
                        _response.Response = tokenResponse[0];
                        _response.Request = request;
                        //CreateBarcode_MNG_Request barcode_request = new CreateBarcode_MNG_Request();
                        //barcode_request.ReferenceId = request.Order.ReferenceId;
                        //barcode_request.BillOfLandingId = request.Order.BillOfLandingId;
                        //barcode_request.IsCOD = request.Order.IsCOD;
                        //barcode_request.CodAmount = request.Order.CodAmount;
                        //barcode_request.PackagingType = request.Order.PackagingType;
                      

                        //await CreateBarcode(request)
                        return _response;

                    }
                    else
                    {
                        throw new Exception($"Gelen Yanıt Yok:{responseContent}");
                    }

                }
                else
                {
                    throw new Exception($"Yanıt Başarısız:{responseContent}");
                }
            }



            throw new NotImplementedException();
        }
        public async Task<CreateBarcode_MNG_Response> CreateBarcode(CreateBarcode_MNG_Request Request)
        {
            //bu aşamada kontrol yapılcak bu siparişe ait bu referans Id ile daha önce dbde kayıt varsa direkt yazdır yoksa istek at yazdır
            CargoBarcode? cargoBarcode = _context.msg_CargoBarcodes.FirstOrDefault(cb=>cb.OrderNo == Request.Response.Request.Order.Barcode && !String.IsNullOrEmpty(cb.BarcodeZplCode) );
            if (cargoBarcode != null)
            {
                
                await ConvertAndPrintBarcode(cargoBarcode.BarcodeZplCode);
            }

            string url = "https://api.mngkargo.com.tr/mngapi/api/barcodecmdapi/createbarcode";
            string clientId = _configuriation["Cargo:ApiKey"];
            string clientSecret = _configuriation["Cargo:ApiSecretKey"];


            CreateTokenResponse_MNG token = await CreateToken();
            CreateBarcode_MNG_Request request = new CreateBarcode_MNG_Request();
            request.ReferenceId = Request.ReferenceId;
            request.BillOfLandingId = Request.BillOfLandingId;
            request.IsCOD = Request.IsCOD;
            request.CodAmount = Request.CodAmount;
            request.PackagingType = Request.PackagingType;
            request.OrderPieceList = Request.OrderPieceList;
            string json = JsonConvert.SerializeObject(request);

            using (HttpClient client = new HttpClient())
            {

                client.DefaultRequestHeaders.Add("X-IBM-Client-Id", clientId.Trim());
                client.DefaultRequestHeaders.Add("X-IBM-Client-Secret", clientSecret.ToString());
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Jwt.ToString());


                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);
                string responseContent = await response.Content.ReadAsStringAsync();


                if (response.IsSuccessStatusCode)
                {

                    CreateBarcode_MNG_Response[]? tokenResponse = JsonConvert.DeserializeObject<CreateBarcode_MNG_Response[]>(responseContent);
                    if (tokenResponse != null && tokenResponse.Length > 0)
                    {
                        var printResponse = await PrintBarcode(tokenResponse[0].Barcodes[0].ToString());
                        if (printResponse)
                        {
                            try
                            {
                                CargoBarcode _cargoBarcode = new CargoBarcode();
                                _cargoBarcode.Id = 0;
                                _cargoBarcode.Request = new JavaScriptSerializer().Serialize(Request.Response.Request);
                                _cargoBarcode.Response = new JavaScriptSerializer().Serialize(Request.Response.Response);
                                _cargoBarcode.OrderNo = Request.BillOfLandingId;
                                _cargoBarcode.BarcodeZplCode = tokenResponse[0].Barcodes[0].Value;
                                _cargoBarcode.ReferenceId = request.ReferenceId;
                                _cargoBarcode.CreatedDate = DateTime.Now;


                                var dbResponse = await _cargoBarcodeWriteRepository.AddAsync(_cargoBarcode);
                                if (dbResponse)
                                {
                                    await ConvertAndPrintBarcode(tokenResponse[0].Barcodes[0].Value);
                                    return tokenResponse[0];
                                }
                                else
                                {
                                    return null;
                                }

                            }
                            catch (Exception ex)
                            {

                                throw new Exception(ex.Message + ex.InnerException);
                            }
                           
                          
                        }
                        else

                        {
                            throw new Exception($"Gelen Yazıcı Yanıt Yok:{responseContent}");
                        }


                    }
                    else
                    {
                        throw new Exception($"Gelen Yanıt Yok:{responseContent}");
                    }

                }
                else
                {
                    throw new Exception($"Yanıt Başarısız:{responseContent}");
                }
            }



            throw new NotImplementedException();
        }
        //public async Task ConvertPNG(string zpl)
        //{

        //    var zplAsBytes = System.Text.Encoding.UTF8.GetBytes(zpl);
        //    var requestContent = new ByteArrayContent(zplAsBytes);

        //    requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

        //    using (var httpClient = new HttpClient())
        //    {
        //        var response = await httpClient.PostAsync("http://api.labelary.com/v1/printers/8dpmm/labels/4x6/0/", requestContent);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            using (var ms = await response.Content.ReadAsStreamAsync())
        //            {
        //                using (var fs = new FileStream(@"C:\code\label.png", FileMode.Create, FileAccess.Write))
        //                {
        //                    ms.CopyTo(fs);
        //                    fs.Flush();
        //                }
        //            }

        //            Console.WriteLine("Etiket PNG olarak kaydedildi.");
        //        }
        //        else
        //        {
        //            Console.WriteLine("Bir hata oluştu: " + response.StatusCode);
        //        }
        //    }
        //}


        public async Task<CreateTokenResponse_MNG> CreateToken( )
        {
            string url = "https://api.mngkargo.com.tr/mngapi/api/token";
            string clientId = _configuriation["Cargo:ApiKey"];
            string clientSecret = _configuriation["Cargo:ApiSecretKey"];

            var request = new CreateToken_MNG_Request
            {
                CustomerNumber = _configuriation["Cargo:UserName"],
                Password = _configuriation["Cargo:Password"],
                IdentityType = 1
            };
            string json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });


            using (HttpClient client = new HttpClient())
            {

                client.DefaultRequestHeaders.Add("X-IBM-Client-Id", clientId.Trim());
                client.DefaultRequestHeaders.Add("X-IBM-Client-Secret", clientSecret.ToString());

                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);
                //Console.WriteLine(clientId);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    CreateTokenResponse_MNG tokenResponse = JsonConvert.DeserializeObject<CreateTokenResponse_MNG>(responseContent);
                    return tokenResponse;


                }
                else
                {
                    var _reponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {_reponse}");
                    return null;
                }
            }
        }

        async Task<bool> PrintBarcode(string zplCode)
        {
            return ZplPrinterService.SendStringToPrinter(_configuriation["Printers:ZplPrinterName"], zplCode);
        }

        public async Task ConvertAndPrintBarcode(string zpl)
        {
         

            var zplAsBytes = System.Text.Encoding.UTF8.GetBytes(zpl);
            var requestContent = new ByteArrayContent(zplAsBytes);

            requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync("http://api.labelary.com/v1/printers/8dpmm/labels/4x4/0/", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    using (var ms = await response.Content.ReadAsStreamAsync())
                    {
                        var guid = Guid.NewGuid().ToString();
                        var path = @"C:\code\" + guid + ".png";
                        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                        {
                            ms.CopyTo(fs);
                            fs.Flush();
                        }

                        Bitmap myBitmap = new Bitmap(path);
                        await PrintWithoutDialog(myBitmap);
                    }

                    Console.WriteLine("Etiket PNG olarak kaydedildi.");


                }
                else
                {
                    Console.WriteLine("Bir hata oluştu: " + response.StatusCode);
                }
            }
        }
        public  Bitmap Base64ToBitmap( string base64string)
        {
            // Sample Base64 string representing an image
            string base64String = "YourBase64StringHere";

            // Convert Base64 string to byte array
            byte[] imageBytes = Convert.FromBase64String(base64String);

            // Create a memory stream from the byte array
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                // Create Bitmap from memory stream
                Bitmap bitmap = new Bitmap(ms);

                // Now you have your bitmap, you can use it as needed
                // For example, you can save it to a file
                return bitmap;
            }
        }

        private Task PrintWithoutDialog(Bitmap image)
        {
            return Task.Run(( ) =>
            {
                System.Drawing.Printing.PrintDocument printDocument = new System.Drawing.Printing.PrintDocument();


                // Yazdırma yönünü yatay (landscape) olarak ayarla


                printDocument.DefaultPageSettings.Landscape = true;
                // DPI değerlerini al
                int dpiX = (int)(image.HorizontalResolution);
                int dpiY = (int)(image.VerticalResolution);

                // 10cm x 10cm boyutunu piksele çevir
                float cmToInch = 0.393701f; // 1 cm = 0.393701 inç
                int width = (int)(10 * cmToInch * dpiX);
                int height = (int)(10 * cmToInch * dpiY);

                printDocument.PrintPage += (s, args) =>
                {
                    // Belirlenen boyutta ve konumda resmi çiz
                    args.Graphics.DrawImage(image, 0, 0, width, height);
                };

                // Yazıcı adını belirt (örneğinizde "KARGOBARKOD" kullanılmış)
                printDocument.PrinterSettings.PrinterName = "KARGOBARKOD";

                // Diyalog göstermeden yazdırmak için Standart Yazdırma Kontrolörü kullan
                printDocument.PrintController = new StandardPrintController();

                // Yazdırma işlemini başlat
                printDocument.Print();
            });
        }


        public async Task<bool> PrintBarcode2(string zplCode)
        {
            if (zplCode == null)
            {
                zplCode = "^XA\r\n^MMT\r\n^CI28\r\n^PW831\r\n^LL0959\r\n^LS0\r\n~SD20\r\n^PRB\r\n\r\n\r\n^FT658,45^A0N,23,28^FH^FDMNG^FS\r\n^FT717,45^A0N,23,31^FH^FDKargo^FS\r\n\r\n; Var_C4_b1_C5_9f / _C3_87_C4_b1k_C4_b1_C5_9f _C5_9eube Ad_C4_b1\r\n^FT723,50^A0R,23,24^FH^FDYEN_C4_b0KAPI^FS\r\n^FT748,53^A0R,62,62^FH^FD_C3_87AMLICA^FS\r\n\r\n\r\n; Tarih, Kargo Cinsi; KG/DESI, _C4_b0_C3_a7erik\r\n^FT701,50^A0R,20,19^FH^FD12.03.2024/^FS\r\n^FT701,147^A0R,20,19^FH^FD M_C4_b0^FS\r\n^FT680,50^A0R,20,19^FH^FDKg:1 Ds:1 Kg/Ds:1^FS\r\n^FT657,50^A0R,20,19^FH^FDToplam:1/Par_C3_a7a:0001/0001^FS\r\n\r\n; G_C3_b6nderi T_C3_bCr_C3_bC(_C3_96ncelikli vb.) ,G_C3_b6nderi _C3_9Ccreti, _C3_9Cr_C3_bCn Bedeli Kutusu\r\n\r\n\r\n\r\n^FT725,465^A0R,23,24^FH^FD^FS\r\n^FT695,410^A0R,25,23^FH^FD^FS\r\n^FT665,410^A0R,25,23^FH^FD^FS\r\n\r\n\r\n; Gon/Al_C4_b1c_C4_b1 Ba_C5_9fl_C4_b1k Kutusu\r\n^FO483,20^GB171,27,2^FS\r\n\r\n^FT608,43^A0N,20,19^FH^FDG_C3_b6n^FS\r\n^FT509,42^A0N,20,31^FH^FDAl_C4_b1c_C4_b1^FS\r\n\r\n; G_C3_b6n / Al_C4_b1c_C4_b1 Adres Kutusu\r\n^FO483,46^GB171,535,2^FS\r\n^FO597,23^GB0,556,2^FS\r\n\r\n\r\n\r\n\r\n\r\n; G_C3_b6nderen Ad_C4_b1, Telefon\r\n^FT637,48^A0R,14,14^FH^FDMEL_C4_b0H G_C3_9CNAL DENTAL MED_C4_b0KAL SAN T_C4_b0C LTD _C5_9eT_C4_b0 BO_C4_9eAZ_C4_b0_C3_87_C4_b0 D_C4_b0_C5_9e DEPO^FS\r\n^FT619,48^A0R,14,14^FH^FDSU^FS\r\n^FT602,48^A0R,14,14^FH^FD^FS\r\n^FT602,450^A0R,17,19^FH^FDTEL:2125233518^FS\r\n\r\n; Al_C4_b1c_C4_b1 Ad,Adres,Telefon\r\n^FT574,450^A0R,20,19^FH^FDTEL:5393465584^FS\r\n^FT574,50^A0R,20,19^FH^FDBURAK DEM_C4_b0R^FS\r\n^FT554,49^A0R,20,19^FH^FD^FS\r\n^FT533,49^A0R,17,16^FH^FDK_C3_9C_C3_87_C3_9CK _C3_87AMLICA MAHALLESI M_C3_9CFT_C3_9C KUYUSU SOKAK _C5_9eEKERKAYA VILLAL^FS\r\n^FT511,49^A0R,17,16^FH^FDARI NO: 11 DAIRE 2   [_C3_9CSK_C3_9CDAR/_C4_b0STANBUL]^FS\r\n^FT489,49^A0R,17,16^FH^FD^FS\r\n\r\n\r\n\r\n; DK Kodu Kutusu(Sa_C4_9f en _C3_bCst kutu)\r\n^FO701,589^GB96,201,2^FS\r\n\r\n; DK Kodu / Mobil _C5_9eube\r\n^FT744,595^A0R,62,62^FH^FDDK-2^FS\r\n^FT725,595^A0R,17,16^FH^FD^FS\r\n^FT708,595^A0R,17,16^FH^FD^FS\r\n; Hat Ad_C4_b1\r\n^FT424,22^A0R,62,62^FH^FDMARMARA H.^FS\r\n\r\n^FO600,589^GB103,201,2^FS\r\n; Atf Seri Numara _C3_96deme _C5_9eekli G_C3_b6nderi Numaras_C4_b1 Kutusu\r\n; G_C3_b6nderi Numaras_C4_b1/ G_C3_b6nderi Tipi\r\n^FT660,602^A0R,42,40^FH^FDYS 733411^FS\r\n^FT634,657^A0R,25,24^FH^FDP_C3_96 AT^FS\r\n^FT606,607^A0R,25,24^FH^FD660023296442^FS\r\n^FT627,599^A0R,14,14^FH^FDGN:^FS\r\n\r\n\r\n\r\n; Line Barkod\r\n^BY3,3,259^FT147,30^BCR,,N,N\r\n;^FD>:C@53@IOYYSWMHDAAA6J^FS\r\n\r\n; QR Barkod \r\n^BY126,126^FT567,617^BXI,7,200,0,0,1,~\r\n^FH^FDC@53@IOYYSWMHDAAA6J^FS\r\n\r\n; En Alt A_C3_a7_C4_b1klama Kutusu\r\n^BY2,3,70^FT25,30^BCR,,N,N\r\n^FD>:^FS\r\n\r\n\r\n^FT110,568^A0R,17,17^FH^FDREF:12345^FS\r\n^FT110,259^A0R,17,17^FH^FDIRS NO:BURAKDEM_C4_b0R^FS\r\n^FT110,13^A0R,14,14^FH^FDsetno:68 ^FS\r\n^PQ1,0,1,Y^XZ\r\n";
            }
            return ZplPrinterService.SendStringToPrinter(_configuriation["Printers:ZplPrinterName"], zplCode);
        }


        public Task<bool> DeleteCargo( )
        {
            throw new NotImplementedException();
        }


    }
}
