using Microsoft.AspNetCore.Mvc;
using Biblioteca.Application.Interfaces;
using Biblioteca.Domain.Entities;

namespace Biblioteca.API.Controllers
{
    public class MaterialController : Controller
    {
        private readonly IMaterialService _materialService;

        public MaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        public async Task<IActionResult> Index()
        {
            var materiales = await _materialService.GetAllAsync();
            return View(materiales);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Material material)
        {
            if (string.IsNullOrWhiteSpace(material.Titulo))
            {
                ModelState.AddModelError("Titulo", "El título es obligatorio.");
            }

            if (!ModelState.IsValid)
                return View(material);

            await _materialService.AddAsync(material);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var material = await _materialService.GetByIdAsync(id);
            if (material == null) return NotFound();
            return View(material);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Material material)
        {
            if (!ModelState.IsValid)
                return View(material);

            await _materialService.UpdateAsync(material);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            var material = await _materialService.GetByIdAsync(id);
            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var material = await _materialService.GetByIdAsync(id);
            if (material == null) return NotFound();
            return View(material);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _materialService.DeleteAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Delete), new { id });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
