using System.Text.Json;

namespace WebApplication1
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
       => session.SetString(key, JsonSerializer.Serialize(value));

        public static T? GetObject<T>(this ISession session, string key)
            => session.GetString(key) is string s ? JsonSerializer.Deserialize<T>(s) : default;
    }
}
