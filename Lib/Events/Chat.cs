using HogWarp.Lib.Game;

namespace HogWarp.Lib.Events
{
    public class Chat
    {
        public Player Player { get; private set; }
        public string Message {  get; private set; }

        public Chat(Player player, string message)
        {
            Player = player;
            Message = message;
        }
    }
}
