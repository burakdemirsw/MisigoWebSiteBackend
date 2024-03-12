using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.File
{
    public class UploadFileModel
    {
        public IFormFile File { get; set; }
    }
    public class UploadFilesModel
    {
        public List<IFormFile> Files { get; set; }
    }

    public class UploadFileResultModel
    {
        public string? Result { get; set; }
        public string? Url { get; set; }
    }
}
