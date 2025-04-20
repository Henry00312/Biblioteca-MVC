using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteca_MVC.Models;
using Biblioteca_MVC.Helpers;


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
            return View(await _context.Materiales.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var material = await _context.Materiales.FirstOrDefaultAsync(m => m.Id == id);
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

            var material = await _context.Materiales.FindAsync(id);
            return material == null ? NotFound() : View(material);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,FechaRegistro,CantidadRegistrada,CantidadActual")] Material material)
        {
            if (id != material.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(material);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialExists(material.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(material);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var material = await _context.Materiales.FirstOrDefaultAsync(m => m.Id == id);
            return material == null ? NotFound() : View(material);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _context.Materiales
                .Include(m => m.Prestamos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (material == null)
                return NotFound();

            if (material.Prestamos.Any())
            {
                ModelState.AddModelError("", "No se puede eliminar este material porque tiene préstamos o devoluciones registrados.");
                return View(material);
            }

            _context.Materiales.Remove(material);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaterialExists(int id)
        {
            return _context.Materiales.Any(e => e.Id == id);
        }

        // GET: Material/Incrementar/5
        public async Task<IActionResult> Incrementar(int? id)
        {
            if (id == null)
                return NotFound();

            var material = await _context.Materiales.FindAsync(id);
            return material == null ? NotFound() : View(material);
        }

        // POST: Material/Incrementar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Incrementar(int id, int cantidad)
        {
            var material = await _context.Materiales.FindAsync(id);
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
    }
}
