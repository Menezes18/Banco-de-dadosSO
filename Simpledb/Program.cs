﻿using System;
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
        string dataNamePath = "DataBase.db";
        Simpledb Database = new Simpledb(dataNamePath);
        

        while (true)
        {
            Console.Write("$ simpledb-client> ");
            string input = Console.ReadLine();

            string[] parts = input.Split(' ');
            string command = parts[0].ToLower();

            switch (command)
            {
                case "insert":
                    if (parts.Length != 2)
                    {
                        Console.WriteLine("Incorrect usage. Use: insert key, value");
                    }
                    else
                    {
                        string[] keyValue = parts[1].Split(',');
                        if (keyValue.Length != 2)
                        {
                            Console.WriteLine("Incorrect usage. Use: insert key, value");
                        }
                        else
                        {
                            string key = keyValue[0];
                            string value = keyValue[1];
                            Database.Insert(key, value);
                        }
                    }
                    break;

                case "update":
                   
                    break;

                case "remove":
                   
                    break;

                case "search":
                   
                    break;

                case "quit":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Invalid command. Try again");
                    break;
            }
        }
    }
}
