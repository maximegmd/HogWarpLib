using HogWarp.Lib.Events;
using HogWarp.Lib.Game;
using System.Runtime.InteropServices;
using static HogWarp.Loader.PluginManager;
using static HogWarp.Loader.Events;
using HogWarp.Lib;
using HogWarp.Lib.System;

namespace HogWarp.Loader
{
    public static class EntryPoint
    {
        private static Server? _server;

        [UnmanagedCallersOnly]
        public static void Initialize(InitializationParameters Params)
        {
            Player.Initialize(Params.PlayerFunctionParameters);
            BufferReader.Initialize(Params.ReaderParameters);

            var world = new World(Params.WorldAddress);
            _server = new Server(world);

            LoadFromBase("plugins");

            InitializePlugins(_server);
        }

        [UnmanagedCallersOnly]
        public static void Shutdown(ShutdownArgs args)
        {
            EventProcessor<Shutdown>.Dispatch(new Shutdown());
        }

        [UnmanagedCallersOnly]
        public static void Update(UpdateArgs args)
        {
            EventProcessor<Update>.Dispatch(new Update(args.Delta));
        }

        [UnmanagedCallersOnly]
        public static void OnPlayerJoined(PlayerArgs args)
        {
            EventProcessor<PlayerJoin>.Dispatch(new PlayerJoin(new Player(args.Ptr)));
        }

        [UnmanagedCallersOnly]
        public static void OnPlayerLeft(PlayerArgs args)
        {
            EventProcessor<PlayerLeave>.Dispatch(new PlayerLeave(new Player(args.Ptr)));
        }

        [UnmanagedCallersOnly]
        public static int OnPlayerChat(ChatArgs args)
        {
            string message = Marshal.PtrToStringUTF8(args.Message)!;

            return EventProcessor<Chat>.DispatchCancellable(new Chat(new Player(args.Ptr), message)) ? 1 : 0;
        }

        [UnmanagedCallersOnly]
        public static void OnMessage(MessageArgs args)
        {
            string modName = Marshal.PtrToStringUTF8(args.Plugin)!;

            var buffer = new Lib.System.Buffer(args.Message);
            var msg = new Lib.Events.ClientMessage(new Player(args.Ptr), buffer, args.Opcode);

            EventProcessor<ClientMessage>.DispatchTo(modName, msg);
        }
    }
}