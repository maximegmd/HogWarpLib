using System.Runtime.InteropServices;

namespace HogWarp.Lib.System
{
    public unsafe class Buffer
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Parameters
        {
            internal IntPtr Allocate;
            internal IntPtr Free;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct Internal
        {
            [FieldOffset(0)] public IntPtr Data;
            [FieldOffset(8)] public ulong Size;
        }
        internal Internal* Address;
        private bool _disposable = false;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate Internal* AllocateDelegate(ulong length);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void FreeDelegate(Internal* address);

        private static AllocateDelegate? allocateDelegate;
        private static FreeDelegate? freeDelegate;

        public static void Initialize(Parameters parameters)
        {
            allocateDelegate = (AllocateDelegate)Marshal.GetDelegateForFunctionPointer(parameters.Allocate, typeof(AllocateDelegate));
            freeDelegate = (FreeDelegate)Marshal.GetDelegateForFunctionPointer(parameters.Free, typeof(FreeDelegate));
        }

        private Buffer()
        {

        }

        public static Buffer FromAddress(IntPtr address)
        {
            var buffer = new Buffer();
            buffer.Address = (Internal*)address;

            return buffer;
        }

        public Buffer(ulong size)
        {
            _disposable = true;
            Address = allocateDelegate!(size);

        }

        public void Dispose()
        {
            if (_disposable)
                freeDelegate!(Address);
        }
    }
}
