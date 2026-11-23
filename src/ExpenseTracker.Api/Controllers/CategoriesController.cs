using ExpenseTracker.Application.DTOs.Categories;
using ExpenseTracker.Application.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var categories = await _categoryService.GetAllAsync(
                cancellationToken);

            return Ok(categories);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var category = await _categoryService.GetByIdAsync(
                id,
                cancellationToken);

            if (category is null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
               CreateCategoryDto dto,
               CancellationToken cancellationToken)
        {
            var category = await _categoryService.CreateAsync(
                dto,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = category.Id },
                category);
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
               int id,
               UpdateCategoryDto dto,
               CancellationToken cancellationToken)
        {
            var category = await _categoryService.UpdateAsync(
                id,
                dto,
                cancellationToken);

            if (category is null)
                return NotFound();

            return Ok(category);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken)
        {
            var deleted = await _categoryService.DeleteAsync(
                id,
                cancellationToken);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
