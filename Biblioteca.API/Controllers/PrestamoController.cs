using Microsoft.AspNetCore.Mvc;
using Biblioteca.Application.Interfaces;
using Biblioteca.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Biblioteca.API.Controllers
{
    public class PrestamoController : Controller
    {
        private readonly IMaterialService _materialService;
        private readonly IPersonaService _personaService;
        private readonly IPrestamoService _prestamoService;

        public PrestamoController(
            IPrestamoService prestamoService,
            IMaterialService materialService,
            IPersonaService personaService)
        {
            _prestamoService = prestamoService;
            _materialService = materialService;
            _personaService = personaService;
        }

        public async Task<IActionResult> Index()
        {
            var prestamos = await _prestamoService.GetAllAsync();
            return View(prestamos);
        }

        // GET: Prestamo/Create
        public async Task<IActionResult> Create()
        {
            var personas = (await _personaService.GetAllAsync())
                .Where(p => p.Activo) // ✅ Solo personas activas
                .ToList();

            var materiales = await _materialService.GetAllAsync();

            ViewBag.Personas = new SelectList(
                personas.Select(p => new { p.Id, NombreCompleto = $"{p.Cedula} - {p.Nombre}" }),
                "Id", "NombreCompleto"
            );

            ViewBag.Materiales = new SelectList(materiales, "Id", "Titulo");

            return View();
        }

        // POST: Prestamo/Create
        [HttpPost]
        public async Task<IActionResult> Create(Prestamo prestamo)
        {
            if (!ModelState.IsValid)
            {
                var personas = (await _personaService.GetAllAsync())
                    .Where(p => p.Activo)
                    .ToList();

                var materiales = await _materialService.GetAllAsync();

                ViewBag.Personas = new SelectList(
                    personas.Select(p => new { p.Id, NombreCompleto = $"{p.Cedula} - {p.Nombre}" }),
                    "Id", "NombreCompleto", prestamo.PersonaId
                );

                ViewBag.Materiales = new SelectList(materiales, "Id", "Titulo", prestamo.MaterialId);

                return View(prestamo);
            }

            await _prestamoService.AddAsync(prestamo);
            return RedirectToAction(nameof(Index));
        }

        // GET: Prestamo/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var prestamo = await _prestamoService.GetByIdAsync(id);
            if (prestamo == null) return NotFound();
            return View(prestamo);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Prestamo prestamo)
        {
            if (!ModelState.IsValid)
                return View(prestamo);

            await _prestamoService.UpdateAsync(prestamo);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Devolver(int id)
        {
            var prestamo = await _prestamoService.GetByIdAsync(id);
            if (prestamo == null) return NotFound();

            // Solo si aún no ha sido devuelto
            if (!prestamo.FechaDevolucion.HasValue)
            {
                prestamo.FechaDevolucion = DateTime.Now;
                prestamo.Tipo = "Devolucion";

                await _prestamoService.UpdateAsync(prestamo);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var prestamo = await _prestamoService.GetByIdAsync(id);
            if (prestamo == null) return NotFound();
            return View(prestamo);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _prestamoService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
