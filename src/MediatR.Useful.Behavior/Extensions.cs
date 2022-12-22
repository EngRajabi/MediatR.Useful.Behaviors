using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace MediatR.Useful.Behavior;
public static class Extensions
{
    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter()
        },
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    //public static async Task<TEntity> GetEntityAsync<TEntity>(this IDistributedCache distributedCache, string key, CancellationToken cancellationToken = default)
    //{
    //    var itemBytes = await distributedCache.GetAsync(key, cancellationToken).ConfigureAwait(false);
    //    if (itemBytes == null)
    //        return default;

    //    var itemString = Encoding.UTF8.GetString(itemBytes);
    //    return itemString.ToObject<TEntity>();
    //}


    public static T ToObject<T>(this string source)
    {
        return JsonSerializer.Deserialize<T>(source, JsonSerializerOptions);
    }

    public static string ToJson(this object source)
    {
        return JsonSerializer.Serialize(source, JsonSerializerOptions);
    }

    public static byte[] ToJsonUtf8Bytes(this object source)
    {
        return JsonSerializer.SerializeToUtf8Bytes(source, JsonSerializerOptions);
    }
}
