using Ardalis.Specification;
using HRSystem.Application.Interfaces;
using HRSystem.Domain.Entities;
using HRSystem.Infrastructure.Persistence.Repositories.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HRSystem.Infrastructure.Persistence.Repositories.CachingRepos
{

    /// <inheritdoc/>
    //public class CachedRepository<T> : IReadRepository<T> where T : class
    //{
    //    private readonly IMemoryCache _cache;
    //    private readonly ILogger<CachedRepository<T>> _logger;
    //    private readonly MyRepository<T> _sourceRepository;
    //    private MemoryCacheEntryOptions _cacheOptions;
    //    private readonly CacheSettings _cacheSettings;

    //    public CachedRepository(IMemoryCache cache,
    //        ILogger<CachedRepository<T>> logger,
    //        MyRepository<T> sourceRepository,
    //        IOptions<CacheSettings> cacheSettings)
    //    {
    //        _cache = cache;
    //        _logger = logger;
    //        _sourceRepository = sourceRepository;
    //        _cacheSettings = cacheSettings.Value;
    //        _cacheOptions = new MemoryCacheEntryOptions()
    //            .SetAbsoluteExpiration(relative: TimeSpan.FromSeconds(_cacheSettings.CacheTimeInSeconds));   // TODO we can use SlidingExpiration technique
    //    }

    //    public Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    //    {
    //        if (specification.CacheEnabled)
    //        {
    //            string key = $"{specification.CacheKey}-AnyAsync";
    //            _logger.LogInformation("Checking cache for " + key);
    //            return _cache.GetOrCreate(key, entry =>
    //            {
    //                entry.SetOptions(_cacheOptions);
    //                _logger.LogWarning("Fetching source data for " + key);
    //                return _sourceRepository.AnyAsync(specification, cancellationToken);
    //            })!;
    //        }
    //        return _sourceRepository.AnyAsync(specification, cancellationToken);
    //    }

    //    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    //    {
    //        return _sourceRepository.AnyAsync(cancellationToken);
    //    }

    //    public IAsyncEnumerable<T> AsAsyncEnumerable(ISpecification<T> specification)
    //    {
    //        if (specification.CacheEnabled)
    //        {
    //            string key = $"{specification.CacheKey}-AsAsyncEnumerable";
    //            _logger.LogInformation("Checking cache for " + key);

    //            // Use GetOrCreateAsync for async caching operations
    //            return (IAsyncEnumerable<T>)_cache.GetOrCreateAsync(key, async entry =>
    //            {
    //                entry.SetOptions(_cacheOptions);
    //                _logger.LogWarning("Fetching source data for " + key);

    //                // Fetch the data from the repository as an IEnumerable, then convert to async stream
    //                var data = await _sourceRepository.ListAsync(specification);
    //                return data.AsEnumerable();  // Convert IEnumerable<T> to IAsyncEnumerable<T>
    //            }).Result!;  // GetOrCreateAsync returns a Task, so use Result to unwrap it for IAsyncEnumerable
    //        }

    //        // If no caching is enabled, directly return the data from the repository as an async enumerable
    //        return (IAsyncEnumerable<T>)_sourceRepository.ListAsync(specification).Result.AsEnumerable();
    //    }

    //    /// <inheritdoc/>
    //    public Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    //    {
    //        // TODO: Add Caching
    //        return _sourceRepository.CountAsync(specification, cancellationToken);
    //    }

    //    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    //    {
    //        return _sourceRepository.CountAsync(cancellationToken);
    //    }

    //    public async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    //    {
    //        if (specification.CacheEnabled)
    //        {
    //            string key = $"{specification.CacheKey}-FirstOrDefaultAsync";
    //            _logger.LogInformation("Checking cache for " + key);
    //            return await _cache.GetOrCreateAsync(key, entry =>
    //            {
    //                entry.SetOptions(_cacheOptions);
    //                _logger.LogWarning("Fetching source data for " + key);
    //                return _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);
    //            });
    //        }
    //        return await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);
    //    }

    //    public async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    //    {
    //        if (specification.CacheEnabled)
    //        {
    //            string key = $"{specification.CacheKey}-FirstOrDefaultAsync-{typeof(TResult).Name}";
    //            _logger.LogInformation("Checking cache for " + key);
    //            return await _cache.GetOrCreateAsync(key, entry =>
    //            {
    //                entry.SetOptions(_cacheOptions);
    //                _logger.LogWarning("Fetching source data for " + key);
    //                return _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);
    //            });
    //        }
    //        return await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);
    //    }

    //    /// <inheritdoc/>
    //    public Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    //    {
    //        return _sourceRepository.GetByIdAsync(id, cancellationToken);
    //    }

    //    /// <inheritdoc/>
    //    public Task<T> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
    //    {
    //        return _sourceRepository.GetByIdAsync(id, cancellationToken)!;
    //    }

    //    /// <inheritdoc/>
    //    public Task<T> GetBySpecAsync<Spec>(Spec specification, CancellationToken cancellationToken = default) where Spec : ISingleResultSpecification, ISpecification<T>
    //    {
    //        if (specification.CacheEnabled)
    //        {
    //            string key = $"{specification.CacheKey}-GetBySpecAsync";
    //            _logger.LogInformation("Checking cache for " + key);
    //            return _cache.GetOrCreate(key, entry =>
    //            {
    //                entry.SetOptions(_cacheOptions);
    //                _logger.LogWarning("Fetching source data for " + key);
    //                return _sourceRepository.GetBySpecAsync(specification, cancellationToken);
    //            })!;
    //        }
    //        return _sourceRepository.GetBySpecAsync(specification, cancellationToken);
    //    }

    //    /// <inheritdoc/>
    //    public Task<TResult> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    //    {
    //        if (specification.CacheEnabled)
    //        {
    //            string key = $"{specification.CacheKey}-GetBySpecAsync-{typeof(TResult).Name}";
    //            _logger.LogInformation("Checking cache for " + key);
    //            return _cache.GetOrCreate(key, entry =>
    //            {
    //                entry.SetOptions(_cacheOptions);
    //                _logger.LogWarning("Fetching source data for " + key);
    //                return _sourceRepository.GetBySpecAsync(specification, cancellationToken);
    //            })!;
    //        }
    //        return _sourceRepository.GetBySpecAsync(specification, cancellationToken);
    //    }

    //    public Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    //    {
    //        if (specification.CacheEnabled)
    //        {
    //            string key = $"{specification.CacheKey}-GetBySpecAsync";
    //            _logger.LogInformation("Checking cache for " + key);
    //            return _cache.GetOrCreate(key, entry =>
    //            {
    //                entry.SetOptions(_cacheOptions);
    //                _logger.LogWarning("Fetching source data for " + key);
    //                return _sourceRepository.GetBySpecAsync(specification, cancellationToken);
    //            })!;
    //        }
    //        return _sourceRepository.GetBySpecAsync(specification, cancellationToken);
    //    }

    //    /// <inheritdoc/>
    //    public Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
    //    {
    //        string key = $"{nameof(T)}-ListAsync";
    //        return _cache.GetOrCreate(key, entry =>
    //        {
    //            entry.SetOptions(_cacheOptions);
    //            return _sourceRepository.ListAsync(cancellationToken);
    //        })!;
    //    }

    //    /// <inheritdoc/>
    //    public Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    //    {
    //        if (specification.CacheEnabled)
    //        {
    //            string key = $"{specification.CacheKey}-ListAsync";
    //            _logger.LogInformation("Checking cache for " + key);
    //            return _cache.GetOrCreate(key, entry =>
    //            {
    //                entry.SetOptions(_cacheOptions);
    //                _logger.LogWarning("Fetching source data for " + key);
    //                return _sourceRepository.ListAsync(specification, cancellationToken);
    //            })!;
    //        }
    //        return _sourceRepository.ListAsync(specification, cancellationToken);
    //    }

    //    /// <inheritdoc/>
    //    public Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    //    {
    //        if (specification.CacheEnabled)
    //        {
    //            string key = $"{specification.CacheKey}-ListAsync-{typeof(TResult).Name}";
    //            _logger.LogInformation("Checking cache for " + key);
    //            return _cache.GetOrCreate(key, entry =>
    //            {
    //                entry.SetOptions(_cacheOptions);
    //                _logger.LogWarning("Fetching source data for " + key);
    //                return _sourceRepository.ListAsync(specification, cancellationToken);
    //            })!;
    //        }
    //        return _sourceRepository.ListAsync(specification, cancellationToken);
    //    }

    //    public async Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default)
    //    {
    //        if (specification.CacheEnabled)
    //        {
    //            string key = $"{specification.CacheKey}-SingleOrDefaultAsync";
    //            _logger.LogInformation("Checking cache for " + key);
    //            return await _cache.GetOrCreateAsync(key, async entry =>
    //            {
    //                entry.SetOptions(_cacheOptions);
    //                _logger.LogWarning("Fetching source data for " + key);
    //                return await _sourceRepository.SingleOrDefaultAsync(specification, cancellationToken);
    //            });
    //        }
    //        return await _sourceRepository.SingleOrDefaultAsync(specification, cancellationToken);
    //    }

    //    public async Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    //    {
    //        if (specification.CacheEnabled)
    //        {
    //            string key = $"{specification.CacheKey}-SingleOrDefaultAsync-{typeof(TResult).Name}";
    //            _logger.LogInformation("Checking cache for key: {Key}", key);

    //            // Use async lambda in GetOrCreateAsync to avoid blocking the thread.
    //            return await _cache.GetOrCreateAsync(key, async entry =>
    //            {
    //                entry.SetOptions(_cacheOptions);
    //                _logger.LogWarning("Cache miss. Fetching source data for key: {Key}", key);
    //                var result = await _sourceRepository.SingleOrDefaultAsync(specification, cancellationToken);

    //                // Handle case where the sourceRepository result is null
    //                if (result is null)
    //                {
    //                    _logger.LogWarning("No data found in source repository for key: {Key}", key);
    //                }

    //                return result;
    //            });
    //        }

    //        // Fallback to direct source retrieval if caching is not enabled
    //        return await _sourceRepository.SingleOrDefaultAsync(specification, cancellationToken);
    //    }

    //}

}
