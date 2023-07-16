using GooleAPI.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddPersistanceServices();
builder.Services.AddCors(
    options =>
        options.AddDefaultPolicy(
            policy =>
                policy
                    .WithOrigins( "http://localhost:7178", "http://212.156.46.206:7178", "http://192.168.2.38:7178", "http://212.156.46.206:4200", "http://localhost:4200", "http://192.168.2.38:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
        )
);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
