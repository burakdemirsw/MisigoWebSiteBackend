using GoogleAPI.Domain.Models.NEBIM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooleAPI.Application.Abstractions
{
    public interface IProductService
    {
        public Task<string> GenerateBarcode_A(List<BarcodeModel_A> barcodes);
    }
}
