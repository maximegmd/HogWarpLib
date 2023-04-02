using HogWarp.Lib.Game;

namespace HogWarp.Lib.Events
{
    public class PlayerLeave
    {
        public Player Player { get; private set; }

        public PlayerLeave(Player player)
        {
            Player = player;
        }
    }
}
