using HogWarp.Lib.Game;

namespace HogWarp.Lib
{
    public class Server
    {
        public World World;
        public PlayerManager PlayerManager;

        public Server(World world, PlayerManager playerManager)
        {
            World = world;
            PlayerManager = playerManager;
        }
    }
}
