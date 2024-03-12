using GoogleAPI.Domain.Models.File;
using GoogleAPI.Persistance.Concreates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoogleAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        IFileUploadService _uploadService;

        public FilesController(IFileUploadService uploadService)
        {
            _uploadService = uploadService;
        }


        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileModel uploadFileModel)
        {
            var response = await _uploadService.UploadFileAsync2(uploadFileModel.File);
            UploadFileResultModel uploadResult = new UploadFileResultModel();
            uploadResult.Url = response.ToString();
            uploadResult.Result = "OK";
            return Ok(uploadResult);
        }

        [HttpPost("upload-files")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFilesModel uploadFilesModel)
        {
            var response = await _uploadService.UploadFileAsync3(uploadFilesModel.Files);

            return Ok(response);
        }

    }
}
