using AutoMapper;
using CookBookBackend.Api.DTO;
using CookBookBackend.Data.Models;

namespace CookBookBackend.Api.Mappings.Resolvers
{
    public class FoodItemPhotoUrlResolver<T> : IValueResolver<FoodItem, T, List<string>> where T : class
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FoodItemPhotoUrlResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<string> Resolve(FoodItem source, T destination, List<string> destMember, ResolutionContext context)
        {
            if (source.PhotoPaths == null) return new List<string>();

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            return source.PhotoPaths.Select(path => $"{baseUrl}/{path}").ToList();
        }

    }
}
