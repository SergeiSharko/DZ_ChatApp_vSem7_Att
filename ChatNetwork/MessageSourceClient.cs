using System.Text;
using NetMQ;
using NetMQ.Sockets;
using DZ_ChatApp_vSem7.ChatCommonLib;
using DZ_ChatApp_vSem7.CommonLib;

namespace DZ_ChatApp_vSem7.ChatNetworks
{
    public class MessageSourceClient : IMessageSourceClient
    {

        private readonly DealerSocket _dealerSocket;

        public MessageSourceClient(string ipAdress, int port)
        {
            _dealerSocket = new DealerSocket();
            _dealerSocket.Connect($"tcp://{ipAdress}:{port}");

        }

        public MessageUDP ReceiveMessage()
        {
            var messageReceived = _dealerSocket.ReceiveFrameString();
            var message = MessageUDP.FromJson(messageReceived) ?? new MessageUDP();

            return message;
        }

        public void Send(MessageUDP message)
        {
            _dealerSocket.SendFrame(Encoding.UTF8.GetBytes(message.ToJson()));
        }
    }
}
