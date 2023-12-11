using GooleAPI.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Servisler ekleniyor.
builder.Services.AddPersistanceServices();

// CORS (Cross-Origin Resource Sharing) ayarlarý yapýlýyor.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // Ýzin verilen kökenleri (origins) burada belirtiyoruz.
        policy
            .WithOrigins(
                "http://localhost:7179",
                "http://212.156.46.206:7179",
                "http://192.168.2.36:7179",
                "http://212.156.46.206:4201",
                "http://localhost:4201",
                "http://192.168.2.36:4201") // Yýldýz (*) kullanarak herhangi bir kaynaða izin verebilirsiniz.
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

app.UseCors(); // CORS özelliðini etkinleþtiriyoruz.
app.UseHttpsRedirection(); // HTTPS'e yönlendirme yapýlýyor.

app.UseAuthorization(); // Yetkilendirme iþlemleri için kullanýlýyor.

app.MapControllers(); // Controller'lara yönlendirme yapýlýyor.

app.Run();
