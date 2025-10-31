using AutoMapper;

namespace EMPBACKEND.Extensions
{
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// Maps a collection to another type
        /// </summary>
        public static List<TDestination> MapToList<TDestination>(this IMapper mapper, IEnumerable<object> source)
        {
            return mapper.Map<List<TDestination>>(source);
        }

        /// <summary>
        /// Maps a single object to another type with null check
        /// </summary>
        public static TDestination? MapOrDefault<TDestination>(this IMapper mapper, object? source) where TDestination : class
        {
            return source != null ? mapper.Map<TDestination>(source) : null;
        }

        /// <summary>
        /// Maps source to existing destination object
        /// </summary>
        public static TDestination MapTo<TDestination>(this IMapper mapper, object source, TDestination destination)
        {
            return mapper.Map(source, destination);
        }
    }
}