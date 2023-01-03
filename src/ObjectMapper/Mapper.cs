namespace ObjectMapper
{
    using Abstractions;
    using Helpers;

    public class Mapper : IMapper
    {
        private readonly MappingService _mappingService = MappingService.Create();

        public Mapper()
        {
        }

        public void MapTo(object source, object target)
        {
            Checker.NullChecks(source, target);

            Map(source, target);
        }

        public void MapFrom(object source, object target)
        {
            Checker.NullChecks(source, target);

            Map(target, source);
        }

        public void Map(object source, object target)
        {
            Checker.NullChecks(source, target);

            _mappingService.ApplyDiffs(source, target);
        }

        public Mapper Create() => new();
    }
}