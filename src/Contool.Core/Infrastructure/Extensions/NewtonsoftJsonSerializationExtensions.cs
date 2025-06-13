using Contentful.Core.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Contool.Core.Infrastructure.Extensions;

public static class NewtonsoftJsonSerializationExtensions
{
    private static readonly JsonSerializerSettings DefaultSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.None,
        MissingMemberHandling = MissingMemberHandling.Ignore,
        Converters = [new ContentJsonConverter()],
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
    };

    public static string SerializeToJsonString(this object obj, JsonSerializerSettings? settings = null)
        => JsonConvert.SerializeObject(obj, settings ?? DefaultSettings);

    public static T? DeserializeFromJsonString<T>(this string jsonString, JsonSerializerSettings? settings = null)
        => JsonConvert.DeserializeObject<T>(jsonString, settings ?? DefaultSettings);
}