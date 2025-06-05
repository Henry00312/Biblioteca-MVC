using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Biblioteca.Application.Interfaces;
using Biblioteca.Domain.Entities;

namespace Biblioteca.API.Controllers
{
    public class PersonaController : Controller
    {
        private readonly IPersonaService _personaService;

        public PersonaController(IPersonaService personaService)
        {
            _personaService = personaService;
        }

        public async Task<IActionResult> Index()
        {
            var personas = await _personaService.GetAllAsync();
            return View(personas);
        }

        public IActionResult Create()
        {
            CargarRoles(); // ✅ cargar roles para el formulario
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Persona persona)
        {
            // ✅ Validaciones del lado del controlador
            if (string.IsNullOrWhiteSpace(persona.Nombre))
                ModelState.AddModelError("Nombre", "El nombre es obligatorio.");

            if (!new[] { "Estudiante", "Profesor", "Administrativo" }.Contains(persona.Rol))
                ModelState.AddModelError("Rol", "El rol debe ser Estudiante, Profesor o Administrativo.");

            if (!ModelState.IsValid)
            {
                CargarRoles(persona.Rol); // ⚠️ recargar roles si hay error
                return View(persona);
            }

            await _personaService.AddAsync(persona);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var persona = await _personaService.GetByIdAsync(id);
            if (persona == null) return NotFound();

            CargarRoles(persona.Rol);
            return View(persona);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Persona persona)
        {
            if (!ModelState.IsValid)
            {
                CargarRoles(persona.Rol);
                return View(persona);
            }

            await _personaService.UpdateAsync(persona);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var persona = await _personaService.GetByIdAsync(id);
            return persona == null ? NotFound() : View(persona);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var persona = await _personaService.GetByIdAsync(id);
            return persona == null ? NotFound() : View(persona);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _personaService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ✅ Método privado para cargar roles en la ViewBag
        private void CargarRoles(string? rolSeleccionado = null)
        {
            ViewBag.Roles = new SelectList(new[]
            {
                new SelectListItem { Text = "Estudiante", Value = "Estudiante" },
                new SelectListItem { Text = "Profesor", Value = "Profesor" },
                new SelectListItem { Text = "Administrativo", Value = "Administrativo" }
            }, "Value", "Text", rolSeleccionado);
        }
    }
}
