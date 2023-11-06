using System;
using System.Collections.Generic;
using System.IO;

// Classe principal do programa.
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
                case "--insert":
                    // Comando para inserir um novo par chave-valor no banco de dados.
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

                case "--update":
                    // Comando para atualizar um valor existente no banco de dados.
                    if (parts.Length != 2)
                    {
                        Console.WriteLine("Incorrect usage. Use: update key, value");
                    }
                    else
                    {
                        string[] keyValue = parts[1].Split(',');
                        if (keyValue.Length != 2)
                        {
                            Console.WriteLine("Incorrect usage. Use: update key, value");
                        }
                        else
                        {
                            string key = keyValue[0];
                            string value = keyValue[1];
                            Database.Update(key, value);
                        }
                    }
                    break;

                case "--remove":
                    // Comando para remover um par chave-valor do banco de dados.
                    if (parts.Length != 2)
                    {
                        Console.WriteLine("Incorrect usage. Use: remove key");
                    }
                    else
                    {
                        string key = parts[1];
                        Database.Remove(key);
                    }
                    break;

                case "--search":
                    // Comando para procurar um valor no banco de dados com base na chave.
                    if (parts.Length != 2)
                    {
                        Console.WriteLine("Incorrect usage. Use: search key");
                    }
                    else
                    {
                        string key = parts[1];
                        var result = Database.Search(key);
                        if (result != null)
                        {
                            Console.WriteLine(result);
                        }
                    }
                    break;

                case "--quit":
                    // Comando para sair do programa.
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Invalid command. Try again");
                    break;
            }
        }
    }
}
