using GoogleAPI.Domain.Models.NEBIM.Customer;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Domain.Models.Payment;
using System;
using System.Collections.Generic;
using System.Linq;  
using System.Text;
using System.Threading.Tasks;

namespace GooleAPI.Application.Abstractions
{
    public interface IPaymentService
    {
        Task<Payment_CR> PayTRPayment(Payment_CM model);

        Task<bool> PayTR_SMS(Payment_CM model);
    }


}
