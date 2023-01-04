﻿namespace ObjectMapper.Extensions
{
    using Helpers;

    public static class MapperExtensions
    {
        public static void MapTo(this object source, object target)
        {
            Checker.NullCheckAll(source, target);

            var mapper = new Mapper();
            mapper.MapTo(source, target);
        }

        public static void MapFrom(this object source, object target)
        {
            Checker.NullCheckAll(source, target);

            var mapper = new Mapper();
            mapper.MapFrom(source, target);
        }

        public static void MapTo<TTarget>(this object source, TTarget target)
        {
            //var mapper = Mapper.Create();
            //mapper.MapTo(source, target);
            ////mapper.Map(source, target);


            // Null check both
            Checker.CoalescedNullCheck(source);
            Checker.CoalescedNullCheck(target);

            //  Type check TTarget only
            Checker.TypeCheck(target);

            var svc = MappingService.Create();
            svc.ApplyDiffs(source, target);
        }

        public static void MapFrom<TSource>(this object target, TSource source)
        {
            //  Null check both
            Checker.CoalescedNullCheck(source);
            Checker.CoalescedNullCheck(target);

            //  Type check only the source object
            Checker.TypeCheck(source);

            var svc = MappingService.Create();
            svc.ApplyDiffs(source, target);
        }
    }
}