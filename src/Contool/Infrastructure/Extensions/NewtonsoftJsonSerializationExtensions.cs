using Newtonsoft.Json;

namespace Contool.Infrastructure.Extensions;

internal static class NewtonsoftJsonSerializationExtensions
{
    private static readonly JsonSerializerSettings DefaultSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.None,
        MissingMemberHandling = MissingMemberHandling.Ignore
    };

    public static string SerializeToJsonString(this object obj, JsonSerializerSettings? settings = null)
        => JsonConvert.SerializeObject(obj, settings ?? DefaultSettings);

    public static T? DeserializeFromJsonString<T>(this string jsonString, JsonSerializerSettings? settings = null)
        => JsonConvert.DeserializeObject<T>(jsonString, settings ?? DefaultSettings);
}