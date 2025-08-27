using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace JOB_FINDER_API.Models.Services
{
    public static class EmergencyPerformanceHelper
    {
        private static readonly ConcurrentDictionary<string, (DateTime created, string result)> _emergencyCache 
            = new ConcurrentDictionary<string, (DateTime, string)>();
        
        private static readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5);
        
        public static bool TryGetCachedResult(string key, out string result)
        {
            result = null;
            if (_emergencyCache.TryGetValue(key, out var cached))
            {
                if (DateTime.UtcNow - cached.created < _cacheExpiry)
                {
                    result = cached.result;
                    return true;
                }
                else
                {
                    _emergencyCache.TryRemove(key, out _);
                }
            }
            return false;
        }
        
        public static void CacheResult(string key, string result)
        {
            _emergencyCache[key] = (DateTime.UtcNow, result);
            
            // Clean old entries every 100 inserts
            if (_emergencyCache.Count > 100)
            {
                Task.Run(() => CleanOldEntries());
            }
        }
        
        private static void CleanOldEntries()
        {
            var cutoff = DateTime.UtcNow - _cacheExpiry;
            var keysToRemove = new List<string>();
            
            foreach (var kvp in _emergencyCache)
            {
                if (kvp.Value.created < cutoff)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }
            
            foreach (var key in keysToRemove)
            {
                _emergencyCache.TryRemove(key, out _);
            }
        }
        
        public static string CreateCacheKey(string jobId, string cvId, string operation)
        {
            return $"{operation}_{jobId}_{cvId}";
        }
    }
}