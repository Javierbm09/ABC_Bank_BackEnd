using ABC_Bank;
using ABC_Bank.Model;
using ABC_Bank.Repositorio;
using ABC_Bank.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlite("Data Source=AbcBank.db"));


builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddScoped<IContactoRepositorio, ContactoRepositorio>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
