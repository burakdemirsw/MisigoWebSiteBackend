﻿namespace GoogleAPI.Domain.Models.NEBIM.Invoice
{
    public class InvoiceErrorResponseModel
    {
        public int ModelType { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public string ErrorSource { get; set; }
        public int StatusCode { get; set; }
    }
}
