using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace dotNetWebApiTranslation.Services.Localization;

public class JsonResourceLocalizer : IResourceLocalizer
{
  private readonly IDistributedCache _cache;

  public JsonResourceLocalizer(IDistributedCache cache)
  {
    _cache = cache;
  }
  public string this[string name, string? scope = null]
  {
    get
    {
      string? value = GetString(name, scope);
      if(value != null)
      {
        return value;
      }
      else
      {
        var scopeKey = scope != null ? scope + "-" : string.Empty;
        return $"{scopeKey}{Thread.CurrentThread.CurrentCulture.Name}-{name}";
      }
    }
  }
  private string? GetString(string key, string? scope = null)
  {
    LoadLocale(scope);
    string cacheKey = GenerateCacheKey(scope, key);
    string? cacheValue = _cache.GetString(cacheKey);
    if (!string.IsNullOrEmpty(cacheValue))
    {
      return cacheValue;
    }

    string filePath = GenerateFilePath(scope);
    if (File.Exists(filePath))
    {
      string? result = GetValueFromJson(key, filePath);
      if (!string.IsNullOrEmpty(result)) _cache.SetString(cacheKey, result);
      return result;
    }
    return null;
  }
  private string? GetValueFromJson(string key, string filePath)
  {
    string? result = null;
    if (!string.IsNullOrEmpty(key)  && !string.IsNullOrEmpty(filePath))
    {
      using (var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
      using (var sReader = new StreamReader(str))
      using (var reader = new JsonTextReader(sReader))
      {
        while (reader.Read())
        {
          if (reader.TokenType == JsonToken.PropertyName && (string)reader.Value == key)
          {
            reader.Read();
            result = reader.Value?.ToString() ?? key;
            break;
          }
        }
      }
    }
    return result;
  }
  private void LoadLocale(string? scope = null)
  {
    string cacheKey = GenerateCacheKey(scope);
    string? cacheValue = _cache.GetString(cacheKey);
    if (string.IsNullOrEmpty(cacheValue))
    {
      string filePath = GenerateFilePath(scope);
      if (File.Exists(filePath))
      {
        using (var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        using (var sReader = new StreamReader(str))
        using (var reader = new JsonTextReader(sReader))
        {
          while (reader.Read())
          {
            if (reader.TokenType != JsonToken.PropertyName)
              continue;
            string key = reader.Value.ToString();
            reader.Read();
            string value = reader.Value.ToString();;
            _cache.SetString(GenerateCacheKey(scope, key), value);
          }
          _cache.SetString(cacheKey, "loaded");
        }
      }
    }
  }
  private string GenerateCacheKey(string? scope, string? key = null)
  {
    string scopeKey = scope != null ? scope + "-" : string.Empty;
    return $"locale-{scopeKey}{Thread.CurrentThread.CurrentCulture.Name}-{key}";
  }
  private string GenerateFilePath(string? scope)
  {
    string scopePath = scope != null ? scope + "/" : string.Empty;
    return $"wwwroot/Localization/{scopePath}{Thread.CurrentThread.CurrentCulture.Name}.json";
  }
}
