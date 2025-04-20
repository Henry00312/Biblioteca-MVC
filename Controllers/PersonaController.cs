using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca_MVC.Models;
using Biblioteca_MVC.Helpers;

namespace Biblioteca_MVC.Controllers
{
    public class PersonaController : Controller
    {
        private readonly BibliotecaContext _context;

        public PersonaController(BibliotecaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Personas.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var persona = await _context.Personas.FirstOrDefaultAsync(m => m.Id == id);
            return persona == null ? NotFound() : View(persona);
        }

        public IActionResult Create()
        {
            ViewData["Roles"] = new SelectList(new[] { "Estudiante", "Profesor", "Administrativo" });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Cedula,Rol")] Persona persona)
        {
            if (_context.Personas.Any(p => p.Cedula == persona.Cedula))
            {
                ModelState.AddModelError("Cedula", "Ya existe una persona registrada con esta cédula.");
            }

            if (!new[] { "Estudiante", "Profesor", "Administrativo" }.Contains(persona.Rol))
            {
                ModelState.AddModelError("Rol", "El rol debe ser Estudiante, Profesor o Administrativo.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(persona);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Roles"] = new SelectList(new[] { "Estudiante", "Profesor", "Administrativo" }, persona.Rol);
            return View(persona);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var persona = await _context.Personas.FindAsync(id);
            if (persona == null)
                return NotFound();

            ViewData["Roles"] = new SelectList(new[] { "Estudiante", "Profesor", "Administrativo" }, persona.Rol);
            return View(persona);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Cedula,Rol")] Persona persona)
        {
            if (id != persona.Id)
                return NotFound();

            if (!new[] { "Estudiante", "Profesor", "Administrativo" }.Contains(persona.Rol))
            {
                ModelState.AddModelError("Rol", "El rol debe ser Estudiante, Profesor o Administrativo.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(persona);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonaExists(persona.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["Roles"] = new SelectList(new[] { "Estudiante", "Profesor", "Administrativo" }, persona.Rol);
            return View(persona);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var persona = await _context.Personas.FirstOrDefaultAsync(m => m.Id == id);
            return persona == null ? NotFound() : View(persona);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var persona = await _context.Personas
                .Include(p => p.Prestamos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (persona == null)
                return NotFound();

            bool tienePrestamosActivos = persona.Prestamos.Any(p => p.Tipo == TiposMovimiento.Prestamo);
            if (tienePrestamosActivos)
            {
                ModelState.AddModelError("", "No se puede eliminar la persona porque tiene préstamos activos.");
                return View(persona);
            }

            _context.Personas.Remove(persona);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonaExists(int id)
        {
            return _context.Personas.Any(e => e.Id == id);
        }
    }
}
