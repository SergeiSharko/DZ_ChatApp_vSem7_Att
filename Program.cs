using DZ_ChatApp_vSem7.ChatNetwork;
using DZ_ChatApp_vSem7.ChatProject;

namespace DZ_ChatApp_vSem7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                var server = new Server(new MessageSourceServer());
                server.Work();
            }
            else if (args.Length == 3) //string _name , string ipAdress, string port
            {   
                int port = int.Parse(args[2]);
                var client = new Client(args[0], args[1], port);
                client.StartClient();
            }
            else
            {
                //Console.WriteLine("Для запуска сервера введите ник-нейм как параметр запуска приложения");
                Console.WriteLine("Для запуска клиента введите <Имя>, <IP-сервера> и <Номер порта>, как параметры запуска приложения");
            }
        }
    }
}
