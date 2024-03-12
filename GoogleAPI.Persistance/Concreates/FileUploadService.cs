using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace GoogleAPI.Persistance.Concreates
{
    internal class FileUploadService : IFileUploadService
    {
        string credientialPath = @"C:/code/cred.json";
        string folderId = "1g0hJpcUowhgKINq7ySFRRkdygdWJBBSa"; //DAVYE
        string[] filesToUpload = { @"C:/code/video.mp4" };
        public async Task<string> UploadFileAsync( )
        {


            GoogleCredential credential;
            using (var stream = new FileStream(credientialPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(new[] { DriveService.ScopeConstants.DriveFile });
            }

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Drive Upload Console App"
            });

            foreach (var path in filesToUpload)
            {
                var file = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = Path.GetFileName(path),
                    Parents = new List<string> { folderId }
                };

                FilesResource.CreateMediaUpload request;
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    request = service.Files.Create(file, stream, "");
                    request.Fields = "id";
                    await request.UploadAsync();

                    var fileId = request.ResponseBody?.Id;
                    if (fileId != null)
                    {
                        var fileUrl = $"https://drive.google.com/uc?id={fileId}";
                        return fileUrl;
                    }
                }

                // Dosyanın URL bilgisini almak için dosyanın ID'sini kullanabiliriz.

            }

            // Eğer dosya yükleme işlemi başarısız olursa null döndürebiliriz.
            return null;

        }


        public async Task<string> UploadFileAsync2(IFormFile file)
        {
            GoogleCredential credential;
            using (var stream = new FileStream(credientialPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(new[] { DriveService.ScopeConstants.DriveFile });
            }

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Drive Upload Console App"
            });

            var driveFile = new Google.Apis.Drive.v3.Data.File()
            {
                Name = file.FileName,
                Parents = new List<string> { folderId }
            };

            FilesResource.CreateMediaUpload request;
            using (var stream = file.OpenReadStream())
            {
                request = service.Files.Create(driveFile, stream, "");
                request.Fields = "id, webViewLink";
                await request.UploadAsync();

                var uploadedFile = request.ResponseBody;
                if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.WebViewLink))
                {
                    // Dosyayı herkese açık yapmak için erişim izinlerini ayarlayın
                    var permission = new Google.Apis.Drive.v3.Data.Permission()
                    {
                        Type = "anyone",
                        Role = "reader"
                    };
                    await service.Permissions.Create(permission, uploadedFile.Id).ExecuteAsync();



                    return $"https://lh3.googleusercontent.com/d/{uploadedFile.Id}";
                }
            }

            return null;
        }

        public async Task<List<string>> UploadFileAsync3(List<IFormFile> files)
        {
            GoogleCredential credential;

            List<string> urls = new List<string>();
            using (var stream = new FileStream(credientialPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(new[] { DriveService.ScopeConstants.DriveFile });
            }

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Drive Upload Console App"
            });

            foreach (var file in files)
            {
                var driveFile = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = file.FileName,
                    Parents = new List<string> { folderId }
                };

                FilesResource.CreateMediaUpload request;
                using (var stream = file.OpenReadStream())
                {
                    request = service.Files.Create(driveFile, stream, "");
                    request.Fields = "id, webViewLink";
                    await request.UploadAsync();

                    var uploadedFile = request.ResponseBody;
                    if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.WebViewLink))
                    {
                        //Dosyayı herkese açık yapmak için erişim izinlerini ayarlayın
                        var permission = new Google.Apis.Drive.v3.Data.Permission()
                        {
                            Type = "anyone",
                            Role = "reader"
                        };
                        await service.Permissions.Create(permission, uploadedFile.Id).ExecuteAsync();
                        Console.WriteLine(JsonSerializer.Serialize(uploadedFile));


                        var _url = $"https://lh3.googleusercontent.com/d/{uploadedFile.Id}";
                        //var _url = $"https://drive.google.com/uc?id={uploadedFile.Id}";
                        //var _url = $"https://drive.usercontent.google.com/download?id={uploadedFile.Id}";
                        urls.Add(_url);

                    }
                }
            }

            return urls;



        }

        //     var fileUrl = $"https://lh3.googleusercontent.com/d/{fileId}";

        public async Task<List<string>> GetPhotoUrlsInFolder(string folderId)
        {
            GoogleCredential credential;
            using (var stream = new FileStream(credientialPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(new[] { DriveService.ScopeConstants.DriveFile });
            }

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Drive API"
            });

            var request = service.Files.List();
            request.Q = $"'{folderId}' in parents";
            request.Fields = "files(webContentLink)";
            var response = await request.ExecuteAsync();

            return response.Files.Select(file => file.WebContentLink).ToList();
        }

    }
}
