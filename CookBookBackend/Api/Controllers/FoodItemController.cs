using CookBookBackend.Api.Controllers.ModelBinders;
using CookBookBackend.Api.DTO;
using CookBookBackend.Api.DTO.FoodItem;
using CookBookBackend.Api.DTO.Validator;
using CookBookBackend.Core.Enums;
using CookBookBackend.Core.Exceptions;
using CookBookBackend.Core.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookBookBackend.Api.Controllers
{
    [Route("food_items")]
    [ApiController]
    public class FoodItemController : ControllerBase
    {

        private readonly FoodItemService _service;
        private readonly ILogger<FoodItemController> _logger;

        public FoodItemController(FoodItemService service, ILogger<FoodItemController> logger)
        {
            this._service = service;
            this._logger = logger;
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateFoodItem([FromServices] IValidator<FoodItemCreateDTO> _validator, [FromForm] FoodItemCreateDTO dto)
        {
            var validationResult = _validator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Errors = validationResult.Errors.Select(x => new { x.PropertyName, x.ErrorMessage })
                });
            }

            return Ok(await _service.CreateFoodItemAsync(dto));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteFoodItem([FromRoute]int id)
        {
            try
            {
                await _service.DeleteFoodItemAsync(id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogWarning(ex, "FoodItem not found");
                return NotFound();
            }
            catch(FoodItemInUseException ex)
            {
                _logger.LogWarning(ex, "the FoodItem is used in a Dish");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetFoodItems(
            [FromQuery] string? searchBy = null,
            [FromQuery] string? sortBy = null,
            [FromQuery(Name = "desc")] bool? isSortDescending = false,
            [FromQuery] FoodItemCategory? category = null,
            [FromQuery][ModelBinder(BinderType = typeof(DietaryFlagsModelBinder))] DietaryFlags flags = DietaryFlags.NONE,
            [FromQuery(Name = "ready")] ReadinessToEat? readinessToEat = null)
        {
            var items = await _service.GetFoodItemsAsync(
                toSearch: searchBy,
                sortBy: sortBy,
                sortDescending: isSortDescending ?? false,
                category: category,
                flags: flags,
                readinessToEat: readinessToEat
                );

            return Ok(items);
        }

        [HttpPatch("edit/{id}")]
        public async Task<IActionResult> EditFoodItem([FromRoute] int id, [FromForm] FoodItemEditDTO editDto, IValidator<FoodItemEditDTO> _validator)
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
            
            return Ok(await _service.EditFoodItemAsync(id, editDto));
            
        }


    }
}
