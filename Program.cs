using System;
using System.Collections.Generic;
using System.IO;

class Simpledb
{
    private Dictionary<string, string> database;
    private string dataPath;

    public Simpledb(string dataPath)
    {
        this.dataPath = dataPath;
        LoadData();
    }

    public void Insert(string key, string value)
    {
        if (!database.ContainsKey(key))
        {
            database[key] = value;
            SaveData();
            Console.WriteLine("inserted");
        }
        else
        {
            Console.WriteLine("Key already exists. Use the Update to modify the object");
        }
    }

    public void Update(string key, string value)
    {
        if (database.ContainsKey(key))
        {
            database[key] = value;
            SaveData();
            Console.WriteLine("updated");
        }
        else
        {
            Console.WriteLine("Key not found. Use the Insert to add a new object");
        }
    }

    public void Remove(string key)
    {
        if (database.ContainsKey(key))
        {
            database.Remove(key);
            SaveData();
            Console.WriteLine("removed");
        }
        else
        {
            Console.WriteLine("Key not found.");
        }
    }

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

class Program
{
    static void Main(string[] args)
    {
        string dataNamePath = "keyvalue.db";
        Simpledb Database = new Simpledb(dataNamePath);

       if(args.Length == 0) return;

        string command = args[0];  
        string[] input = args[1].Split(',');
        string key = input.Length > 0 ? input[0] : null;
        string value = input.Length > 1 ? input[1] : null;



        switch (command)
        {
            case "--insert":

                if (key != null && value != null)
                {
                    Database.Insert(key, value);
                }
                else
                {
                    Console.WriteLine("Usage: --insert key,value");
                }
                break;

            case "--update":
                if (key != null && value != null)
                {
                    Database.Update(key, value);
                }
                else
                {
                    Console.WriteLine("Usage: --update key,value");
                }
                break;

            case "--remove":
                if (key != null)
                {
                    Database.Remove(key);
                }
                else
                {
                    Console.WriteLine("Usage: --remove key");
                }
                break;

            case "--search":
                if (key != null)
                {
                    var result = Database.Search(key);
                    if (result != null)
                    {
                        Console.WriteLine(result);
                    }
                }
                else
                {
                    Console.WriteLine("Usage: --search key");
                }
                break;

            default:
                Console.WriteLine("Invalid command. Try again");
                break;
        }
    }
}

