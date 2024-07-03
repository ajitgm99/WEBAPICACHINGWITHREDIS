
using StackExchange.Redis;
using System.Text.Json;

namespace WEBAPICACHINGWITHREDIS.Services
{
    public class CachedService : ICacheService
    {
        private IDatabase _cacheDB;
        public CachedService() 
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _cacheDB=redis.GetDatabase();
        }

        public T GetData<T>(string key)
        {
            var value=_cacheDB.StringGet(key);

            if(!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default(T);
            
        }

        public object RemoveData(string key)
        {
            var isexist=_cacheDB.KeyExists(key);

            if (isexist)
            {
                return _cacheDB.KeyDelete(key);
            }
            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);

            return _cacheDB.StringSet(key,JsonSerializer.Serialize(value), expiryTime);
        }
    }
}
