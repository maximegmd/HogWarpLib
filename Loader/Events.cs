using HogWarp.Lib.Game;
using System.Runtime.InteropServices;

namespace HogWarp.Loader
{
    public class Events
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct InitializationParameters
        {
            internal IntPtr WorldAddress;
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
    }
}
