using Microsoft.AspNetCore.Http;

namespace GoogleAPI.Persistance.Concreates
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync( );
        Task<List<string>> GetPhotoUrlsInFolder(string folderId);
        Task<string> UploadFileAsync2(IFormFile file);
        Task<List<string>> UploadFileAsync3(List<IFormFile> files);
    }
}
