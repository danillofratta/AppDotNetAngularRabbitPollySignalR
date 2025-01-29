namespace ApiStock.Service.Redis
{
    public interface IRedisCacheService
    {
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<T> GetAsync<T>(string key);
        Task RemoveAsync(string key);
        Task AddToSetAsync(string key, string value);
        Task RemoveFromSetAsync(string key, string value);
        Task<string[]> GetSetMembersAsync(string key);
    }
}
