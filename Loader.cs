using HogWarp.Lib.Game;
using System.Runtime.InteropServices;

namespace HogWarp.Lib
{
    public static class Loader
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        static public extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [StructLayout(LayoutKind.Sequential)]
        public struct InitArgs
        {
            public IntPtr World;
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
        public static void Initialize(InitArgs args)
        {
            Console.WriteLine("Initialize");
            var module = LoadLibrary("HogWarpServer.exe");
            if (module == 0)
            {
                module = LoadLibrary("HogWarpServer");
                if (module == 0)
                    return;
            }
            
            Player.Initialize(module);
            World.Initialize(module);

            Server.World = new World(args.World);
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