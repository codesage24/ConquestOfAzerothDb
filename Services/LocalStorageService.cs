using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace ConquestOfAzerothDb.Services;

public interface ILocalStorageService
{
    Task SetItemAsync<T>(string key, T item);

    Task<T> GetItemAsync<T>(string key);

    Task<T> GetItemSetDefaultAsync<T>(string key, T defaultValue);

    Task RemoveItemAsync(string key);

    Task ClearAsync();
}

public class LocalStorageService : ILocalStorageService
{
    private readonly IJSRuntime _js;
    private readonly JsonSerializerOptions _jsonOptions;

    public LocalStorageService(IJSRuntime js)
    {
        _js = js;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task SetItemAsync<T>(string key, T item)
    {
        var json = JsonSerializer.Serialize(item, _jsonOptions);

        await _js.InvokeVoidAsync("localStorage.setItem", key, json);
    }

    public async Task<T> GetItemAsync<T>(string key)
    {
        var json = await _js.InvokeAsync<string>("localStorage.getItem", key);

        if (json is null)
        {
            return default!;
        }

        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    public async Task<T> GetItemSetDefaultAsync<T>(string key, T defaultValue)
    {
        var json = await _js.InvokeAsync<string>("localStorage.getItem", key);

        if (json is null)
        {
            await SetItemAsync(key, defaultValue);

            return defaultValue;
        }

        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }


    public async Task RemoveItemAsync(string key)
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", key);
    }

    public async Task ClearAsync()
    {
        await _js.InvokeVoidAsync("localStorage.clear");
    }
}
