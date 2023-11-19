using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace simpleDb
{

    // Classe que representa um banco de dados simples chave-valor.
    class Simpledb
    {
        private Dictionary<string, string> database; // Um dicionário para armazenar os pares chave-valor.
        private string dataPath; // Caminho para o arquivo de dados.
        private Mutex mutex; // Mutex utilizado para lidar com a exclusão mútua.

        // Construtor da classe.
        public Simpledb(string dataPath)
        {
            this.dataPath = dataPath;
            this.mutex = new Mutex();
            LoadData(); // Carrega os dados do arquivo.
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
            switch(command.Op){
                case Operacao.Insert:
                return Insert(command.Key,command.Value);
                case Operacao.Update:
                return Update(command.Key,command.Value);      
                case Operacao.Remove:
                return Remove(command.Key);
                case Operacao.Search:
                return Search(command.Key); 
                default: return null;
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