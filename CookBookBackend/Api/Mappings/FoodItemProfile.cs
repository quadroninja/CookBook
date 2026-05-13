using AutoMapper;
using CookBookBackend.Api.DTO.FoodItem;
using CookBookBackend.Api.Mappings.Resolvers;
using CookBookBackend.Core.Enums;
using CookBookBackend.Data.Models;

namespace CookBookBackend.Api.Mappings
{
    public class FoodItemProfile : Profile
    {
        public FoodItemProfile()
        {
            CreateMap<FoodItem, FoodItemEditResultDTO>()
                .ForMember(dest => dest.PhotoUrls,
                    opt => opt.MapFrom<FoodItemPhotoUrlResolver<FoodItemEditResultDTO>>());
            CreateMap<FoodItem, FoodItemCreateResultDTO>()
                .ForMember(dest => dest.PhotoUrls,
                    opt => opt.MapFrom<FoodItemPhotoUrlResolver<FoodItemCreateResultDTO>>());

            CreateMap<FoodItem, FoodItemPreviewDTO>()
                .ForMember(dest => dest.PhotoUrl,
                    opt => opt.MapFrom<FoodItemFirstPhotoUrlResolver<FoodItemPreviewDTO>>());
            CreateMap<FoodItem, FoodItemDetailedDTO>()
                .ForMember(dest => dest.PhotoUrls, 
                    opt => opt.MapFrom<FoodItemPhotoUrlResolver<FoodItemDetailedDTO>>());


            CreateMap<FoodItemCreateDTO, FoodItem>();
            CreateMap<FoodItemEditDTO, FoodItem>()
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
                        o => o.PreCondition(s => s.Category.HasValue && s.Category != FoodItemCategory.NONE))
                    .ForMember(d => d.ReadinessToEat,
                        o => o.PreCondition(s => s.ReadinessToEat.HasValue && s.ReadinessToEat != ReadinessToEat.NONE))
                    .ForMember(d => d.DietaryFlags,
                        o => o.PreCondition(s => s.DietaryFlags != DietaryFlags.NONE));
        }
    }
}
