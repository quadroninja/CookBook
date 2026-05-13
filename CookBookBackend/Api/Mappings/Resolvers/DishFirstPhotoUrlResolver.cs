using AutoMapper;
using CookBookBackend.Data.Models;

namespace CookBookBackend.Api.Mappings.Resolvers
{
    public class DishFirstPhotoUrlResolver<T> : IValueResolver<Dish, T, string?> where T : class
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DishFirstPhotoUrlResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? Resolve(Dish source, T destination, string? destMember, ResolutionContext context)
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
