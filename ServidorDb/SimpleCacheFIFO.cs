namespace simpleDb
{
    // Implementação de uma estratégia de cache FIFO (First In, First Out).
    public class SimpleCacheFIFO<TKey, TValue> : ICacheStrategy<TKey, TValue>
    {
       public int maxSize = 10;
        private readonly Dictionary<TKey, CacheItem> cache;  
        private readonly Queue<CacheItem> fifoQueue;         

        // Construtor que define o tamanho máximo da cache.
        public SimpleCacheFIFO(int maxSize)
        {
            this.maxSize = maxSize;
            this.cache = new Dictionary<TKey, CacheItem>(maxSize);  
            this.fifoQueue = new Queue<CacheItem>();     
                   
        }

        // Tenta obter um valor da cache com a chave fornecida.
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (cache.TryGetValue(key, out var item))
            {
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
                // Se atingiu o tamanho máximo, remove o item mais antigo (FIFO).
                var oldestItem = fifoQueue.Dequeue();
                cache.Remove(oldestItem.Key);
            }

            // Adiciona o novo item na cache e na fila FIFO.
            var newItem = new CacheItem(key, value);
            cache[key] = newItem;
            fifoQueue.Enqueue(newItem);
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

            // Construtor que inicializa um item com uma chave e um valor.
            public CacheItem(TKey key, TValue value)
            {

                Key = key;
                Value = value;
            }
        }
    }
}
