using System.Runtime.InteropServices;

namespace HogWarp.Lib.System
{
    public unsafe class Buffer
    {
        [StructLayout(LayoutKind.Explicit)]
        internal struct Internal
        {
            [FieldOffset(0)] public IntPtr Data;
            [FieldOffset(8)] public ulong Size;
        }
        internal Internal* Address;

        public Buffer(IntPtr address)
        {
            Address = (Internal*)address;
        }
    }
}
