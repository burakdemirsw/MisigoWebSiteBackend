using GoogleAPI.Domain.Models.Payment;
using GoogleAPI.Persistance.Concreates;
using GooleAPI.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoogleAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }


        [HttpPost("paytr-payment")]
        public async Task<ActionResult<Payment_CR>> PayTRPayment(
            Payment_CM request
        )
        {
            Payment_CR model = await _paymentService.PayTRPayment(request);
            Console.WriteLine("URL: " + model.PageUrl);
            return Ok(model);
        }

        [HttpPost("paytr-sms")]
        public async Task<ActionResult<Payment_CR>> PayTR_SMS(
           Payment_CM request 
       )
        {
           await _paymentService.PayTR_SMS(request);

            return Ok();
        }


    }
}
