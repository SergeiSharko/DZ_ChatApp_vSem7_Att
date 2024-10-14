using DZ_ChatApp_vSem7.ChatCommonLib;

namespace DZ_ChatApp_vSem7.CommonLib
{
    public interface IMessageSourceClient
    {
        public void Send(MessageUDP message);
        public MessageUDP ReceiveMessage();

    }
}
