using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ObjectMapper
{
    public static class MapperExtensions
    {
        public static void MapFrom<T>(this T source, T target)
        {
            var mapper = Mapper.Create();
            mapper.Map(target, source);
        }

        public static void MapFrom(this object source, object target)
        {
            var mapper = Mapper.Create();
            mapper.MapFrom(source, target);
        }

        public static void MapTo<T>(this T source, T target)
        {
            var mapper = Mapper.Create();
            mapper.Map(source, target);
        }

        public static void MapTo(this object source, object target)
        {
            var mapper = Mapper.Create();
            mapper.Map(source, target);
        }
    }
}
