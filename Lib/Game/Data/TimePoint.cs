using System.Runtime.InteropServices;

namespace HogWarp.Lib.Game.Data
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TimePoint
    {
        public ulong Tick;
        public Movement Move;
    }
}
