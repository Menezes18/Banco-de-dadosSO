using System;
using System.Collections.Generic;
using System.IO;

class Program
{

    //estrutura esqueleto do projeto
    static void Main(string[] args)
    {


        while (true)
        {
            Console.Write("$ simpledb-client> ");
            string input = Console.ReadLine();

            string command = Console.ReadLine();

            switch (command)
            {
                case "insert":

                    break;

                case "update":

                    break;

                case "remove":

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