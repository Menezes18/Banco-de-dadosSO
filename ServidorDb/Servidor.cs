using System;
using System.Collections.Generic;
using System.IO;
using MSMQ.Messaging;
using System.Threading;

namespace simpleDb
{
    // Classe principal do programa.
    class Program
    {
        
        // Inicialização da fila de mensagens
        const string msQueuePath = @".\Private$\simpleDbQueue"; // Caminho da fila de mensagens do servidor
        const string clientMsPath = @".\Private$\ClientQueue"; // Caminho da fila de mensagens do cliente

        static void Main(string[] args)
        {


            string dataNamePath = "DataBase.db"; // Caminho para salvar o banco de dados

            // Instância da classe Simpledb para interagir com o banco de dados
            ICacheStrategy<string, string> cacheStrategy = null;
            CacheManager<string, string> cacheManager = null;

            if (args.Length > 0)
            {
                // Se há argumentos de linha de comando, processa-os e retorna
                ReadLinesArgs(args, dataNamePath, ref cacheStrategy, ref cacheManager);
                return;
            }

            Console.WriteLine("Selecting cache strategy...");
            cacheStrategy = SelectCacheStrategy();
            cacheManager = new CacheManager<string, string>(cacheStrategy);
            Console.WriteLine("Running");
            CreateQueue(); // Cria a fila de mensagens

            MessageQueue messageQueue = new MessageQueue(msQueuePath);
            messageQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(Command) });

            // Loop principal
            while (true)
            {
                Thread thread = new Thread(() =>
                {
                    // Recebe uma mensagem da fila
                    Message msg = messageQueue.Receive();
                    Command msQueue = (Command)msg.Body;

                    // Função para lidar com a comunicação do cliente
                    ClientCommunication(dataNamePath, cacheStrategy, cacheManager, msQueue);

                    messageQueue.Close(); // Fecha a fila após o processamento
                });

                thread.Start(); // Inicia uma nova thread para processar a mensagem
            }

            Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e) {
                DeleteQueue();
            };
        }

        static ICacheStrategy<string, string> SelectCacheStrategy()
        {
            Console.Write("Select cache strategy (fifo/lru/aging): ");
            string strategy = Console.ReadLine().ToLower();

            switch (strategy)
            {
                case "fifo":
                    return new SimpleCacheFIFO<string, string>();
                case "lru":
                    return new SimpleCacheLRU<string, string>();
                case "aging":
                    //return new SimpleCacheAging<string, string>();
                default:
                    Console.WriteLine("Invalid cache strategy. Using FIFO as default.");
                    return new SimpleCacheFIFO<string, string>();
            }
        }

        // Função para lidar com a comunicação do cliente
        static void ClientCommunication(string dataNamePath, ICacheStrategy<string, string> cacheStrategy, CacheManager<string, string> cacheManager, Command command)
        {
            // Instância da classe Simpledb para interagir com o banco de dados
            Simpledb Database = new Simpledb(dataNamePath, cacheStrategy);

            // Recebe e processa a mensagem
            Console.WriteLine("Load Response...");
            string response = Database.Execute(command);
            Message ms = new Message(response);

            // Envia a resposta de volta ao cliente
            Console.WriteLine("Sending Response");
            MessageQueue clientMsQueue = new MessageQueue(clientMsPath)
            {
                Formatter = new XmlMessageFormatter(new Type[] { typeof(string) })
            };
            clientMsQueue.Send(ms);
            clientMsQueue.Close(); // Fecha a fila do cliente após o envio da resposta
        }

        // Função para criar a fila se não existir
        static void CreateQueue()
        {
            if (!MessageQueue.Exists(msQueuePath))
            {
                MessageQueue.Create(msQueuePath);
            }
        }

        // Função para excluir a fila se existir
        static void DeleteQueue()
        {
            if (MessageQueue.Exists(msQueuePath))
            {
                MessageQueue.Delete(msQueuePath);
            }
        }
        // Função para processar os argumentos de linha de comando
        static void ReadLinesArgs(string[] args, string dataNamePath, ref ICacheStrategy<string, string> cacheStrategy, ref CacheManager<string, string> cacheManager)
        {
            string entry = args[0].ToLower();
            string[] parts = args[1].Split(',');

            Command command = new Command();
            command.Key = parts[0];

            switch (entry)
            {
                case "--insert":
                    // Comando para inserir um novo par chave-valor no banco de dados.
                    if (parts.Length != 2)
                    {
                        Console.WriteLine("Incorrect usage. Use: insert key, value");
                    }
                    else
                    {
                            command.Value = parts[1];      
                            command.Op = Operacao.Insert;  
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
                            command.Value = parts[1];  
                            command.Op = Operacao.Update;
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
                        command.Op = Operacao.Remove;
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
                    command.Op = Operacao.Search;
                    var result = command.Op; // Assuming you want to retrieve the operation
                    if (result != null)
                    {
                        Console.WriteLine(result);
                    }
                }
                break;

                case "-cache-size":
                int cacheSize;
                if (int.TryParse(parts[1], out cacheSize) && cacheSize > 0)
                {
                    // Assuming SelectCacheStrategy doesn't take any parameters
                    cacheStrategy = SelectCacheStrategy();
                    cacheManager = new CacheManager<string, string>(cacheStrategy);
                    Console.WriteLine($"Cache size set to: {cacheSize}");
                }
                else
                {
                    Console.WriteLine("Invalid cache size. Using default size.");
                }
                continue;

                case "--quit":
                    // Comando para sair do programa.
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Invalid command. Try again");
                    break;

            }

                
               

                string response = simpledb.Execute(command);
                Console.WriteLine(response);   
        }
    }
}

