using Microsoft.EntityFrameworkCore;
using Biblioteca.Application.Interfaces;
using Biblioteca.Infrastructure.Data;
using Biblioteca.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ⬇️ Agregar el contexto de EF Core (ya lo tenías bien)
builder.Services.AddDbContext<BibliotecaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BibliotecaConnection")));

// ⬇️ Inyección de dependencias para servicios
builder.Services.AddScoped<IPersonaService, PersonaService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IPrestamoService, PrestamoService>();

// ⬇️ MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ⬇️ Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// ⬇️ Rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
