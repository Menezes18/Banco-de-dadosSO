using System;
using MSMQ.Messaging;

namespace simpleDb
{

    // Classe principal do programa.
    class Program
    {
        //Inicialização da fila de mensagens
        const string msQueuePath= (".\\Private$\\simpleDbQueue"); // fila de mensagens do servidor
        const string clientMsPath = (".\\Private$\\ClientQueue"); // fila de mensagens do client
        static void Main(string[] args)
        {
            string dataNamePath = "DataBase.db"; // banco de dados save path

            Simpledb Database = new Simpledb(dataNamePath);

            if(args.Length > 0){
            ReadLinesArgs(args, Database);
            return;
            }
            Console.WriteLine("Rodando");
            CreateQueue();

            MessageQueue messageQueue = new MessageQueue(msQueuePath);
            messageQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(Command) });
          

                        while(true)
                        {
                            Message msg = messageQueue.Receive();
                            Command msQueue = (Command)msg.Body;

                            ClientComunication(Database, msQueue);

                            messageQueue.Close();
                        }

            

                    static void ClientComunication(Simpledb simpledb, Command command)
                    {
                        //Recebendo e processando a mensagem
                        Console.WriteLine("LoadResponse...");
                        string response = simpledb.Execute(command);
                        Message ms = new Message(response);

                        //Retornando resposta
                        Console.WriteLine("Sending Response");
                        MessageQueue clientMsQueue = new MessageQueue(clientMsPath)
                        {
                            Formatter = new XmlMessageFormatter(new Type[] { typeof(string) })
                        };
                        clientMsQueue.Send(ms);
                        clientMsQueue.Close();
                    }

                    DeleteQueue();

                    static void CreateQueue()
                    {
                        if(!MessageQueue.Exists(msQueuePath)) {

                            MessageQueue.Create(msQueuePath);
                        }
                    }

                    static void DeleteQueue()
                    {
                        if(MessageQueue.Exists(msQueuePath)){
                        MessageQueue.Delete(msQueuePath);
                        } 
                    }

                    static void ReadLinesArgs(string[] args, Simpledb simpledb)
                    {

                            string input = Console.ReadLine();
                            string[] parts = input.Split(' ');
                            string entry = parts[0].ToLower();

                            Command command = new Command();

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
                                        string[] keyValue = parts[1].Split(',');
                                        if (keyValue.Length != 2)
                                        {
                                            Console.WriteLine("Incorrect usage. Use: insert key, value");
                                        }
                                        else
                                        {
                                            string key = keyValue[0];
                                            string value = keyValue[1];
                                            command.Op = Operacao.Insert;
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
                                            command.Op = Operacao.Update;
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
                                        string key = parts[1];
                                        var result = command.Op = Operacao.Search;
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

                                    // try { //MUDANCA
                                    //     string resposta = simpledb.Execute(command);
                                    //     Console.WriteLine(resposta);
                                    // }
                                    // catch (Exception e) {
                                    //     Console.WriteLine("error: " + e.Message);
                                    // }
                }
            }
        }
    }
}
