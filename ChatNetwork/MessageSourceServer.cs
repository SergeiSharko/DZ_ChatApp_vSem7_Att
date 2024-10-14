using System.Text;
using NetMQ.Sockets;
using NetMQ;
using DZ_ChatApp_vSem7.ChatCommonLib;
using DZ_ChatApp_vSem7.CommonLib;

namespace DZ_ChatApp_vSem7.ChatNetwork
{
    public class MessageSourceServer : IMessageSource
    {
        private readonly RouterSocket _routerSocket = new();


        public MessageSourceServer()
        {
            _routerSocket.Bind($"tcp://*:{12345}");
        }

        public MessageUDP ReceiveMessage(ref string? clientID)
        {
            // Получаем само сообщение
            string messageReceived = _routerSocket.ReceiveFrameString();
            return MessageUDP.FromJson(messageReceived) ?? new MessageUDP();
        }

        public void Send(MessageUDP message, string clientId)
        {
            // Отправляем идентификатор клиента
            _routerSocket.SendMoreFrame(clientId);

            // Отправляем разделитель (обычно пустая строка)
            _routerSocket.SendMoreFrame(""); // Пустой фрейм для разделения метаданных

            // Отправляем само сообщение
            _routerSocket.SendFrame(Encoding.UTF8.GetBytes(message.ToJson()));
        }

    }
}
