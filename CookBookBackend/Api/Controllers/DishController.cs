using CookBookBackend.Api.Controllers.ModelBinders;
using CookBookBackend.Api.DTO.Dish;
using CookBookBackend.Api.DTO.FoodItem;
using CookBookBackend.Core.Enums;
using CookBookBackend.Core.Exceptions;
using CookBookBackend.Core.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CookBookBackend.Api.Controllers
{
    [ApiController]
    [Route("dishes")]
    public class DishController : ControllerBase
    {
        private readonly DishService _service;
        private readonly ILogger<DishController> _logger;

        public DishController(DishService service, ILogger<DishController> logger)
        {
            this._service = service;
            this._logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateDish(IValidator<DishCreateDTO> _validator, [FromForm] DishCreateDTO dto)
        {
            var validationResult = _validator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            return Ok(await _service.CreateDishAsync(dto));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteDish([FromRoute] int id)
        {
            try
            {
                await _service.DeleteDishAsync(id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogWarning(ex, "Delete Dish failed");
                return NotFound();
            }
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetFoodItems(
            [FromQuery] string? searchBy = null,
            [FromQuery] DishCategory? category = null,
            [FromQuery][ModelBinder(BinderType = typeof(DietaryFlagsModelBinder))] DietaryFlags flags = DietaryFlags.NONE)
        {
            var items = await _service.GetFoodItemsAsync(
                toSearch: searchBy,
                category: category,
                flags: flags
                );

            return Ok(items);
        }

        [HttpPatch("edit/{id}")]
        public async Task<IActionResult> EditDish([FromRoute] int id, [FromForm] DishEditDTO editDto, IValidator<DishEditDTO> _validator)
        {
            var validationResult = _validator.Validate(editDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Errors = validationResult.Errors.Select(x => new { x.PropertyName, x.ErrorMessage })
                });
            }

            return Ok(await _service.EditDishAsync(id, editDto));

        }


    }
}
