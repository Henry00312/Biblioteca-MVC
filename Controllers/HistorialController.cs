using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteca_MVC.Models;

namespace Biblioteca_MVC.Controllers
{
    public class HistorialController : Controller
    {
        private readonly BibliotecaContext _context;

        public HistorialController(BibliotecaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var historial = await _context.Prestamos
                .Include(p => p.Persona)
                .Include(p => p.Material)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            return View(historial);
        }
    }
}
