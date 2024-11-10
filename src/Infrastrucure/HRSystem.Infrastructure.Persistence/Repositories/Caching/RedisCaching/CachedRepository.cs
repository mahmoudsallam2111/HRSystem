using Ardalis.Specification;
using HRSystem.Application.Interfaces;
using HRSystem.Infrastructure.Persistence.Extensions;
using HRSystem.Infrastructure.Persistence.Repositories.CachingRepos;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace HRSystem.Infrastructure.Persistence.Repositories.Caching.RedisCaching
{
    public class CachedRepository<T> : IReadRepository<T> where T : class
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<CachedRepository<T>> _logger;
        private readonly MyRepository<T> _sourceRepository;
        private DistributedCacheEntryOptions _cacheOptions;
        private readonly CacheSettings _cacheSettings;

        public CachedRepository(IDistributedCache distributedCache,
            ILogger<CachedRepository<T>> logger,
            MyRepository<T> sourceRepository,
            IOptions<CacheSettings> cacheSettings)
        {
            _distributedCache = distributedCache;
            _logger = logger;
            _sourceRepository = sourceRepository;
            _cacheSettings = cacheSettings.Value;
            _cacheOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(_cacheSettings.CacheTimeInSeconds));   // TODO we can use SlidingExpiration technique
        }

        public async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            if (!specification.CacheEnabled)
            {
                return await _sourceRepository.AnyAsync(specification, cancellationToken);
            }

            string cacheKey = $"{specification.CacheKey}-AnyAsync";
            _logger.LogInformation("Checking cache for key: {CacheKey}", cacheKey);

            // Try to get the result from Redis cache
            string? cachedData = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                return JsonSerializer.Deserialize<bool>(cachedData);  // Deserialize the cached data
            }

            // Cache miss: Fetch data from the source repository
            _logger.LogWarning("Cache miss. Fetching data for key: {CacheKey}", cacheKey);
            var result = await _sourceRepository.AnyAsync(specification, cancellationToken);

            // Cache the result in Redis
            await _distributedCache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(result),  // Serialize the result
                _cacheOptions,
                cancellationToken);

            return result;
        }

        public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            return _sourceRepository.AnyAsync(cancellationToken);
        }

        public IAsyncEnumerable<T> AsAsyncEnumerable(ISpecification<T> specification)
        {
            if (specification.CacheEnabled)
            {
                string cacheKey = $"{specification.CacheKey}-AsAsyncEnumerable";
                _logger.LogInformation("Checking cache for key: {CacheKey}", cacheKey);

                var result = _distributedCache.GetOrCreate(
                    cacheKey,
                    _logger,
                   _cacheOptions,
                     async () =>await _sourceRepository.ListAsync(specification));

                 return (IAsyncEnumerable<T>)result;
            }

            // If no caching is enabled, directly return the data from the repository as an async enumerable
            return (IAsyncEnumerable<T>)_sourceRepository.ListAsync(specification).Result.AsEnumerable();
        }

        /// <inheritdoc/>
        public Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            // TODO: Add Caching
            return _sourceRepository.CountAsync(specification, cancellationToken);
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return _sourceRepository.CountAsync(cancellationToken);
        }

        public async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            if (specification.CacheEnabled)
            {
                string cacheKey = $"{specification.CacheKey}-FirstOrDefaultAsync";
                _logger.LogInformation("Checking cache for key: {CacheKey}", cacheKey);

                var result = await _distributedCache.GetOrCreateAsync(
                    cacheKey,
                    _logger,
                    _cacheOptions,
                    async () =>await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken),
                    cancellationToken);

                return result;
            }

            // If caching is not enabled, fetch from the source directly
            return await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);

            #region old approach without extension method
            //if (specification.CacheEnabled)
            //{
            //    string cacheKey = $"{specification.CacheKey}-FirstOrDefaultAsync";
            //    _logger.LogInformation("Checking cache for key: {CacheKey}", cacheKey);

            //    // Try to get the result from Redis cache
            //    var cachedData = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
            //    if (!string.IsNullOrEmpty(cachedData))
            //    {
            //        _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
            //        return JsonSerializer.Deserialize<T>(cachedData); // Deserialize the cached data
            //    }

            //    // Cache miss: Fetch data from the source repository
            //    _logger.LogWarning("Cache miss. Fetching data for key: {CacheKey}", cacheKey);
            //    var result = await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);

            //    if (result != null)
            //    {
            //        // Cache the result in Redis
            //        var serializedData = JsonSerializer.Serialize(result);
            //        await _distributedCache.SetStringAsync(cacheKey, serializedData, _cacheOptions, cancellationToken);
            //    }

            //    return result;
            //}

            //// If caching is not enabled, fetch from the source directly
            //return await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);

            #endregion
        }


        public async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
        {
            if (specification.CacheEnabled)
            {
                string cacheKey = $"{specification.CacheKey}-FirstOrDefaultAsync-{typeof(TResult).Name}";
                _logger.LogInformation("Checking cache for key: {CacheKey}", cacheKey);

                // Try to get the result from Redis cache
                var cachedData = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);

                    // Deserialize the cached data as TResult
                    return JsonSerializer.Deserialize<TResult>(cachedData);
                }

                // Cache miss: Fetch data from the source repository
                _logger.LogWarning("Cache miss. Fetching data for key: {CacheKey}", cacheKey);
                var result = await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);

                if (result != null)
                {
                    // Cache the result in Redis
                    var serializedData = JsonSerializer.Serialize(result);
                    await _distributedCache.SetStringAsync(cacheKey, serializedData, _cacheOptions, cancellationToken);
                }

                return result;
            }

            // If caching is not enabled, fetch from the source directly
            return await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return _sourceRepository.GetByIdAsync(id, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<T> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        {
            return _sourceRepository.GetByIdAsync(id, cancellationToken)!;
        }

        /// <inheritdoc/>
        public async Task<T> GetBySpecAsync<Spec>(Spec specification, CancellationToken cancellationToken = default) where Spec : ISingleResultSpecification, ISpecification<T>
        {
            if (specification.CacheEnabled)
            {
                string cacheKey = $"{specification.CacheKey}-GetBySpecAsync";
                _logger.LogInformation("Checking cache for key: {CacheKey}", cacheKey);

                var result = await _distributedCache.GetOrCreateAsync(
                    cacheKey,
                    _logger,
                    _cacheOptions,
                    async () =>await _sourceRepository.GetBySpecAsync(specification, cancellationToken),
                    cancellationToken);

                return result;
            }

            // If caching is not enabled, fetch from the source directly
            return await _sourceRepository.GetBySpecAsync(specification, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<TResult> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
        {
            if (specification.CacheEnabled)
            {
                string cacheKey = $"{specification.CacheKey}-GetBySpecAsync-{typeof(TResult).Name}";
                _logger.LogInformation("Checking cache for key: {CacheKey}", cacheKey);

                // Try to get the result from Redis cache
                var cachedData = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);

                    // Deserialize the cached data as TResult
                    return JsonSerializer.Deserialize<TResult>(cachedData);
                }

                // Cache miss: Fetch data from the source repository
                _logger.LogWarning("Cache miss. Fetching data for key: {CacheKey}", cacheKey);
                var result = await _sourceRepository.GetBySpecAsync(specification, cancellationToken);

                if (result != null)
                {
                    // Cache the result in Redis
                    var serializedData = JsonSerializer.Serialize(result);
                    await _distributedCache.SetStringAsync(cacheKey, serializedData, _cacheOptions, cancellationToken);
                }

                return result;
            }

            // If caching is not enabled, fetch from the source directly
            return await _sourceRepository.GetBySpecAsync(specification, cancellationToken);
        }

        public async Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            if (specification.CacheEnabled)
            {
                string cacheKey = $"{specification.CacheKey}-GetBySpecAsync";
                _logger.LogInformation("Checking cache for key: {CacheKey}", cacheKey);

                var result = await _distributedCache.GetOrCreateAsync(
                    cacheKey,
                    _logger,
                    _cacheOptions,
                    () => _sourceRepository.GetBySpecAsync(specification, cancellationToken),
                    cancellationToken);

                return result;
            }

            // If caching is not enabled, fetch from the source directly
            return await _sourceRepository.GetBySpecAsync(specification, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            if (specification.CacheEnabled)
            {
                string cacheKey = $"{specification.CacheKey}-ListAsync";
                _logger.LogInformation("Checking cache for key: {CacheKey}", cacheKey);

                var result = await _distributedCache.GetOrCreateAsync(
                    cacheKey,
                    _logger,
                    _cacheOptions,
                    async () => await _sourceRepository.ListAsync(specification, cancellationToken),
                    cancellationToken);

                return result;
            }

            // If caching is not enabled, fetch from the source directly
            return await _sourceRepository.ListAsync(specification, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
        {
            if (specification.CacheEnabled)
            {
                string cacheKey = $"{specification.CacheKey}-ListAsync-{typeof(TResult).Name}";
                _logger.LogInformation("Checking cache for key: {CacheKey}", cacheKey);

                // Try to get the result from Redis cache
                var cachedData = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);

                    // Deserialize the cached data as TResult
                    return JsonSerializer.Deserialize<List<TResult>>(cachedData);
                }

                // Cache miss: Fetch data from the source repository
                _logger.LogWarning("Cache miss. Fetching data for key: {CacheKey}", cacheKey);
                var result = await _sourceRepository.ListAsync(specification, cancellationToken);

                if (result != null)
                {
                    // Cache the result in Redis
                    var serializedData = JsonSerializer.Serialize(result);
                    await _distributedCache.SetStringAsync(cacheKey, serializedData, _cacheOptions, cancellationToken);
                }

                return result;
            }

            // If caching is not enabled, fetch from the source directly
            return await _sourceRepository.ListAsync(specification, cancellationToken);
        }

        public async Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default)
        {
            if (specification.CacheEnabled)
            {
                string cacheKey = $"{specification.CacheKey}-SingleOrDefaultAsync";
                _logger.LogInformation("Checking cache for key: {CacheKey}", cacheKey);

                var result = await _distributedCache.GetOrCreateAsync(
                    cacheKey,
                    _logger,
                    _cacheOptions,
                    async () => await _sourceRepository.SingleOrDefaultAsync(specification, cancellationToken),
                    cancellationToken);

                return result;
            }

            // If caching is not enabled, fetch from the source directly
            return  await _sourceRepository.SingleOrDefaultAsync(specification, cancellationToken);
        }

        public async Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default)
        {
            if (specification.CacheEnabled)
            {
                string cacheKey = $"{specification.CacheKey}-SingleOrDefaultAsync-{typeof(TResult).Name}";
                _logger.LogInformation("Checking cache for key: {CacheKey}", cacheKey);

                // Try to get the result from Redis cache
                var cachedData = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);

                    // Deserialize the cached data as TResult
                    return JsonSerializer.Deserialize<TResult>(cachedData);
                }

                // Cache miss: Fetch data from the source repository
                _logger.LogWarning("Cache miss. Fetching data for key: {CacheKey}", cacheKey);
                var result = await _sourceRepository.SingleOrDefaultAsync(specification, cancellationToken);

                if (result != null)
                {
                    // Cache the result in Redis
                    var serializedData = JsonSerializer.Serialize(result);
                    await _distributedCache.SetStringAsync(cacheKey, serializedData, _cacheOptions, cancellationToken);
                }

                return result;
            }

            // If caching is not enabled, fetch from the source directly
            return await _sourceRepository.SingleOrDefaultAsync(specification, cancellationToken);
        }

    }
}
