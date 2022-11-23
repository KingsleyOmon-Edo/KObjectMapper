using Newtonsoft.Json;

namespace Extensions
{
    public static class HttpResponseExtensions
    {
        public static async Task<T?> GetData<T>(this HttpResponseMessage response)
        {
            var stringResult = await response.Content.ReadAsStringAsync();
            var dataResult = JsonConvert.DeserializeObject<T>(stringResult);

            return dataResult;
        }
    }
}