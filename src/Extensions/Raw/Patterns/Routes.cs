namespace Extensions.Raw.Patterns
{
    public class Routes
    {
        private const string BaseApiRoute = "api";

        public static class Contacts
        {
            private const string BaseContactRoute = $"{BaseApiRoute}/contacts";
            public static string? Get() => BaseContactRoute;
            public static string? GetById(int id) => $"{BaseContactRoute}/{id}";

            public static string? GetByPage(int pageNumber, int pageSize) =>
                $"{BaseContactRoute}/?pageNumber={pageNumber}&pageSize={pageSize}";

            public static string? Post() => BaseContactRoute;
            public static string? Delete(int id) => $"{BaseContactRoute}/{id}";

            public static string? Put(int id) => $"{BaseContactRoute}/{id}";

            public static string? Patch(int id) => $"{BaseContactRoute}/{id}";
        }
    }
}