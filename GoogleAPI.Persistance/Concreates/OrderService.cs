using GooleAPI.Application.Abstractions;
using System.Drawing;
using System.Threading.Tasks;
using QRCoder;



namespace GoogleAPI.Persistance.Concretes
{
    public class OrderService : IOrderService  // IOrderService bir arayüz olabilir, işlevinizi tanımlar
    {
        public Task<Bitmap> GetOrderDetailsFromQrCode(string data)
        {
            throw new NotImplementedException();
        }

        public Image QrCode(Guid id)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(id.ToString(), QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);

            byte[] qrCodeBytes = qrCode.GetGraphic(20);
            Bitmap qrCodeImage = new Bitmap(new MemoryStream(qrCodeBytes));

            return qrCodeImage;
        }
    }


}
