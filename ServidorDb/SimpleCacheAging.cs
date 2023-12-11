namespace simpleDb
{
    // Implementação de uma estratégia de cache Aging.
    public class SimpleCacheAging<TKey, TValue> : ICacheStrategy<TKey, TValue>
    {
        public int maxSize = 10;
        private readonly Dictionary<TKey, CacheItem> cache;  

        // Construtor que define o tamanho máximo da cache.
        public SimpleCacheAging(int maxSize)
        {
            this.maxSize = maxSize;
            this.cache = new Dictionary<TKey, CacheItem>(maxSize);  
        }

        // Tenta obter um valor da cache com a chave fornecida.
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (cache.TryGetValue(key, out var item))
            {
                // Redefine a idade do item para um valor baixo quando é acessado.
                item.Age = 0;
                value = item.Value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        // Adiciona ou atualiza um item na cache.
        public void AddOrUpdate(TKey key, TValue value)
        {
            if (cache.Count >= maxSize)
            {
                // Se atingiu o tamanho máximo, envelhece todos os itens na cache.
                foreach (var cacheItem in cache.Values)
                {
                    cacheItem.Age++;
                }

                // Remove o item mais antigo.
                var oldestItem = cache.Values.OrderBy(i => i.Age).FirstOrDefault();
                if (oldestItem != null)
                {
                    cache.Remove(oldestItem.Key);
                }
            }

            // Adiciona ou atualiza o item na cache com uma idade inicial baixa.
            var newItem = new CacheItem(key, value);
            cache[key] = newItem;
        }

        // Tenta remover um item da cache com a chave fornecida.
        public bool TryRemove(TKey key, out TValue value)
        {
            if (cache.TryGetValue(key, out var item))
            {
                cache.Remove(key);
                value = item.Value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        // Classe interna que representa um item na cache.
        private class CacheItem
        {
            public TKey Key { get; }
            public TValue Value { get; }
            public int Age { get; set; }  // Idade do item na cache.

            // Construtor que inicializa um item com uma chave, um valor e uma idade inicial baixa.
            public CacheItem(TKey key, TValue value)
            {
                Key = key;
                Value = value;
                Age = 0;
            }
        }
    }
}
