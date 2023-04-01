using System.Runtime.InteropServices;

namespace HogWarp.Lib.Interop
{
    internal class Delegates
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        internal delegate int GetintDelegate(IntPtr ptr);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        internal delegate int SetintDelegate(IntPtr ptr, int value);
    }
}
