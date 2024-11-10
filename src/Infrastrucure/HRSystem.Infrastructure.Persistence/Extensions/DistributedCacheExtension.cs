using HRSystem.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HRSystem.Infrastructure.Persistence.Extensions;

public static class DistributedCacheExtension
{
    public static async Task<T?> GetOrCreateAsync<T>(
        this IDistributedCache cache,
        string cacheKey,
        ILogger logger,
        DistributedCacheEntryOptions cacheOptions,
        Func<Task<T?>> fetchFromSource,
        CancellationToken cancellationToken = default) where T : class
    {
        // Try to get the result from Redis cache
        var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
            return JsonSerializer.Deserialize<T>(cachedData);  // Deserialize cached data
        }

        // Cache miss: Fetch data from the source
        logger.LogWarning("Cache miss. Fetching data for key: {CacheKey}", cacheKey);
        var result = await fetchFromSource();

        if (result != null)
        {
            // Cache the result in Redis
            var serializedData = JsonSerializer.Serialize(result);
            await cache.SetStringAsync(cacheKey, serializedData, cacheOptions, cancellationToken);
        }

        return result;
    }


    public static  T? GetOrCreate<T>(
    this IDistributedCache cache,
    string cacheKey,
    ILogger logger,
    DistributedCacheEntryOptions cacheOptions,
    Func<T?> fetchFromSource,
    CancellationToken cancellationToken = default) where T : class
    {
        // Try to get the result from Redis cache
        var cachedData =  cache.GetStringAsync(cacheKey, cancellationToken).Result;
        if (!string.IsNullOrEmpty(cachedData))
        {
            logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
            return JsonSerializer.Deserialize<T>(cachedData);  // Deserialize cached data
        }

        // Cache miss: Fetch data from the source
        logger.LogWarning("Cache miss. Fetching data for key: {CacheKey}", cacheKey);
        var result =  fetchFromSource();

        if (result != null)
        {
            // Cache the result in Redis
            var serializedData = JsonSerializer.Serialize(result);
            cache.SetStringAsync(cacheKey, serializedData, cacheOptions, cancellationToken);
        }

        return result;
    }


    public static async Task<TResult?> GetOrCreateAsyncWithResultType<T, TResult>(
    this IDistributedCache cache,
    string cacheKey,
    ILogger logger,
    DistributedCacheEntryOptions cacheOptions,
    Func<Task<TResult?>> fetchFromSource,
    CancellationToken cancellationToken = default) where TResult : class
{
    try
    {
        // Try to get the result from Redis cache
        var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
            var deserializedData = JsonSerializer.Deserialize<TResult>(cachedData);
            if (deserializedData != null)
            {
                return deserializedData;  // Return cached data if deserialization is successful
            }
            else
            {
                logger.LogWarning("Failed to deserialize cached data for key: {CacheKey}", cacheKey);
            }
        }

        // Cache miss: Fetch data from the source
        logger.LogWarning("Cache miss. Fetching data for key: {CacheKey}", cacheKey);
        var result = await fetchFromSource();

        if (result != null)
        {
            // Cache the result in Redis
            var serializedData = JsonSerializer.Serialize(result);
            await cache.SetStringAsync(cacheKey, serializedData, cacheOptions, cancellationToken);
        }

        return result;
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while processing cache for key: {CacheKey}", cacheKey);
        return null;  // Return null or handle as needed
    }
}
}