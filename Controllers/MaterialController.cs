using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteca_MVC.Models;

namespace Biblioteca_MVC.Controllers
{
    public class MaterialController : Controller
    {
        private readonly BibliotecaContext _context;

        public MaterialController(BibliotecaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var materiales = await _context.Materiales
                .Where(m => m.Activo)
                .ToListAsync();

            return View(materiales);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var material = await _context.Materiales
                .FirstOrDefaultAsync(m => m.Id == id && m.Activo);

            return material == null ? NotFound() : View(material);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,FechaRegistro,CantidadRegistrada,CantidadActual")] Material material)
        {
            if (_context.Materiales.Any(m => m.Id == material.Id))
            {
                ModelState.AddModelError("Id", "Ya existe un material con ese ID.");
            }

            if (material.CantidadActual != material.CantidadRegistrada)
            {
                ModelState.AddModelError("CantidadActual", "La cantidad actual debe ser igual a la registrada inicialmente.");
            }

            if (ModelState.IsValid)
            {
                material.Activo = true;
                _context.Add(material);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(material);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var material = await _context.Materiales.FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            return material == null ? NotFound() : View(material);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,FechaRegistro,CantidadRegistrada,CantidadActual")] Material material)
        {
            if (id != material.Id)
                return NotFound();

            if (material.CantidadActual > material.CantidadRegistrada)
            {
                ModelState.AddModelError("CantidadActual", "La cantidad actual no puede ser mayor a la registrada.");
            }

            if (ModelState.IsValid)
            {
                var original = await _context.Materiales.FirstOrDefaultAsync(m => m.Id == id && m.Activo);
                if (original == null)
                    return NotFound();

                original.Titulo = material.Titulo;
                original.FechaRegistro = material.FechaRegistro;
                original.CantidadRegistrada = material.CantidadRegistrada;
                original.CantidadActual = material.CantidadActual;

                _context.Update(original);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(material);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var material = await _context.Materiales
                .FirstOrDefaultAsync(m => m.Id == id && m.Activo);

            return material == null ? NotFound() : View(material);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _context.Materiales
                .Include(m => m.Prestamos)
                .FirstOrDefaultAsync(m => m.Id == id && m.Activo);

            if (material == null)
                return NotFound();

            if (material.Prestamos.Any())
            {
                ModelState.AddModelError("", "No se puede eliminar este material porque tiene préstamos registrados.");
                return View(material);
            }

            // ✅ Borrado lógico
            material.Activo = false;
            _context.Update(material);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Incrementar(int? id)
        {
            if (id == null)
                return NotFound();

            var material = await _context.Materiales.FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            return material == null ? NotFound() : View(material);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Incrementar(int id, int cantidad)
        {
            var material = await _context.Materiales.FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (material == null)
                return NotFound();

            if (cantidad <= 0)
            {
                ModelState.AddModelError("", "La cantidad a incrementar debe ser mayor que cero.");
                return View(material);
            }

            material.CantidadRegistrada += cantidad;
            material.CantidadActual += cantidad;

            _context.Update(material);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool MaterialExists(int id)
        {
            return _context.Materiales.Any(e => e.Id == id && e.Activo);
        }
    }
}
