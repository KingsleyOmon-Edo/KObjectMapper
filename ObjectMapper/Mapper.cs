namespace ObjectMapper
{
    using Extensions;
    public class Mapper
    {
        public Mapper()
        {
        }

        public void Map<T>(T source, T target)

        {
            source.ApplyDiffs<T>(target);

        }
    }
}
