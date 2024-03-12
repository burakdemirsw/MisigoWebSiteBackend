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

// CORS (Cross-Origin Resource Sharing) ayarlar� yap�l�yor.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // �zin verilen k�kenleri (origins) burada belirtiyoruz.
        policy
            .WithOrigins(
                "http://localhost:7179",
                "http://212.156.46.206:7179", //36 d�� api
                "http://192.168.2.36:7179", //36 i� api

                "http://212.156.46.206:7181", //38 d�� api
                "http://192.168.2.28:7181", //38 i� api

                "http://192.168.2.36:4200", //36 i� client
                "http://192.168.2.28:4206", //38 i� client

                "http://212.156.46.206:4201", //36 d�� client
                "http://192.168.2.36:4201",//36 i� client

                "http://localhost:4200", //bu pc

                "http://localhost:4201",
                 "http://localhost:4206"
                ) // Y�ld�z (*) kullanarak herhangi bir kayna�a izin verebilirsiniz.
            .AllowAnyHeader() // Herhangi bir ba�l�k (header) izni veriyoruz.
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers(); // Controller'lar� ekliyoruz.

// Swagger/OpenAPI belgelerini yap�land�rma.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry(); // Application Insights i�in yap�land�rma ekleniyor.

var app = builder.Build(); // Uygulamay� olu�turuyoruz.

// HTTP istek i�leme hatt�n� yap�land�rma.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.ConfigureExceptionHandler<Program>();

app.UseCors(); // CORS �zelli�ini etkinle�tiriyoruz.
app.UseHttpsRedirection(); // HTTPS'e y�nlendirme yap�l�yor.

app.UseAuthorization(); // Yetkilendirme i�lemleri i�in kullan�l�yor.

app.MapControllers(); // Controller'lara y�nlendirme yap�l�yor.

app.Run();
