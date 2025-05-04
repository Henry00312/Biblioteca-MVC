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
            var prestamos = await _context.Prestamos
                .Include(p => p.Material)
                .Include(p => p.Persona)
                .OrderByDescending(p => p.Fecha) // Ordenar por la fecha de manera descendente
                .ToListAsync();

            return View(prestamos);
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
        // GET: Prestamo/Create
        public IActionResult Create()
        {
            ViewData["MaterialId"] = new SelectList(_context.Materiales.Where(m => m.Activo), "Id", "Titulo");
            ViewData["PersonaId"] = new SelectList(_context.Personas.Where(p => p.Activo), "Id", "Cedula");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaterialId,PersonaId")] Prestamo prestamo)
        {
            if (ModelState.IsValid)
            {
                var material = await _context.Materiales.FindAsync(prestamo.MaterialId);
                var persona = await _context.Personas.FindAsync(prestamo.PersonaId);

                if (material == null || !material.Activo)
                {
                    ModelState.AddModelError("MaterialId", "Material no válido o inactivo.");
                }

                if (persona == null || !persona.Activo)
                {
                    ModelState.AddModelError("PersonaId", "Persona no válida o inactiva.");
                }

                // Verificar si la persona ha alcanzado el límite de préstamos activos según su rol
                if (ModelState.IsValid)
                {
                    var prestamosActivos = await _context.Prestamos
                        .Where(p => p.PersonaId == prestamo.PersonaId && p.Tipo == "Prestamo")
                        .CountAsync();

                    int limite = persona.Rol.ToLower() switch
                    {
                        "estudiante" => 5, // Estudiantes tienen un límite de 5
                        "profesor" => 3, // Profesores tienen un límite de 3
                        "administrativo" => 1, // Administrativos tienen un límite de 1
                        _ => 0
                    };

                    if (prestamosActivos >= limite)
                    {
                        ModelState.AddModelError("", $"El {persona.Rol} ha alcanzado su límite de préstamos ({limite}).");
                    }
                }

                // Validar si el material está disponible
                if (material != null && material.CantidadActual <= 0)
                {
                    ModelState.AddModelError("", "Este material no está disponible para préstamo.");
                }

                // Si todo es válido, proceder con el préstamo
                if (ModelState.IsValid)
                {
                    prestamo.Fecha = DateTime.Now;
                    prestamo.Tipo = "Prestamo"; // Se asigna en el backend

                    _context.Add(prestamo);

                    // Reducir la cantidad disponible del material
                    material.CantidadActual -= 1;
                    _context.Update(material);

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index)); // Redirige al listado de préstamos
                }
            }

            ViewData["MaterialId"] = new SelectList(_context.Materiales.Where(m => m.Activo), "Id", "Titulo", prestamo.MaterialId);
            ViewData["PersonaId"] = new SelectList(_context.Personas.Where(p => p.Activo), "Id", "Cedula", prestamo.PersonaId);
            return View(prestamo);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var prestamo = await _context.Prestamos.FindAsync(id);
            if (prestamo == null)
                return NotFound();

            LoadSelectLists(prestamo.MaterialId, prestamo.PersonaId);
            return View(prestamo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tipo,Fecha,MaterialId,PersonaId")] Prestamo prestamo)
        {
            if (id != prestamo.Id)
                return NotFound();

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
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            LoadSelectLists(prestamo.MaterialId, prestamo.PersonaId);
            return View(prestamo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Devolver(int id)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.Material)
                .Include(p => p.Persona)
                .FirstOrDefaultAsync(p => p.Id == id && p.Tipo == "Prestamo");

            if (prestamo == null)
                return NotFound();

            prestamo.Tipo = "Devolucion";
            prestamo.FechaDevolucion = DateTime.Now; // Asignamos la fecha de devolución

            if (prestamo.Material != null)
            {
                prestamo.Material.CantidadActual += 1; // Aumentamos la cantidad del material
                _context.Update(prestamo.Material);
            }

            _context.Update(prestamo); // Actualizamos el préstamo
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Redirigimos al listado de préstamos
        }

        private void LoadSelectLists(int? selectedMaterialId = null, int? selectedPersonaId = null)
        {
            ViewData["MaterialId"] = new SelectList(_context.Materiales.Where(m => m.Activo), "Id", "Titulo", selectedMaterialId);
            ViewData["PersonaId"] = new SelectList(_context.Personas.Where(p => p.Activo), "Id", "Cedula", selectedPersonaId);
        }

        private bool PrestamoExists(int id)
        {
            return _context.Prestamos.Any(e => e.Id == id);
        }
    }
}
