using HogWarp.Lib.Events;
using HogWarp.Lib.Game;
using System.Runtime.InteropServices;
using static HogWarp.Loader.PluginManager;
using static HogWarp.Loader.Events;
using HogWarp.Lib;

namespace HogWarp.Loader
{
    public static class EntryPoint
    {
        private static Server? _server;

        [UnmanagedCallersOnly]
        public static void Initialize(InitializationParameters Params)
        {
            Player.Initialize(Params.PlayerFunctionParameters);

            var world = new World(Params.WorldAddress);
            _server = new Server(world);

            LoadFromBase("plugins");

            InitializePlugins(_server);
        }

        [UnmanagedCallersOnly]
        public static void Shutdown(ShutdownArgs args)
        {
            Dispatch(new Shutdown());
        }

        [UnmanagedCallersOnly]
        public static void Update(UpdateArgs args)
        {
            Dispatch(new Update(args.Delta));
        }

        [UnmanagedCallersOnly]
        public static void OnPlayerJoined(PlayerArgs args)
        {
            Dispatch(new PlayerJoin(new Player(args.Ptr)));
        }

        [UnmanagedCallersOnly]
        public static void OnPlayerLeft(PlayerArgs args)
        {
            Dispatch(new PlayerLeave(new Player(args.Ptr)));
        }

        [UnmanagedCallersOnly]
        public static void OnPlayerChat(ChatArgs args)
        {
            Console.WriteLine("Chat");
        }

        [UnmanagedCallersOnly]
        public static void OnMessage(MessageArgs args)
        {
            Console.WriteLine("Message");
        }
    }
}