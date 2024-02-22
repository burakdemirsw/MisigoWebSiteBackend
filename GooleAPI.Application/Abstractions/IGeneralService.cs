using System.Drawing;

namespace GooleAPI.Application.Abstractions
{
    public interface IGeneralService
    {
        public Task<string> ConnectIntegrator( );
        public Task<string> PostNebimAsync(string content, string OperationType);
        public Task<string> GetCurrentMethodName(string name);
        public Task ClearFolder( );
        public Task PrintWithoutDialog(Bitmap image);
        public Image QrCode(string id);
        public string ConvertImageToBase64(Image image);
        public Task<Boolean> GenerateReceipt(List<string> orderNumbers);
        public Task PrintInvoice(string HtmlPath, int width, int height, string type);
        public Task<Bitmap> HtmlToImage(string html, string path, int width, int height, string type);
    }
}