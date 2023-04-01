using HogWarp.Lib.Game;
using System.Runtime.InteropServices;

namespace HogWarp.Lib
{
    public static class Loader
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct InitializationParameters
        {
            internal IntPtr WorldAddress;
            internal World.InitializationVariableParameters WorldVariableParameters;
            internal Player.InitializationFunctionParameters PlayerFunctionParameters;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct ShutdownArgs
        {
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UpdateArgs
        {
            public float Delta;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PlayerArgs
        {
            public IntPtr Ptr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ChatArgs
        {
            public IntPtr Ptr;
            public IntPtr Message;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MessageArgs
        {
            public IntPtr Ptr;
            public IntPtr Message;
        }

        [UnmanagedCallersOnly]
        public static void Initialize(InitializationParameters Params)
        {
            World.Initialize(Params.WorldVariableParameters);
            Player.Initialize(Params.PlayerFunctionParameters);

            Server.World = new World(Params.WorldAddress);
        }

        [UnmanagedCallersOnly]
        public static void Shutdown(ShutdownArgs args)
        {
            Console.WriteLine("Shutdown");
        }

        [UnmanagedCallersOnly]
        public static void Update(UpdateArgs args)
        {
            Console.WriteLine("Update {0}", Server.World.Year);
        }

        [UnmanagedCallersOnly]
        public static void OnPlayerJoined(PlayerArgs args)
        {
            Console.WriteLine("Joined {0}", args.Ptr);
        }

        [UnmanagedCallersOnly]
        public static void OnPlayerLeft(PlayerArgs args)
        {
            Console.WriteLine("Left {0}", args.Ptr);
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