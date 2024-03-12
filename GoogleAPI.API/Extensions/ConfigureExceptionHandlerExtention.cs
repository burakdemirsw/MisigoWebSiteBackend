using GooleAPI.Application.Abstractions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace GoogleAPI.API.Extentions
{
    public static class ConfigureExceptionHandlerExtention
    {
        public static void ConfigureExceptionHandler<T>(
            this WebApplication application
       
        )
        {
            application.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = MediaTypeNames.Application.Json;

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature != null)
                    {
                        var logService = context.RequestServices.GetRequiredService<ILogService>();
                        await logService.LogSystemError(context.Request.Path,"SYSTEM ERROR", contextFeature.Error.Message+"Inner Exception: \n\n"+ contextFeature.Error.InnerException);

                        await context.Response.WriteAsync(
                            JsonSerializer.Serialize(
                                new
                                {
                                    StatusCode = context.Response.StatusCode,
                                    Message = contextFeature.Error.Message,
                                    Title = "Hata alındı",
                                    InnerException = contextFeature.Error.InnerException
                                }
                            )
                        );
                    }
                });
            });
        }
    }
}
