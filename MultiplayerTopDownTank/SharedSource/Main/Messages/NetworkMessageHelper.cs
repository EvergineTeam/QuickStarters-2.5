using System;
using WaveEngine.Networking;
using WaveEngine.Networking.Messages;

namespace MultiplayerTopDownTank.Messages
{
    public class NetworkMessageHelper
    {
        public static OutgoingMessage CreateMessage(NetworkService networkService, NetworkAgentEnum deliverTo, NetworkCommandEnum command, string parameter1, string parameter2)
        {
            OutgoingMessage responseMessage;

            switch (deliverTo)
            {
                case NetworkAgentEnum.Server:
                    responseMessage = networkService.CreateServerMessage();
                    break;
                case NetworkAgentEnum.Client:
                    responseMessage = networkService.CreateClientMessage();
                    break;
                default:
                    throw new Exception("Agent does not not exists.");
            }

            responseMessage.Write((int)command);
            responseMessage.Write(parameter1);
            responseMessage.Write(parameter2);

            return responseMessage;
        }

        public static void ReadMessage(IncomingMessage incomingMessage, out NetworkCommandEnum command, out string parameter1, out string parameter2)
        {
            command = (NetworkCommandEnum)incomingMessage.ReadInt32();
            parameter1 = incomingMessage.ReadString();
            parameter2 = incomingMessage.ReadString();
        }
    }
}
