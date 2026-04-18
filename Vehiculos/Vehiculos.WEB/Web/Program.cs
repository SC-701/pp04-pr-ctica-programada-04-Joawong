using Abstracciones.Interfaces.Reglas;
//using Autorizacion.Abstracciones.DA;
//using Autorizacion.Abstracciones.Flujo;
//using Autorizacion.DA;
//using Autorizacion.DA.Repositorios;
//using Autorizacion.Flujo;
//using Autorizacion.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Reglas;
using Autorizacion.Abstracciones.DA;        // ★
using Autorizacion.Abstracciones.Flujo;     // ★
using Autorizacion.DA;                      // ★
using Autorizacion.DA.Repositorios;        // ★
using Autorizacion.Flujo;                  // ★
using Autorizacion.Middleware;             // ★

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddScoped<IConfiguracion, Configuracion>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Seguridad/Login";
        options.LogoutPath = "/Seguridad/Logout";
        options.AccessDeniedPath = "/Seguridad/AccesoDenegado";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
    });

// ★ Servicios del paquete NuGet de autorización (para AutorizacionClaims)
builder.Services.AddTransient<IAutorizacionFlujo, AutorizacionFlujo>();
builder.Services.AddTransient<ISeguridadDA, SeguridadDA>();
builder.Services.AddTransient<IRepositorioDapper, RepositorioDapper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();    // ★ lee la cookie → llena HttpContext.User
app.AutorizacionClaims();   // ★ agrega claims de rol desde BD de seguridad
app.UseAuthorization();     // ★ verifica [Authorize]


app.MapRazorPages(); 

app.Run();