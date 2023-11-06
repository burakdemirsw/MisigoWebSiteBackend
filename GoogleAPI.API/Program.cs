using GooleAPI.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Servisler ekleniyor.
builder.Services.AddPersistanceServices();

// CORS (Cross-Origin Resource Sharing) ayarlar� yap�l�yor.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // �zin verilen k�kenleri (origins) burada belirtiyoruz.
        policy
            .WithOrigins(
                "http://localhost:7178",
                "http://212.156.46.206:7178",
                "http://192.168.2.36:7178",
                "http://212.156.46.206:4200",
                "http://localhost:4200",
                "http://192.168.2.36:4200",
                "http://212.156.46.206:4202",
                "http://localhost:4202",
                "http://192.168.2.36:4202") // Y�ld�z (*) kullanarak herhangi bir kayna�a izin verebilirsiniz.
            .AllowAnyHeader() // Herhangi bir ba�l�k (header) izni veriyoruz.
            .AllowAnyMethod(); // Herhangi bir HTTP metodunun kullan�m�na izin veriyoruz.
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

app.UseCors(); // CORS �zelli�ini etkinle�tiriyoruz.
app.UseHttpsRedirection(); // HTTPS'e y�nlendirme yap�l�yor.

app.UseAuthorization(); // Yetkilendirme i�lemleri i�in kullan�l�yor.

app.MapControllers(); // Controller'lara y�nlendirme yap�l�yor.

app.Run();
