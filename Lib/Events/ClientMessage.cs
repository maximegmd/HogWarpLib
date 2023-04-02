using HogWarp.Lib.Game;

namespace HogWarp.Lib.Events
{
    public class ClientMessage
    {
        public Player Player { get; private set; }
        public System.Buffer Message { get; private set; }
        public ushort Opcode { get; private set; }

        public ClientMessage(Player player, System.Buffer message, ushort opcode)
        {
            Player = player;
            Message = message;
            Opcode = opcode;
        }
    }
}
