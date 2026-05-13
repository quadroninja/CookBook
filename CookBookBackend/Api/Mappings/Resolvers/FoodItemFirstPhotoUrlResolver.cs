using AutoMapper;
using CookBookBackend.Data.Models;

namespace CookBookBackend.Api.Mappings.Resolvers
{
    public class FoodItemFirstPhotoUrlResolver<T> : IValueResolver<FoodItem, T, string?> where T : class
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FoodItemFirstPhotoUrlResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? Resolve(FoodItem source, T destination, string? destMember, ResolutionContext context)
        {
            if (source.PhotoPaths == null || !source.PhotoPaths.Any())
                return null;

            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
                return null;

            var baseUrl = $"{request.Scheme}://{request.Host}";
            var firstPath = source.PhotoPaths.First().Replace('\\', '/');

            // Ensure leading slash
            var normalizedPath = firstPath.StartsWith('/') ? firstPath : $"/{firstPath}";

            return $"{baseUrl}{normalizedPath}";
        }
    }

}
