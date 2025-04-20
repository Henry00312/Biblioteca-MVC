using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca_MVC.Models;
using Biblioteca_MVC.Helpers;

namespace Biblioteca_MVC.Controllers
{
    public class PrestamoController : Controller
    {
        private readonly BibliotecaContext _context;

        public PrestamoController(BibliotecaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var bibliotecaContext = _context.Prestamos.Include(p => p.Material).Include(p => p.Persona);
            return View(await bibliotecaContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var prestamo = await _context.Prestamos
                .Include(p => p.Material)
                .Include(p => p.Persona)
                .FirstOrDefaultAsync(m => m.Id == id);

            return prestamo == null ? NotFound() : View(prestamo);
        }

        public IActionResult Create()
        {
            ViewData["MaterialId"] = new SelectList(_context.Materiales, "Id", "Titulo");
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "Cedula");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaterialId,PersonaId")] Prestamo prestamo)
        {
            if (ModelState.IsValid)
            {
                var persona = await _context.Personas
                    .Include(p => p.Prestamos)
                    .FirstOrDefaultAsync(p => p.Id == prestamo.PersonaId);

                var material = await _context.Materiales
                    .FirstOrDefaultAsync(m => m.Id == prestamo.MaterialId);

                if (persona == null || material == null)
                {
                    ModelState.AddModelError("", "Persona o material no válido.");
                    ViewData["MaterialId"] = new SelectList(_context.Materiales, "Id", "Titulo", prestamo.MaterialId);
                    ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "Cedula", prestamo.PersonaId);
                    return View(prestamo);
                }

                int limite = persona.Rol switch
                {
                    "Estudiante" => 5,
                    "Profesor" => 3,
                    "Administrativo" => 1,
                    _ => 0
                };

                int prestamosActivos = await _context.Prestamos
                    .CountAsync(p => p.PersonaId == persona.Id && p.Tipo == TiposMovimiento.Prestamo);

                if (prestamosActivos >= limite)
                {
                    ModelState.AddModelError("", "La persona ya alcanzó el límite de materiales prestados según su rol.");
                    ViewData["MaterialId"] = new SelectList(_context.Materiales, "Id", "Titulo", prestamo.MaterialId);
                    ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "Cedula", prestamo.PersonaId);
                    return View(prestamo);
                }

                if (material.CantidadActual <= 0)
                {
                    ModelState.AddModelError("", "No hay unidades disponibles de este material.");
                    ViewData["MaterialId"] = new SelectList(_context.Materiales, "Id", "Titulo", prestamo.MaterialId);
                    ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "Cedula", prestamo.PersonaId);
                    return View(prestamo);
                }

                prestamo.Fecha = DateTime.Now;
                prestamo.Tipo = TiposMovimiento.Prestamo;
                material.CantidadActual -= 1;

                _context.Add(prestamo);
                _context.Update(material);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Si ModelState es inválido por validaciones de data annotations
            ViewData["MaterialId"] = new SelectList(_context.Materiales, "Id", "Titulo", prestamo.MaterialId);
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "Cedula", prestamo.PersonaId);
            return View(prestamo);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var prestamo = await _context.Prestamos.FindAsync(id);
            if (prestamo == null) return NotFound();

            ViewData["MaterialId"] = new SelectList(_context.Materiales, "Id", "Titulo", prestamo.MaterialId);
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "Cedula", prestamo.PersonaId);
            return View(prestamo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tipo,Fecha,MaterialId,PersonaId")] Prestamo prestamo)
        {
            if (id != prestamo.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prestamo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrestamoExists(prestamo.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaterialId"] = new SelectList(_context.Materiales, "Id", "Titulo", prestamo.MaterialId);
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "Cedula", prestamo.PersonaId);
            return View(prestamo);
        }

        // GET: Prestamo/Devolver/5
        public async Task<IActionResult> Devolver(int? id)
        {
            if (id == null)
                return NotFound();

            var prestamo = await _context.Prestamos
                .Include(p => p.Material)
                .Include(p => p.Persona)
                .FirstOrDefaultAsync(p => p.Id == id && p.Tipo == TiposMovimiento.Prestamo);

            if (prestamo == null)
                return NotFound();

            // Crear nuevo registro tipo Devolución
            var devolucion = new Prestamo
            {
                Tipo = TiposMovimiento.Devolucion,
                Fecha = DateTime.Now,
                MaterialId = prestamo.MaterialId,
                PersonaId = prestamo.PersonaId
            };

            // Aumentar inventario
            prestamo.Material.CantidadActual += 1;

            _context.Prestamos.Add(devolucion);
            _context.Materiales.Update(prestamo.Material);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool PrestamoExists(int id)
        {
            return _context.Prestamos.Any(e => e.Id == id);
        }
    }
}
