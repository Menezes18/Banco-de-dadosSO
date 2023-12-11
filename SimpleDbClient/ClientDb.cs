using System;
using MSMQ.Messaging;

namespace Client
{
    class Program
    {
        // Caminho da fila de mensagens do cliente
        const string msQueuePath = (".\\Private$\\ClientQueue");

        // Caminho da fila de mensagens do servidor
        const string servidorMessageQueue = (".\\Private$\\simpleDBQueue");

        static void Main(string[] args)
        {
            // Cria a fila de mensagens do cliente
            CreateQueue();

            Console.Write("$ simpledb-client> ");

            // Lê a entrada do usuário
            string input = Console.ReadLine();
            string[] parts = input.Split(' ');
            string entry = parts[0].ToLower();

            // Cria um objeto Command para representar a mensagem
            Command command = new Command();

            // Processa o comando inserido
            switch (entry)
            {
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
                case "-cache-size":
                     command.Op = Operacao.Cache;
                    break;
                case "--quit":
                    // Comando para sair do programa
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid command. Try again");
                    break;
            }

            // Processa a chave e o valor, se fornecidos
            string[] separateValue = parts[1].Split(',');
            command.Key = separateValue[0];

            if (separateValue.Length > 1)
            {
                command.Value = separateValue[1];
            }

            // Configuração da fila de mensagens do servidor
            MessageQueue messageQueue = new MessageQueue(servidorMessageQueue)
            {
                Formatter = new XmlMessageFormatter(new Type[] { typeof(Command) })
            };

            // Configuração da fila de mensagens do cliente
            MessageQueue clientMsPath = new MessageQueue(msQueuePath)
            {
                Formatter = new XmlMessageFormatter(new Type[] { typeof(string) })
            };

            try
            {
                Console.WriteLine("Sending Menssage");
                // Envia a mensagem para o servidor
                Message ms = new Message(command);
                messageQueue.Send(ms);

                Console.WriteLine("Receiving Menssage");
                // Recebe a resposta do servidor
                Message clientMsg = clientMsPath.Receive();
                string answer = (string)clientMsg.Body;
                clientMsPath.Close();
                messageQueue.Close();

                Console.WriteLine(answer);

            }
            catch (Exception e)
            {
                Console.WriteLine("error: " + e.Message);
            }

            // Deleta a fila do cliente se existir
            if (MessageQueue.Exists(msQueuePath))
            {
                MessageQueue.Delete(msQueuePath);
            }
        }

        // Função para criar a fila se não existir
        static void CreateQueue()
        {
            if (!MessageQueue.Exists(msQueuePath))
            {
                MessageQueue.Create(msQueuePath);
            }
        }
    }
}
