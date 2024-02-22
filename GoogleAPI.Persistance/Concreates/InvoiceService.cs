using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.NEBIM.Invoice;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace GoogleAPI.Persistance.Concreates
{
    public class InvoiceService : IInvoiceService
    {
        private GooleAPIDbContext _context;
        private IGeneralService _gs;
        private ILogService _ls;

        public InvoiceService(GooleAPIDbContext context, ILogService ls, IGeneralService gs)
        {
            _context = context;
            _ls = ls;
            _gs = gs;
        }

        public async Task<bool> AutoInvoice(string orderNumber, string procedureName, OrderBillingRequestModel requestModel, HttpContext context)
        {
            string requestUrl = context.Request.Path + context.Request.QueryString;

            List<OrderData>? OrderDataList = new List<OrderData>();
            List<OrderDataModel> orderDataList = new List<OrderDataModel>();
            try
            {
                string query = null;
                if (procedureName.Contains("WS3"))
                {
                     query = $"exec {procedureName} '{orderNumber}' ,'{requestModel.TaxedOrTaxtFree}'";
                }
                else
                {
                     query = $"exec {procedureName} '{orderNumber}'";
                }
     

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
                        await _ls.LogInvoiceError(JsonConvert.SerializeObject(OrderDataList.First()), "Faturalaştırma Sırasında Hata Alındı", "Fatura Ürünleri Boş Geldi");
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
                            Products = products,

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
                                List<OrderLineBP3> lineList = new List<OrderLineBP3>();
                                foreach (OrderLine line in orderData.Lines)
                                {
                                    OrderLineBP3 orderLineBP = new OrderLineBP3
                                    {
                                        UsedBarcode = line.UsedBarcode,
                                        ItemCode = line.ItemCode,
                                        BatchCode = line.BatchCode,
                                        ITAttributes = line.ITAttributes,
                                        LDisRate1 = line.LDisRate1,
                                        VatRate = line.VatRate,
                                        Price = line.Price,
                                        Amount = line.Amount,
                                        Qty1 = line.Qty1,
                                        DocCurrencyCode = line.DocCurrencyCode,
                                        CurrencyCode = line.CurrencyCode
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
                                List<OrderLineBP3> lineList = new List<OrderLineBP3>();
                                foreach (OrderLine line in orderData.Lines)
                                {
                                    OrderLineBP3 orderLineBP = new OrderLineBP3
                                    {
                                        UsedBarcode = line.UsedBarcode,
                                        ItemCode = line.ItemCode,
                                        BatchCode = line.BatchCode,
                                        ITAttributes = line.ITAttributes,
                                        LDisRate1 = line.LDisRate1,
                                        VatRate = line.VatRate,
                                        Price = line.Price,
                                        Amount = line.Amount,
                                        Qty1 = line.Qty1,
                                        DocCurrencyCode = line.DocCurrencyCode,
                                        CurrencyCode = line.CurrencyCode
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
                                    IsOrderBase = true,
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
                                List<OrderLineBP3> lineList = new List<OrderLineBP3>();
                                foreach (OrderLine line in orderData.Lines)
                                {
                                    OrderLineBP3 orderLineBP = new OrderLineBP3
                                    {
                                        UsedBarcode = line.UsedBarcode,
                                        ItemCode = line.ItemCode,
                                        BatchCode = line.BatchCode,
                                        ITAttributes = line.ITAttributes,
                                        LDisRate1 = line.LDisRate1,
                                        VatRate = line.VatRate,
                                        Price = line.Price,
                                        Amount = line.Amount,
                                        Qty1 = line.Qty1,
                                        DocCurrencyCode = line.DocCurrencyCode,
                                        CurrencyCode = line.CurrencyCode
                                    };

                                    lineList.Add(orderLineBP);
                                }
                                var jsonModel3 = new
                                {
                                    ModelType = 19,
                                    VendorCode = orderData.CurrAccCode,
                                    EInvoicenumber = requestModel.EInvoiceNumber ==null ?  orderData.EInvoicenumber : requestModel.EInvoiceNumber,

                                    PosTerminalID = 1,
                                    TaxTypeCode = orderData.TaxTypeCode,
                                    InvoiceDate =requestModel.InvoiceDate == null ?  DateTime.Now.ToString("yyyy-MM-dd") : requestModel.InvoiceDate?.ToString("yyyy-MM-dd"),
                                    Description = orderData.InternalDescription, //siparisNo
                                    InternalDescription = orderData.InternalDescription, //siparisNo
                                    IsOrderBase = true,
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
                                            IsOrderBase = true,
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
                                            IsOrderBase = true,
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

                        var response = await _gs.PostNebimAsync(json, "FATURA");

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
                            await _ls.LogInvoiceWarn($"Faturalaştırma Aşamasında Hata Alındı", $"Fatura Cevabı Boş Olmadığı İçin Yeniden İstek Atıldı");
                            bool invoiceResponse = await AutoInvoice(orderNumber, procedureName, requestModel, context);
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
                await _ls.LogInvoiceError("", $"Faturalaştırma Aşamasında Hata Alındı", $"{ex.Message}"+  $"{ex.StackTrace}");
                throw new Exception($"Faturalaştırma Aşamasında Hata Alındı : {ex.Message}"+  $"{ex.StackTrace}");

            }
        }

        public async Task<List<SalesPersonModel>> GetAllSalesPersonModels( )
        {
            try
            {
                List<SalesPersonModel> list = await _context.SalesPersonModels.FromSqlRaw("exec ms_GetSalesPerson ").ToListAsync();

                return list;

            }
            catch (Exception ex)
            {
                string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);
                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"{ex.Message}");
                throw new Exception(ex.ToString());
            }
        }

        public async Task<List<CountListModel>> GetInvoiceList( )
        {

            List<CountListModel> countListModels = await _context.CountListModels.FromSqlRaw($"exec Get_InvoicesList").ToListAsync();

            return countListModels;

        }

        public async Task<List<CountListModel>> GetInvoiceListByFilter(InvoiceFilterModel model)
        {

            // StartDate'i 'yyyy-MM-dd' formatına çevir
            string startDateString = model.StartDate.HasValue ? model.StartDate.Value.ToString("yyyy-MM-dd") : null;

            // EndDate'i 'yyyy-MM-dd' formatına çevir
            string endDateString = model.EndDate.HasValue ? model.EndDate.Value.ToString("yyyy-MM-dd") : null;

            string query = "SELECT MAX(ItemDate) AS LastUpdateDate, SUM(Quantity) AS TotalProduct, OrderNumber AS OrderNo FROM ZTMSRAFSAYIM3 Where len(OrderNumber)>1 query1  GROUP BY OrderNumber query2 ORDER BY LastUpdateDate DESC;";
            string addedQuery = "";
            string addedQuery2 = "";
            if (model.OrderNo != null)
            {
                addedQuery += $"and orderNumber like '{model.OrderNo}%'";
            }
            if (model.InvoiceType != null)
            {
                if (model.InvoiceType == "Alış")
                {
                    addedQuery += $" and OrderNumber like 'BPI%'";

                }
                else
                {
                    addedQuery += $" and OrderNumber like 'WSI%'";

                }
            }
            if (model.StartDate != null)
            {

                addedQuery2 += $"having MAX(ItemDate) >= {startDateString}  ";
            }
            if (model.EndDate != null)
            {
                if (addedQuery2.Contains("having"))
                {
                    addedQuery2 += $"and MAX(ItemDate) <= '{endDateString}'  ";
                }
                else
                {
                    addedQuery2 += $" having MAX(ItemDate) <= '{endDateString}'  ";

                }

            }
            query = query.Replace("query1", addedQuery);
            query = query.Replace("query2", addedQuery2);

            List<CountListModel> countListModels = await _context.CountListModels.FromSqlRaw(query).ToListAsync();

            return countListModels;

        }

        public async Task<bool> DeleteInvoiceProducts(string orderNumber)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            var affectedRow = await _context.Database.ExecuteSqlRawAsync($"delete ZTMSRAFInvoiceDetailBP WHERE  OrdernumberRaf ='{orderNumber}' ");
            if (affectedRow > 0)
            {
                await _ls.LogInvoiceSuccess($"{methodName} İşlemi Başarılı", $"İşlem Başarılı");
                return true;

            }
            else
            {
                return false;
            }

        }

        public async Task<bool> BillingOrder(OrderBillingRequestModel model, HttpContext httpContext)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            List<string> productIds = new List<string> {
        model.OrderNo
      };

            bool result2;

            switch (model.InvoiceModel)
            {
                case 1: // ALIŞ FATURASI (oluşturulmamış)
                    result2 = await AutoInvoice(model.OrderNo.ToString(), "usp_GetOrderForInvoiceToplu_BP2", model, httpContext);
                    break;

                case 2: // alış sipariş
                    if (model.OrderNo.Contains("BP") && !model.InvoiceType)
                    {
                        result2 = await AutoInvoice(model.OrderNo, "usp_GetOrderForInvoiceToplu_BP", model, httpContext);
                    }
                    else
                    {
                        result2 = false; // Handle other cases if needed
                    }
                    break;

                case 3: // satış faturası
                    result2 = await AutoInvoice(model.OrderNo, "usp_GetOrderForInvoiceToplu_WS2", model, httpContext);
                    break;

                case 4: // satış sipariş faturası
                    if (model.OrderNo.Contains("WS") && !model.InvoiceType)
                    {
                        result2 = await AutoInvoice(model.OrderNo, "usp_GetOrderForInvoiceToplu_WS3", model, httpContext);
                    }
                    else if (model.OrderNo.Contains("R") && !model.InvoiceType)
                    {
                        result2 = await AutoInvoice(model.OrderNo, "usp_GetOrderForInvoiceToplu_R", model, httpContext);
                    }
                    else
                    {
                        result2 = false; // Handle other cases if needed
                    }
                    break;

                default:
                    result2 = false; // Handle other cases if needed
                    break;
            }

            if (result2)
            { 
                await _ls.LogInvoiceSuccess($"{methodName} İşlemi Başarılı", $"İşlem Başarılı");
                return true;
                // Continue with additional processing if needed
            }
            else
            {

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Obje Null Değer Döndürdü");
                throw new Exception("Result 2 Is NULL");
            }

        }

        public async Task<List<SalesPersonModel>> GetSalesPersonModels( )
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);

            List<SalesPersonModel> list = await GetAllSalesPersonModels();
            if (list.Count < 1)
            {

                await _ls.LogOrderWarn($"{methodName} Sırasında Hata Alındı", $"Satış Elemanlarının Listesi Boş Geldi");
                throw new Exception("Satış Elemanlarının Listesi Boş Geldi");
            }
            else
            {
                await _ls.LogInvoiceSuccess($"{methodName} İşlemi Başarılı", $"İşlem Başarılı");
                return list;
            }
        }

        public async Task<List<CreatePurchaseInvoice>> GetProductOfInvoice(string invoiceId)
        {
            string methodName = await _gs.GetCurrentMethodName(MethodBase.GetCurrentMethod().ReflectedType.Name);


            List<CreatePurchaseInvoice> collectedProduct = await _context.CreatePurchaseInvoices.FromSqlRaw($"exec [Get_ProductOfInvoice] '{invoiceId}'").ToListAsync();
            return collectedProduct;

        }


    }

}