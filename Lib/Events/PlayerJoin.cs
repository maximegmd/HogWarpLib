using HogWarp.Lib.Game;

namespace HogWarp.Lib.Events
{
    public class PlayerJoin
    {
        public Player Player { get; private set; }

        public PlayerJoin(Player player)
        {
            Player = player;
        }
    }
}
