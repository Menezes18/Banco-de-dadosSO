namespace simpleDb
{
    // Implementação de uma estratégia de cache LRU (Least Recently Used).
    public class SimpleCacheLRU<TKey, TValue> : ICacheStrategy<TKey, TValue>
    {
        public int maxSize = 10;
        private readonly Dictionary<TKey, LinkedListNode<CacheItem>> cache; 
        private readonly LinkedList<CacheItem> lruList;                    

        // Construtor que define o tamanho máximo da cache.
        public SimpleCacheLRU(int maxSize)
        {
            this.maxSize = maxSize;
            this.cache = new Dictionary<TKey, LinkedListNode<CacheItem>>(maxSize);  
            this.lruList = new LinkedList<CacheItem>();                           
        }

        // Tenta obter um valor da cache com a chave fornecida.
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (cache.TryGetValue(key, out var node))
            {
                // Move o nó para o final da lista (LRU).
                lruList.Remove(node);
                lruList.AddLast(node);

                value = node.Value.Value;
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
                // Se atingiu o tamanho máximo, remove o item menos recentemente usado.
                TKey lruKey = lruList.First.Value.Key;
                cache.Remove(lruKey);
                lruList.RemoveFirst();
            }

            // Adiciona o novo item na cache e no final da lista (LRU).
            var newItem = new CacheItem(key, value);
            var node = new LinkedListNode<CacheItem>(newItem);
            cache[key] = node;
            lruList.AddLast(node);
        }

        // Tenta remover um item da cache com a chave fornecida.
        public bool TryRemove(TKey key, out TValue value)
        {
            if (cache.TryGetValue(key, out var node))
            {
                // Remove o item da cache se estiver presente.
                cache.Remove(key);
                lruList.Remove(node);

                value = node.Value.Value;
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

            // Construtor que inicializa um item com uma chave e um valor.
            public CacheItem(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}
