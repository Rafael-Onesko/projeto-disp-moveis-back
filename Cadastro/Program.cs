using Cadastro.Domain;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
const string SQLroute = "Server=localhost\\SQLEXPRESS;Database=projeto_site_db;Trusted_Connection=True;TrustServerCertificate=True;";
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Culture = CultureInfo.InvariantCulture;
    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Unspecified;
    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
    options.UseCamelCasing(true);
});

builder.Services.AddDbContext<Contexto>(options => options.UseSqlServer(SQLroute));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
