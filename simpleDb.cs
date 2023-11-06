using System;
using System.Collections.Generic;
using System.IO;

// Classe que representa um banco de dados simples chave-valor.
class Simpledb
{
    private Dictionary<string, string> database; // Um dicionário para armazenar os pares chave-valor.
    private string dataPath; // Caminho para o arquivo de dados.

    // Construtor da classe.
    public Simpledb(string dataPath)
    {
        this.dataPath = dataPath;
        LoadData(); // Carrega os dados do arquivo.
    }

    // Método para inserir um novo par chave-valor no banco de dados.
    public void Insert(string key, string value)
    {
        if (!database.ContainsKey(key))
        {
            database[key] = value;
            SaveData(); // Salva os dados no arquivo.
            Console.WriteLine("inserted");
        }
        else
        {
            Console.WriteLine("Key already exists. Use the Update to modify the object");
        }
    }

    // Método para atualizar um valor existente no banco de dados.
    public void Update(string key, string value)
    {
        if (database.ContainsKey(key))
        {
            database[key] = value;
            SaveData(); // Salva os dados no arquivo.
            Console.WriteLine("updated");
        }
        else
        {
            Console.WriteLine("Key not found. Use the Insert to add a new object");
        }
    }

    // Método para remover um par chave-valor do banco de dados.
    public void Remove(string key)
    {
        if (database.ContainsKey(key))
        {
            database.Remove(key);
            SaveData(); // Salva os dados no arquivo.
            Console.WriteLine("removed");
        }
        else
        {
            Console.WriteLine("Key not found.");
        }
    }

    // Método para procurar um valor no banco de dados com base na chave.
    public string Search(string key)
    {
        if (database.ContainsKey(key))
        {
            return database[key];
        }
        else
        {
            Console.WriteLine("Not found");
            return null;
        }
    }

    // Método privado para carregar os dados do arquivo para o dicionário.
    private void LoadData()
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

    // Método privado para salvar os dados do dicionário no arquivo.
    private void SaveData()
    {
        List<string> lines = new List<string>();
        foreach (var entry in database)
        {
            lines.Add(entry.Key + "," + entry.Value);
        }
        File.WriteAllLines(dataPath, lines);
    }
}