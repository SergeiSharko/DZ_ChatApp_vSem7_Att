using DZ_ChatApp_vSem7.ChatCommonLib;
using DZ_ChatApp_vSem7.ChatNetworks;
using DZ_ChatApp_vSem7.CommonLib;

namespace DZ_ChatApp_vSem7.ChatProject
{
    public class Client
    {
        private readonly string _name;
        private readonly IMessageSourceClient _messageSource;


        public Client(string name, string ipAdress, int port)
        {
            _name = name;
            _messageSource = new MessageSourceClient(ipAdress, port);
        }

        public void ClientSender()
        {
            Register();
            Console.WriteLine($"Пользователь {_name} зарегистрирован в чате поумолчанию.");

            while (true)
            {
                try
                {
                    string? toName = GetCorrectInput("Введите адресата сообщения => ");
                    string? contentText = GetCorrectInput("Введите сообщение или <Exit> для выхода из клиента => ");
                    if (contentText!.ToLower().Equals("exit"))
                    {
                        var exitMessage = new MessageUDP { Command = Command.Exit, FromName = _name };
                        _messageSource.Send(exitMessage);
                        return;
                    }

                    var message = new MessageUDP() { Command = Command.Message, FromName = _name, ToName = toName!, Date = DateTime.Now, Text = contentText };
                    _messageSource.Send(message);
                    Console.WriteLine("Сообщение отправлено.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при обработке сообщения: " + ex.Message);
                }
            }
        }

        public void ClientListener()
        {
            Console.WriteLine("UDP Клиент запущен...");

            while (true)
            {
                MessageUDP? receiveMessage = _messageSource.ReceiveMessage();
                if (receiveMessage != null)
                {
                    PrintingMessagesWithOffset(receiveMessage!.ToString());
                    Confirm(receiveMessage!);
                }
            }
        }


        public void Register()
        {
            var regMessage = new MessageUDP() { Command = Command.Register, FromName = _name, Date = DateTime.Now };
            _messageSource.Send(regMessage);
        }


        public void Confirm(MessageUDP message)
        {
            var confirmMessage = new MessageUDP { Command = Command.Confirmation, FromName = message.FromName, ToName = message.ToName, Date = DateTime.Now, Id = message.Id };
            _messageSource.Send(confirmMessage);
        }

        public void StartClient()
        {
            new Thread(() => ClientListener()).Start();
            ClientSender();
        }

        private static bool IsCorrectInput(string input)
        {
            return !string.IsNullOrEmpty(input) && !string.IsNullOrWhiteSpace(input);
        }


        private static string GetCorrectInput(string message)
        {
            Console.Write(message);
            string? result = Console.ReadLine();
            while (!IsCorrectInput(result!))
            {
                Console.WriteLine("Некорретный ввод. Пробуйте ещё раз");
                Console.Write(message);
                result = Console.ReadLine();
            }
            return result!;
        }

        private static void PrintingMessagesWithOffset(string message)
        {
            if (OperatingSystem.IsWindows())
            {
                var position = Console.GetCursorPosition(); // получаем текущую позицию курсора
                int left = position.Left;   // смещение в символах относительно левого края
                int top = position.Top;     // смещение в строках относительно верха                                                
                Console.MoveBufferArea(0, top, left, 1, 0, top + 1); // копируем ранее введенные символы в строке на следующую строку                    
                Console.SetCursorPosition(0, top); // устанавливаем курсор в начало текущей строки
                Console.WriteLine(message); // в текущей строке выводит полученное сообщение
                Console.SetCursorPosition(left, top + 1);// переносим курсор на следующую строку и продолжаем ввод
            }
            else Console.WriteLine(message);
        }

    }
}
