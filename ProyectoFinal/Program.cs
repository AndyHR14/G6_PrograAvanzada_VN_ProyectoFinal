using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

// Para poder utilizar el servicio de Bitacora
builder.Services.AddScoped<IBitacoraService, BitacoraService>();

// Configurar la conexión a MySQL
builder.Services.AddDbContext<AppDbContext>(

    options =>
           options.UseMySQL(builder.Configuration.GetConnectionString("MySqlConnection"))

    );


var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=login}/{action=Login}/{id?}");

app.Run();