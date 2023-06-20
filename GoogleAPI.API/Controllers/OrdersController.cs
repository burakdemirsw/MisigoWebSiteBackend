using GoogleAPI.Domain.Models;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace GoogleAPI.API.Controllers
{
    [Route("api/Order")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly GooleAPIDbContext _context;
        private readonly string ErrorTextBase = "İstek Sırasında Hata Oluştu: ";
        private IOrderService _orderService;
        public OrdersController(
           GooleAPIDbContext context,
           IOrderService orderService
        )
        {
            _orderService =orderService ;
            _context = context;
        }


         [HttpGet("GenerateQRCode")]
        public  IActionResult GenerateQRCode()
        {
            try
            {
                Guid guid = Guid.NewGuid(); 
                Image qrCodeImage =  _orderService.QrCode(guid);
                qrCodeImage.Save(@$"C:\\code\{guid.ToString()}.png");
                return Ok(qrCodeImage);
            }
            catch (Exception ex)
            {
                return BadRequest("QR kodu oluşturulamadı: " + ex.Message);
            }
        }


        [HttpGet]
        public IActionResult GetSaleOrders( )
        {
            try
            {
                List<SaleOrderModel> saleOrderModel = _context.SaleOrderModels.FromSqlRaw("exec Get_ZTMSSatisSiparis").AsEnumerable().ToList();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }        
        }

        [HttpGet("{id}")]
        public IActionResult GetSaleOrdersById(string id  )
        {
            try
            {
                List<SaleOrderModel> saleOrderModel = _context.SaleOrderModels.FromSqlRaw($"exec Get_ZTMSSatisSiparisById '{id.Split(' ')[0]}' ").AsEnumerable().ToList();

                return Ok(saleOrderModel);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpPost("Add")]
        public IActionResult AddSaleBarcode(BarcodeAddModel model)
        {
            try
            {
               
                var addedEntity = _context.Entry(model);

                addedEntity.State = 
                    EntityState
                    .Added;
                _context.SaveChanges();

                return Ok(model);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetBarcodeDetail/{qrCode}")]
        public IActionResult GetBarcodeDetail(string qrCode ) 
        {

           try
            {
                List<BarcodeModel> barcodeModels = _context.BarcodeModels.FromSqlRaw($"usp_QRKontrolSorgula '{qrCode}'").AsEnumerable().ToList();
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                return Ok(barcodeModels);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
        }

        [HttpGet("GetOrderSaleDetail/{orderNumber}")]
        public IActionResult GetOrderSaleDetail( string orderNumber)
        {


            try
            {
                List<OrderSaleDetail> orderSaleDetails = _context.OrderSaleDetails.FromSqlRaw($"Get_ZTMSSatisSiparisDetay '{orderNumber}'").AsEnumerable().ToList();
                //BarcodeModel barcodeModel = barcodeModels.FirstOrDefault();
                return Ok(orderSaleDetails);
            }
            catch (Exception ex)
            {

                return BadRequest(ErrorTextBase + ex.Message);
            }
           
        }




    }
}
