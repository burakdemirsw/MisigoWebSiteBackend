using GoogleAPI.Domain.Models.Payment;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Nancy;
using Nancy.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Persistance.Concreates
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private GooleAPIDbContext _c;
        public PaymentService(IConfiguration configuration, GooleAPIDbContext context)
        {
            _configuration = configuration;
            _c= context;
        }

        public async Task<Payment_CR> PayTRPayment(Payment_CM model)
        {
            try
            {
                var CD = Guid.NewGuid().ToString();

                decimal formattedPrice = Convert.ToDecimal($"{model.TotalValue:F2}");
                string merchant_id = _configuration["Payment:PayTR:MerchantId"];
                string merchant_key = _configuration["Payment:PayTR:ApiKey"];
                string merchant_salt = _configuration["Payment:PayTR:ApiSecretKey"];
                string emailstr = model.User.Mail;
                int payment_amountstr = Convert.ToInt32(formattedPrice * 100);
                string merchant_oid = Guid.NewGuid().ToString().Replace("-", "");
                string user_namestr = model.User.Mail;
                string user_phonestr = model.User.Phone;
                string user_addressstr = model.Address.Address;
                string merchant_ok_url = _configuration["Payment:PayTR:OkUrl"];
                string merchant_fail_url = _configuration["Payment:PayTR:FailUrl"];
                string user_ip = "176.227.56.29";
                List<Product_PayTR> user_basket = new List<Product_PayTR> { };
                Console.WriteLine(merchant_oid);

                // Assume model.BasketItems is an IEnumerable of Products.
                foreach (var product in model.BasketItems)
                {
                    // Create a new product object.
                    var newProduct = new Product_PayTR
                    {
                        Description = product.Description,
                        NormalPrice = $"{product.Price:F2}",
                        StockAmount = product.Quantity
                    };

                    // Add the new product object to the user basket.
                    user_basket.Add(newProduct);
                }
                string timeout_limit = "30";
                string debug_on = "0";
                string test_mode = "0";
                string no_installment = "0";
                string max_installment = "0";
                string currency = "TL";
                string lang = "";

                NameValueCollection data = new NameValueCollection();
                data["merchant_id"] = merchant_id;
                data["user_ip"] = user_ip;
                data["merchant_oid"] = merchant_oid;
                data["email"] = emailstr;
                data["payment_amount"] = payment_amountstr.ToString();

                JavaScriptSerializer ser = new JavaScriptSerializer();
                string user_basket_json = ser.Serialize(user_basket);
                string user_basketstr = Convert.ToBase64String(Encoding.UTF8.GetBytes(user_basket_json));
                data["user_basket"] = user_basketstr;

                string merged = string.Concat(merchant_id, user_ip, merchant_oid, emailstr, payment_amountstr.ToString(), user_basketstr, no_installment, max_installment, currency, test_mode, merchant_salt);
                HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(merchant_key));
                byte[] b = hmac.ComputeHash(Encoding.UTF8.GetBytes(merged));

                data["paytr_token"] = Convert.ToBase64String(b);
                data["debug_on"] = debug_on;
                data["test_mode"] = test_mode;
                data["no_installment"] = no_installment;
                data["max_installment"] = max_installment;
                data["user_name"] = user_namestr;
                data["user_address"] = user_addressstr;
                data["user_phone"] = user_phonestr;
                data["merchant_ok_url"] = merchant_ok_url;
                data["merchant_fail_url"] = merchant_fail_url;
                data["timeout_limit"] = timeout_limit;
                data["currency"] = currency;
                data["lang"] = lang;


                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    byte[] result = client.UploadValues("https://www.paytr.com/odeme/api/get-token", "POST", data);
                    string ResultAuthTicket = Encoding.UTF8.GetString(result);
                    dynamic json = JValue.Parse(ResultAuthTicket);

                    if (json.status == "success")
                    {

                        Payment payment = new Payment();
                        payment.PaymentValue = Convert.ToDecimal(model.TotalValue);
                        payment.OrderNo = model.OrderNo;
                        payment.PaymentMethod = "PAYTR";
                        payment.CreatedDate = DateTime.Now;
                        payment.PaymentToken = json.token;
                        payment.ConversationId = CD;
                        EntityEntry response = await _c.msg_Payments.AddAsync(payment);
                        await _c.SaveChangesAsync();

                        var Src = "https://www.paytr.com/odeme/guvenli/" + json.token;
                        //Process.Start(new ProcessStartInfo(Src) { UseShellExecute = true });

                        Payment_CR payment_response = new Payment_CR() { PageUrl = Src };

                        return payment_response;
                        // İşlem başarılı ise yapılacak işlemler. sipariş onaylama vb



                    }

                    else
                    {
                        Payment payment = new Payment();
                        payment.PaymentValue = Convert.ToDecimal(model.TotalValue);
                        payment.PaymentMethod = "PAYTR";
                        payment.CreatedDate = DateTime.Now;
                        payment.PaymentToken = null;
                        payment.ConversationId = CD;
                        payment.ExceptionCode = json.exceptionCode;
                        payment.ExceptionDescription = json.exceptionDescription;
                        var response = await _c.msg_Payments.AddAsync(payment);
                        await _c.SaveChangesAsync();

                        throw new Exception(json.reason);
                        // işlem başarısız ise yapılacak işlemler sepete geri döndürme vb
                    }
                }

            }
            catch (Exception ex)
            {

                throw new Exception("PAYTR IFRAME failed. reason:" + ex.Message);
            }
        }

        public async Task<bool> PayTR_SMS(Payment_CM model)
        {

            string merchant_id = _configuration["Payment:PayTR:MerchantId"];
            string merchant_key = _configuration["Payment:PayTR:ApiKey"];
            string merchant_salt = _configuration["Payment:PayTR:ApiSecretKey"];
            string name = "DAVYE ÖDEME";
            int price = Convert.ToInt32(model.TotalValue)*100;
            string max_installment = "1";
            string currency = "TL";
            string link_type = "product";
            string lang = "tr";
            string get_qr = "";
            string expiry_date = DateTime.Now.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss");
            string callback_id = "";
            string callback_link = "";
            string max_count = "";      
            string pft = "";
            string debug_on = "1";   
            string min_count = "";
            string email = "";
            string Birlestir = "";

            NameValueCollection data = new NameValueCollection();
            data["merchant_id"] = merchant_id;
            data["name"] = name;
            data["price"] = price.ToString();

            if (link_type == "product")
            {
                min_count = "1";
                Birlestir = string.Concat(name, price.ToString(), currency, max_installment, link_type, lang, min_count, merchant_salt);
            }
            else if (link_type == "collection")
            {
                email = model.User.Mail;
                Birlestir = string.Concat(name, price.ToString(), currency, max_installment, link_type, lang, email, merchant_salt);
            }
            HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(merchant_key));
            byte[] b = hmac.ComputeHash(Encoding.UTF8.GetBytes(Birlestir));
            string paytr_token = Convert.ToBase64String(b);

            // Gönderilecek veriler oluşturuluyor
            data["currency"] = currency;
            data["max_installment"] = max_installment;
            data["link_type"] = link_type;
            data["lang"] = lang;
            data["get_qr"] = get_qr;
            data["min_count"] = min_count;
            data["email"] = email;
            data["expiry_date"] = expiry_date;
            data["callback_link"] = callback_link;
            data["callback_id"] = callback_id;
            data["debug_on"] = debug_on;
            data["paytr_token"] = paytr_token;
            //
            Console.WriteLine(data["expiry_date"] = expiry_date);
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                byte[] result = client.UploadValues("https://www.paytr.com/odeme/api/link/create", "POST", data);
                string ResultAuthTicket = Encoding.UTF8.GetString(result);
                dynamic json = JValue.Parse(ResultAuthTicket);
                if (json.status == "error")
                {
                    Console.WriteLine("PAYTR LINK CREATE API request timeout. Error:" + json.err_msg + "");
                    return false;
                }
                else
                {
                    var Id = json.id.ToString().Replace("{", "").Replace("}", "");
                   var response =  await PayTR_SMS(model, Id);
                    return response;
                }
            }
          
        }

        public async Task<bool> PayTR_SMS(Payment_CM request, string Id)
        {
            // ####################### DÜZENLEMESİ ZORUNLU ALANLAR #######################
            //
            // API Entegrasyon Bilgileri - Mağaza paneline giriş yaparak BİLGİ sayfasından alabilirsiniz.
            string merchant_id = _configuration["Payment:PayTR:MerchantId"];
            string merchant_key = _configuration["Payment:PayTR:ApiKey"];
            string merchant_salt = _configuration["Payment:PayTR:ApiSecretKey"];
            //

            // Gerekli Bilgiler
            string id = Id;  // Link ID - create metodunda dönülen değerdir.
            string cell_phone = request.User.Phone; // SMS gönderilecek numara. 05 ile başlamalı ve 11 hane olmalıdır.
            string debug_on = "1";    // Hataları ekrana basmak için kullanılır.

            // Token oluşturma fonksiyonu, değiştirilmeden kullanılmalıdır.
            string Birlestir = string.Concat(id, merchant_id, cell_phone, merchant_salt);
            HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(merchant_key));
            byte[] b = hmac.ComputeHash(Encoding.UTF8.GetBytes(Birlestir));
            string paytr_token = Convert.ToBase64String(b);

            // Gönderilecek veriler oluşturuluyor
            NameValueCollection data = new NameValueCollection();
            data["merchant_id"] = merchant_id;
            data["id"] = id;
            data["cell_phone"] = cell_phone;
            data["debug_on"] = debug_on;
            data["paytr_token"] = paytr_token;
            //

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                byte[] result = client.UploadValues("https://www.paytr.com/odeme/api/link/send-sms", "POST", data);
                string ResultAuthTicket = Encoding.UTF8.GetString(result);
                dynamic json = JValue.Parse(ResultAuthTicket);
                if (json.status == "error")
                {
                    Console.WriteLine("PAYTR LINK SEND SMS API request timeout. Error:" + json.err_msg + "");
                    return false;
                }
                else
                {
                    //Response.Write(json);

                    return true;
                    /* Başarılı yanıt içerik örneği
                    [status]  => success
                    */
                }
            }

        }



    }
}
