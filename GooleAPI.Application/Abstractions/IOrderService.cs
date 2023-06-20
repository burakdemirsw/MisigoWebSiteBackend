using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooleAPI.Application.Abstractions
{
    public interface IOrderService
    {
        public Image QrCode(Guid texidt);

        public Task<Bitmap> GetOrderDetailsFromQrCode(string data );
    }
}
