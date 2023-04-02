using HogWarp.Lib.Game;
using HogWarp.Lib.System;
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
            internal BufferReader.Parameters ReaderParameters;
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
            public IntPtr Plugin;
            public IntPtr Message;
            public ushort Opcode;
        }
    }
}
