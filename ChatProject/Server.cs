using System.Net;
using System.Text;
using DZ_ChatApp_vSem7.ChatCommonLib;
using DZ_ChatApp_vSem7.ChatProject.Models;
using DZ_ChatApp_vSem7.CommonLib;
using Message = DZ_ChatApp_vSem7.ChatProject.Models.Message;

namespace DZ_ChatApp_vSem7.ChatProject
{
    public class Server
    {
        private readonly IMessageSource _messageSource;
        private static Dictionary<string, string> clients = [];
        bool work = true;

        public Server(IMessageSource source)
        {
            _messageSource = source;
        }


        public void Register(MessageUDP message, string clientId)
        {
            if (clients.TryAdd(message.FromName ?? "Unknown", clientId))
            {
                using (var ctx = new ChatUdpContext())
                {
                    if (ctx.Users.FirstOrDefault(x => x.Name == message.FromName) != null)
                    {
                        Console.WriteLine($"Пользователь {message.FromName} уже зарегистрирован в чате");
                        return;
                    }
                    else
                    {
                        ctx.Add(new User { Name = message.FromName ?? "Unknown" });
                        ctx.SaveChanges();
                        Console.WriteLine($"Пользователь {message.FromName} зарегистрирован в чате");
                    }
                }
            }

        }


        public void ConfirmMessageReceived(int? id)
        {
            using (var ctx = new ChatUdpContext())
            {
                var msg = ctx.Messages.FirstOrDefault(x => x.Id == id);
                if (msg != null)
                {
                    msg.isReceived = true;
                    ctx.SaveChanges();
                }
            }
        }


        public void ReplyMessage(MessageUDP message)
        {
            int? id = null;
            if (clients.TryGetValue(message.ToName ?? "Unknown", out string? clientId))
            {
                using (var ctx = new ChatUdpContext())
                {
                    var fromUser = ctx.Users.FirstOrDefault(x => x.Name == message.FromName);
                    var toUser = ctx.Users.FirstOrDefault(y => y.Name == message.ToName);
                    var msg = new Message
                    {
                        FromUserId = fromUser!.Id,
                        ToUserId = toUser!.Id,
                        isReceived = false,
                        Text = message.Text,
                    };

                    ctx.Messages.Add(msg);
                    ctx.SaveChanges();
                    id = msg.Id;
                }

                var forwardMessage = new MessageUDP()
                {
                    Id = id,
                    Command = Command.Message,
                    ToName = message.ToName,
                    FromName = message.FromName,
                    Text = message.Text
                };

                byte[] forwardBytes = Encoding.UTF8.GetBytes(forwardMessage.ToJson());
                _messageSource.Send(forwardMessage, clientId);
            }
            else
            {
                Console.WriteLine("Пользователь не найден");
            }
        }


        public void ProcessMessage(MessageUDP message, string fromId)
        {
            if (message.Command == Command.Register)
            {
                Register(message, fromId);
            }
            if (message.Command == Command.Message)
            {
                ReplyMessage(message);
            }
            if (message.Command == Command.Confirmation)
            {
                ConfirmMessageReceived(message.Id);
                Console.WriteLine($"Подтвержденное сообщение, от = {message.FromName} для = {message.ToName}, ему назначено id = {message.Id}");
            }
        }

        public void Work()
        {

            Console.WriteLine("UDP Сервер ожидает сообщений...");
            Console.WriteLine("Нажмите любую клавишу для завершения работы сервера!");
            

            while (work)
            {
                try
                {
                    string? clientId = null;
                    var receiveMessage = _messageSource.ReceiveMessage(ref clientId);
                    if (receiveMessage!.Command == Command.Exit || receiveMessage == null || clientId == null) return;
                    else
                    {
                        ProcessMessage(receiveMessage!, clientId);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при обработке сообщения: " + ex.Message);
                }
            }
        }

        public void Stop()
        {
            work = false;
        }

    }
}