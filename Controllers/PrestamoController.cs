using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca_MVC.Models;

namespace Biblioteca_MVC.Controllers
{
    public class PrestamoController : Controller
    {
        private readonly BibliotecaContext _context;

        public PrestamoController(BibliotecaContext context)
        {
            _context = context;
        }

        // GET: Prestamo
        public async Task<IActionResult> Index()
        {
            var bibliotecaContext = _context.Prestamos.Include(p => p.Material).Include(p => p.Persona);
            return View(await bibliotecaContext.ToListAsync());
        }

        // GET: Prestamo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prestamo = await _context.Prestamos
                .Include(p => p.Material)
                .Include(p => p.Persona)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prestamo == null)
            {
                return NotFound();
            }

            return View(prestamo);
        }

        // GET: Prestamo/Create
        public IActionResult Create()
        {
            ViewData["MaterialId"] = new SelectList(_context.Materiales, "Id", "Titulo");
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "Cedula");
            return View();
        }

        // POST: Prestamo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tipo,Fecha,MaterialId,PersonaId")] Prestamo prestamo)
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
                    return View(prestamo);
                }

                // Lógica de límite por rol
                int limite = persona.Rol switch
                {
                    "Estudiante" => 5,
                    "Profesor" => 3,
                    "Administrativo" => 1,
                    _ => 0
                };

                int prestamosActivos = await _context.Prestamos
                    .CountAsync(p => p.PersonaId == persona.Id && p.Tipo == "Prestamo");

                if (prestamosActivos >= limite)
                {
                    ModelState.AddModelError("", "La persona ya alcanzó el límite de materiales prestados según su rol.");
                    return View(prestamo);
                }

                if (material.CantidadActual <= 0)
                {
                    ModelState.AddModelError("", "No hay unidades disponibles de este material.");
                    return View(prestamo);
                }

                // Registrar préstamo
                prestamo.Fecha = DateTime.Now;
                prestamo.Tipo = "Prestamo";
                material.CantidadActual -= 1;

                _context.Add(prestamo);
                _context.Update(material);
                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));

        }

        // GET: Prestamo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prestamo = await _context.Prestamos.FindAsync(id);
            if (prestamo == null)
            {
                return NotFound();
            }
            ViewData["MaterialId"] = new SelectList(_context.Materiales, "Id", "Titulo", prestamo.MaterialId);
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "Cedula", prestamo.PersonaId);
            return View(prestamo);
        }

        // POST: Prestamo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tipo,Fecha,MaterialId,PersonaId")] Prestamo prestamo)
        {
            if (id != prestamo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prestamo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrestamoExists(prestamo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaterialId"] = new SelectList(_context.Materiales, "Id", "Titulo", prestamo.MaterialId);
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "Cedula", prestamo.PersonaId);
            return View(prestamo);
        }

        // GET: Prestamo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prestamo = await _context.Prestamos
                .Include(p => p.Material)
                .Include(p => p.Persona)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prestamo == null)
            {
                return NotFound();
            }

            return View(prestamo);
        }

        // POST: Prestamo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prestamo = await _context.Prestamos.FindAsync(id);
            if (prestamo != null)
            {
                _context.Prestamos.Remove(prestamo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrestamoExists(int id)
        {
            return _context.Prestamos.Any(e => e.Id == id);
        }
    }
}
