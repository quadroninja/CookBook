using AutoMapper;
using CookBookBackend.Api.DTO.Dish;
using CookBookBackend.Api.DTO.DishIngredient;
using CookBookBackend.Api.DTO.FoodItem;
using CookBookBackend.Api.Mappings.Resolvers;
using CookBookBackend.Core.Enums;
using CookBookBackend.Data.Models;

namespace CookBookBackend.Api.Mappings
{
    public class DishProfile : Profile
    {
        public DishProfile()
        {

            CreateMap<DishIngredient, DishIngredientCreateDTO>()
                .ForMember(dest => dest.DishId, opt => opt.MapFrom(src => src.DishId))
                .ForMember(dest => dest.FoodItemId, opt => opt.MapFrom(src => src.FoodItemId))
                .ForMember(dest => dest.AmountGrams, opt => opt.MapFrom(src => src.AmountGrams))
                .ReverseMap();
            CreateMap<DishIngredient, DishIngredientDetailedDTO>()
                .ForMember(dest => dest.FoodItemId, opt => opt.MapFrom(src => src.FoodItemId))
                .ForMember(dest => dest.FoodItemName, opt => opt.MapFrom(src => src.FoodItem.Name))
                .ForMember(dest => dest.AmountGrams, opt => opt.MapFrom(src => src.AmountGrams));


            CreateMap<Dish, DishCreateResultDTO>()
                .ForMember(dest => dest.PhotoUrls,
                    opt => opt.MapFrom<DishPhotoUrlResolver<DishCreateResultDTO>>());
            CreateMap<Dish, DishEditResultDTO>()
                .ForMember(dest => dest.PhotoUrls,
                    opt => opt.MapFrom<DishPhotoUrlResolver<DishEditResultDTO>>());

            CreateMap<DishCreateDTO, Dish>()
                .ForMember(dest => dest.Ingredients, opt => opt.Ignore());
            CreateMap<DishEditDTO, Dish>()
                .ForMember(dest => dest.Ingredients, opt => opt.UseDestinationValue());

            CreateMap<Dish, DishDetailedDTO>()
                .ForMember(dest => dest.PhotoUrls, opt => opt.MapFrom<DishPhotoUrlResolver<DishDetailedDTO>>())
                .ForMember(dest => dest.Ingredients, opt => opt.Ignore());
            CreateMap<Dish, DishPreviewDTO>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom<DishFirstPhotoUrlResolver<DishPreviewDTO>>());


            CreateMap<DishEditDTO, Dish>()
                    .ForMember(d => d.Name,
                        o => o.PreCondition(s => s.Name != null))
                    .ForMember(d => d.Calories,
                        o => o.PreCondition(s => s.Calories.HasValue))
                    .ForMember(d => d.Proteins,
                        o => o.PreCondition(s => s.Proteins.HasValue))
                    .ForMember(d => d.Fats,
                        o => o.PreCondition(s => s.Fats.HasValue))
                    .ForMember(d => d.Carbohydrates,
                        o => o.PreCondition(s => s.Carbohydrates.HasValue))
                    .ForMember(d => d.Category,
                        o => o.PreCondition(s => s.Category.HasValue && s.Category != DishCategory.NONE))
                    .ForMember(d => d.DietaryFlags,
                        o => o.PreCondition(s => s.DietaryFlags != null))
                    .ForMember(d => d.Ingredients,
                        o => { o.PreCondition(s => s.Ingredients != null); o.Ignore(); });
        }
    }
}
