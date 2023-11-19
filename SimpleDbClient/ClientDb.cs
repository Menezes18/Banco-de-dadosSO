using System;
using MSMQ.Messaging;

namespace Client
{
    class Program
    {
        const string msQueuePath = (".\\Private$\\ClientQueue"); // fila de mensagens
        const string servidorMessageQueue = (".\\Private$\\simpleDBQueue"); // fila de mensagens do servidor

        static void Main(string[] args)
        {   
            CreateQueue();


                Console.Write("$ simpledb-client> ");

                string input = Console.ReadLine();
                string[] parts = input.Split(' ');
                string entry = parts[0].ToLower();

                Command command = new Command();

                switch(entry){
                    case "--insert":
                    command.Op = Operacao.Insert;
                    break;
                    case "--update":
                    command.Op = Operacao.Update;
                    break;
                    case "--remove":
                    command.Op = Operacao.Remove;
                    break;
                    case "--search":
                    command.Op = Operacao.Search;
                    break;
                    case "--quit":
                    Environment.Exit(0);
                    break;

                    default:
                        Console.WriteLine("Invalid command. Try again");
                        break;
                }

                string[] separateValue = parts[1].Split(',');
                command.Key = separateValue[0];

                if(separateValue.Length > 1){
                        command.Value = separateValue[1];
                    }


                MessageQueue messageQueue = new MessageQueue(servidorMessageQueue)
                {
                    Formatter = new XmlMessageFormatter(new Type[] { typeof(Command) })
                };

                MessageQueue clientMsPath = new MessageQueue(msQueuePath)
                {
                    Formatter = new XmlMessageFormatter(new Type[] { typeof(string) })
                };

                try
                {   Console.WriteLine("Enviando Mensagem");
                    Message ms = new Message(command);
                    messageQueue.Send(ms);
                    
                    Console.WriteLine("Recebendo Mensagem");
                    Message clientMsg = clientMsPath.Receive();
                    string answer = (string)clientMsg.Body;
                    clientMsPath.Close();
                    messageQueue.Close();

                    Console.WriteLine(answer);

                    }catch (Exception e){
                    Console.WriteLine("error: " + e.Message);
                    
                   } 

                   if(MessageQueue.Exists(msQueuePath)){
                        MessageQueue.Delete(msQueuePath);
                   }             

               
                      
            static void CreateQueue()
            {
                 if(!MessageQueue.Exists(msQueuePath))
                 {
                    MessageQueue.Create(msQueuePath);
                 }  
            }
        }
    }
}