using DZ_ChatApp_vSem7.ChatCommonLib;

namespace DZ_ChatApp_vSem7.CommonLib
{
    public interface IMessageSource
    {
        void Send(MessageUDP message, string clientId);
        MessageUDP? ReceiveMessage(ref string? clientId);
    }
}
