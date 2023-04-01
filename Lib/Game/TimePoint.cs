using HogWarp.Lib.Game.Data;
using System.Runtime.InteropServices;

namespace HogWarp.Lib.Game
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TimePoint
    {
        public ulong Tick;
        public Movement Move;
    }
}
