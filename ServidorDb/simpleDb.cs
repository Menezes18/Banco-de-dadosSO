using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace simpleDb{

    class Simpledb
    {
        private Dictionary<string, string> database; // Um dicionário para armazenar os pares chave-valor.
        private string dataPath; // Caminho para o arquivo de dados.
        private Mutex mutex; // Mutex utilizado para lidar com a exclusão mútua.
        public CacheManager<string, string> cacheManager;
        public int cacheSize;

        public Simpledb(string dataPath)
        {
            this.dataPath = dataPath;
            this.mutex = new Mutex();
            this.cacheManager = cacheManager;
            LoadData(); // Carrega os dados do arquivo.
        }
        private void UpdateCache(string key, string value)
        {
            // Tenta obter o valor da cache, se não estiver presente, adiciona.
            if (!cacheManager.TryGetValue(key, out var cachedValue))
            {
                cacheManager.AddOrUpdate(key, value);
            }
        }

        private void RemoveFromCache(string key)
        {
            // Remove o item da cache se estiver presente.
            cacheManager.TryRemove(key, out var removedValue);
        }

        // Método para inserir um novo par chave-valor no banco de dados.
        public string Insert(string key, string value)
        {
            mutex.WaitOne(); // Adquire o mutex para garantir exclusão mútua.

            try
            {
                if (!database.ContainsKey(key))
                {
                    database[key] = value;
                    UpdateCache(key, value);
                    SaveData(); // Salva os dados no arquivo.
                    return "inserted";
                }
                else
                {
                    return "Key already exists. Use Update to modify the object";
                }
            }
            finally
            {
                mutex.ReleaseMutex(); // Libera o mutex.
            }
        }

        // Método para atualizar um valor existente no banco de dados.
        public string Update(string key, string value)
        {
            mutex.WaitOne(); // Adquire o mutex para garantir exclusão mútua.

            try
            {
                if (database.ContainsKey(key))
                {
                    database[key] = value;
                    UpdateCache(key, value);
                    SaveData(); // Salva os dados no arquivo.
                    return "updated";
                }
                else
                {
                    return "Key not found. Use Insert to add a new object";
                }
            }
            finally
            {
                mutex.ReleaseMutex(); // Libera o mutex.
            }
        }

        // Método para remover um par chave-valor do banco de dados.
        public string Remove(string key)
        {
            mutex.WaitOne(); // Adquire o mutex para garantir exclusão mútua.

            try
            {
                if (database.ContainsKey(key))
                {
                    database.Remove(key);
                    RemoveFromCache(key);
                    SaveData(); // Salva os dados no arquivo.
                    return "removed";
                }
                else
                {
                    return "Key not found.";
                }
            }
            finally
            {
                mutex.ReleaseMutex(); // Libera o mutex.
            }
        }

        // Método para procurar um valor no banco de dados com base na chave.
        public string Search(string key)
        {
            mutex.WaitOne(); // Adquire o mutex para garantir exclusão mútua.

            try
            {
                if (database.ContainsKey(key))
                {
                    UpdateCache(key, database[key]);
                    return database[key];
                }
                else
                {
                    return "Not Found";
                }
            }
            finally
            {
                mutex.ReleaseMutex(); // Libera o mutex.
            }
        }


        // Método para executar os comandos do client através do servidor
        public string Execute(Command command)
        {

            if (cacheManager != null)
            {   
                // Exemplo: Tenta obter o valor da cache antes de acessar o banco de dados
                if (cacheManager.TryGetValue(command.Key, out var cachedValue))
                {
                    Console.WriteLine($"Cache hit! Key: {command.Key}, Value: {cachedValue}");
                    return cachedValue;
                }
            }
            // Use a estratégia de cache depois de executar o comando
            if (cacheManager != null)
            {
                // Exemplo: Adiciona ou atualiza a cache após acessar o banco de dados
                cacheManager.AddOrUpdate(command.Key, command.Value);
                Console.WriteLine($"Added/Updated cache. Key: {command.Key}, Value: {command.Value}");
            }

            return command.Value;
    
             switch (command.Op)
            {
                case Operacao.Insert:
                    return Insert(command.Key, command.Value);
                case Operacao.Update:
                    return Update(command.Key, command.Value);
                case Operacao.Remove:
                    return Remove(command.Key);
                case Operacao.Search:
                    return Search(command.Key);
                default:
                    return null;
            }
        }

        // Método privado para carregar os dados do arquivo para o dicionário.
           private void LoadData()
        {
            mutex.WaitOne(); // Adquire o mutex para garantir exclusão mútua.

            try
            {
                database = new Dictionary<string, string>();

                if (File.Exists(dataPath))
                {
                    string[] lines = File.ReadAllLines(dataPath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length == 2)
                        {
                            string key = parts[0];
                            string value = parts[1];
                            database[key] = value;
                        }
                    }
                }
            }
            finally
            {
                mutex.ReleaseMutex(); // Libera o mutex.
            }
        }

        // Método privado para salvar os dados do dicionário no arquivo.
        private void SaveData()
        {
            
            mutex.WaitOne(); // Adquire o mutex para garantir exclusão mútua.

            try
            {
                List<string> lines = new List<string>();
                foreach (var entry in database)
                {
                    lines.Add(entry.Key + "," + entry.Value);
                }
                File.WriteAllLines(dataPath, lines);
            }
            finally
            {
                mutex.ReleaseMutex(); // Libera o mutex.
            }
        }
    }
}