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
using GooleAPI.Application.Abstractions.IServices.IMail;
using GoogleAPI.Persistance.Concreates.Services.Mail;

namespace GoogleAPI.Persistance.Concreates
{
    public class MngCargoService : IMNGCargoService
    {

        GooleAPIDbContext _context;
        IConfiguration _configuriation;
        ICargoBarcodeWriteRepository _cargoBarcodeWriteRepository;
        IMailService _mailService;
        public MngCargoService(GooleAPIDbContext context, IConfiguration configuriation, ICargoBarcodeWriteRepository cargoBarcodeWriteRepository,IMailService mailService)
        {
            _mailService = mailService;
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
        public async Task<CreatePackage_MNG_RR> CreateCargo(CreatePackage_MNG_RM request)
        {

            string url = "https://api.mngkargo.com.tr/mngapi/api/standardcmdapi/createOrder";
            string clientId = _configuriation["Cargo:ApiKey"];
            string clientSecret = _configuriation["Cargo:ApiSecretKey"];


            CreateTokenResponse_MNG token = await CreateToken();
            if (token == null)
            {
                throw new Exception("Token Null");
            }

            string json = JsonConvert.SerializeObject(request.OrderRequest, new JsonSerializerSettings
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
                        CargoBarcode _cargoBarcode = new CargoBarcode();
                        _cargoBarcode.OrderRequest = new JavaScriptSerializer().Serialize(request.OrderRequest);                      
                        _cargoBarcode.OrderResponse = new JavaScriptSerializer().Serialize(tokenResponse[0]);
                        _cargoBarcode.BarcodeResponse = new JavaScriptSerializer().Serialize(tokenResponse);
                        _cargoBarcode.OrderNo = request.OrderRequest.Order.BillOfLandingId;
                        //_cargoBarcode.BarcodeZplCode = tokenResponse[0].Barcodes[0].Value;
                        //_cargoBarcode.ShipmentId = tokenResponse[0].ShipmentId;
                        _cargoBarcode.ReferenceId = request.OrderRequest.Order.ReferenceId;
                        _cargoBarcode.CreatedDate = DateTime.Now;
                        _cargoBarcode.Customer = request.OrderRequest.Recipient.FullName;
                        _cargoBarcode.BarcodeRequest = new JavaScriptSerializer().Serialize(request.BarcodeRequest);
                        _cargoBarcode.Desi = request.BarcodeRequest.OrderPieceList.First().Desi;
                        _cargoBarcode.Kg = request.BarcodeRequest.OrderPieceList.First().Desi;
                        _cargoBarcode.PackagingType = request.BarcodeRequest.PackagingType;
                        

                        var dbResponse = await _cargoBarcodeWriteRepository.AddAsync(_cargoBarcode);
                        if (dbResponse)
                        {
                            CreatePackage_MNG_RR _response = new CreatePackage_MNG_RR();
                            _response.Response = tokenResponse[0];
                            _response.Request = request.OrderRequest;

                            List<string> emails = new List<string>();
                            emails.Add("demir.burock96@gmail.com");
                            await _mailService.SendCargoInfoMail(emails, request.OrderRequest.Order.Barcode, "www.davye.com", 0); //takip no düşmeyecek


                            return _response;

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
        public async Task<bool> DeleteShippedCargo(DeletePackage_MNG_Request request)
        {

            string url = "https://api.mngkargo.com.tr/mngapi/api/barcodecmdapi/cancelshipment";
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

                HttpResponseMessage response = await client.PutAsync(url, content);
                string responseContent = await response.Content.ReadAsStringAsync();


                if (response.IsSuccessStatusCode)
                {
                    CargoBarcode? cargoBarcode = _context.msg_CargoBarcodes.FirstOrDefault(c => c.ShipmentId == request.ShipmentId
                    && c.ReferenceId == request.ReferenceId);
                    _cargoBarcodeWriteRepository.Remove(cargoBarcode);
                    return true;

                }
                else
                {
                    return false;
                }
            }



          

        }

        public async Task<List<CargoBarcode_VM>> GetShippedCargos( )
        {
            return _context.msg_CargoBarcodes.ToList().Select(x => new CargoBarcode_VM
            {
                ReferenceId = x.ReferenceId,
                OrderNo = x.OrderNo,
                BarcodeZplCode = x.BarcodeZplCode,
                ShipmentId = x.ShipmentId,
                CreatedDate = x.CreatedDate,
                BarcodeRequest = x.BarcodeRequest,  
                Desi = x.Desi,  
                Kg = x.Kg,  
                PackagingType = x.PackagingType,
                Customer = x.Customer,
               
            }).ToList();
        }
        public async Task<CreateBarcode_MNG_Response> CreateBarcode(string referenceId)
        {
            
            //bu aşamada kontrol yapılcak bu siparişe ait bu referans Id ile daha önce dbde kayıt varsa direkt yazdır yoksa istek at yazdır
            CargoBarcode? cargoBarcode = _context.msg_CargoBarcodes.FirstOrDefault(cb=>cb.ReferenceId == referenceId  );
            if (cargoBarcode.BarcodeZplCode != null)
            {
                
                await ConvertAndPrintBarcode(cargoBarcode.BarcodeZplCode);
                return new CreateBarcode_MNG_Response();
            }

            CreateBarcode_MNG_Request? Request = JsonConvert.DeserializeObject<CreateBarcode_MNG_Request>(cargoBarcode.BarcodeRequest);
           

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
                                CargoBarcode? _cargoBarcode = _context.msg_CargoBarcodes.FirstOrDefault(cb => cb.ReferenceId == Request.ReferenceId);

                                _cargoBarcode.BarcodeZplCode = tokenResponse[0].Barcodes[0].Value;
                                _cargoBarcode.ShipmentId = tokenResponse[0].ShipmentId;

                                var dbResponse = await _cargoBarcodeWriteRepository.Update(_cargoBarcode);
                                if (dbResponse)
                                {
                                    List<string> emails = new List<string>();
                                    //emails.Add(Request.Response.Request.Recipient.Email);
                                    emails.Add("demir.burock96@gmail.com");
                                    await _mailService.SendCargoInfoMail(emails, Request.BillOfLandingId, $"https://kargotakip.mngkargo.com.tr/?takipNo={tokenResponse[0].ShipmentId}",1);
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


        public async Task<bool> PrintSingleBarcode(string zplCode)
        {
            
             await ConvertAndPrintBarcode(zplCode);
            return true;
        }


        //
        public async Task<GetPackageStatus_MNG_Response> GetPackageStatus( string ShipmentId)
        {


            string url = $"https://api.mngkargo.com.tr/mngapi/api/standardqueryapi/getshipmentstatus/{ShipmentId}";
            string clientId = _configuriation["Cargo:ApiKey"];
            string clientSecret = _configuriation["Cargo:ApiSecretKey"];


            CreateTokenResponse_MNG token = await CreateToken();
            if (token == null)
            {
                throw new Exception("Token Null");
            }

            //string json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            //{
            //    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            //});

            //Console.WriteLine(json);
            using (HttpClient client = new HttpClient())
            {

                client.DefaultRequestHeaders.Add("X-IBM-Client-Id", clientId.Trim());
                client.DefaultRequestHeaders.Add("X-IBM-Client-Secret", clientSecret.ToString());
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Jwt.ToString());

                Console.WriteLine(token.Jwt);


                HttpResponseMessage response = await client.GetAsync(url);
                string responseContent = await response.Content.ReadAsStringAsync();


                if (response.IsSuccessStatusCode)
                {
                    GetPackageStatus_MNG_Response[]? tokenResponse = JsonConvert.DeserializeObject<GetPackageStatus_MNG_Response[]>(responseContent);
                   
                        return tokenResponse[0];
                   

                }
                else
                {
                    return null;
                }
            }



        }
    }
}
