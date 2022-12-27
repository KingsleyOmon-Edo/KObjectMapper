﻿using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ObjectMapper
{
    public static class MapperExtensions
    {
        public static void MapFrom<T>(this T source, T target)
        {
            var mapper = new Mapper();
            mapper.Map(target, source);
        }

        public static void MapFrom(this object source, object target)
        {
            var mapper = new Mapper();
            mapper.MapFrom(source, target);
        }

        public static void MapTo<T>(this T source, T target)
        {
            var mapper = new Mapper();
            mapper.Map(source, target);
        }

        public static void MapTo(this object source, object target)
        {
            var mapper = new Mapper();
            mapper.Map(source, target);
        }
    }
}