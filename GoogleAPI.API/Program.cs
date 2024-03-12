using GoogleAPI.API.Extentions;
using GoogleAPI.Persistance.Concreates;
using GooleAPI.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Servisler ekleniyor.
builder.Services.AddPersistanceServices();


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        "Admin",
        options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = builder.Configuration["Token:Audience"],
                ValidIssuer = builder.Configuration["Token:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])
                ),
                LifetimeValidator = (notBefore, expires, securityToken, validationParameters) =>
                    expires != null ? expires > DateTime.UtcNow : false,
                NameClaimType = ClaimTypes.Name
            };
        }
    );

// CORS (Cross-Origin Resource Sharing) ayarlarý yapýlýyor.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // Ýzin verilen kökenleri (origins) burada belirtiyoruz.
        policy
            .WithOrigins(
                "http://localhost:7179",
                "http://212.156.46.206:7179", //36 dýþ api
                "http://192.168.2.36:7179", //36 iç api

                "http://212.156.46.206:7181", //38 dýþ api
                "http://192.168.2.28:7181", //38 iç api

                "http://192.168.2.36:4200", //36 iç client
                "http://192.168.2.28:4206", //38 iç client

                "http://212.156.46.206:4201", //36 dýþ client
                "http://192.168.2.36:4201",//36 iç client

                "http://localhost:4200", //bu pc

                "http://localhost:4201",
                 "http://localhost:4206"
                ) // Yýldýz (*) kullanarak herhangi bir kaynaða izin verebilirsiniz.
            .AllowAnyHeader() // Herhangi bir baþlýk (header) izni veriyoruz.
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers(); // Controller'larý ekliyoruz.

// Swagger/OpenAPI belgelerini yapýlandýrma.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry(); // Application Insights için yapýlandýrma ekleniyor.

var app = builder.Build(); // Uygulamayý oluþturuyoruz.

// HTTP istek iþleme hattýný yapýlandýrma.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.ConfigureExceptionHandler<Program>();

app.UseCors(); // CORS özelliðini etkinleþtiriyoruz.
app.UseHttpsRedirection(); // HTTPS'e yönlendirme yapýlýyor.

app.UseAuthorization(); // Yetkilendirme iþlemleri için kullanýlýyor.

app.MapControllers(); // Controller'lara yönlendirme yapýlýyor.

app.Run();
