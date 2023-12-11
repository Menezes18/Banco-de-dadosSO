
public interface ICacheStrategy<TKey, TValue>
{
    bool TryGetValue(TKey key, out TValue value);
    void AddOrUpdate(TKey key, TValue value);
    bool TryRemove(TKey key, out TValue value);
}
public class CacheManager<TKey, TValue>
{
    private readonly ICacheStrategy<TKey, TValue> cacheStrategy;

    public CacheManager(ICacheStrategy<TKey, TValue> cacheStrategy)
    {
        this.cacheStrategy = cacheStrategy;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return cacheStrategy.TryGetValue(key, out value);
    }

    public void AddOrUpdate(TKey key, TValue value)
    {
        cacheStrategy.AddOrUpdate(key, value);
    }

    public bool TryRemove(TKey key, out TValue value)
    {
        return cacheStrategy.TryRemove(key, out value);
    }
}