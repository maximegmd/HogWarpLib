using System.Runtime.InteropServices;
using System.Text;

namespace HogWarp.Lib.System
{
    public unsafe class BufferReader
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Parameters
        {
            internal IntPtr ReadBits;
            internal IntPtr ReadBytes;
            internal IntPtr ReadBool;
            internal IntPtr ReadFloat;
            internal IntPtr ReadVarInt;
            internal IntPtr ReadDouble;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct Internal
        {
            [FieldOffset(0)] public ulong Position;
            [FieldOffset(8)] public Buffer.Internal* Buffer;
        }

        private Internal _internal;
        private Buffer _buffer;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool ReadBytesDelegate(Internal* buffer, [In, Out][MarshalAs(UnmanagedType.LPArray)] byte[] data, ulong length);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool ReadBitsDelegate(Internal* buffer, out ulong value, ulong length);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool ReadBoolDelegate(Internal* buffer);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate float ReadFloatDelegate(Internal* buffer);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate ulong ReadVarIntDelegate(Internal* buffer);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate double ReadDoubleDelegate(Internal* buffer);

        static ReadBytesDelegate? readBytesDelegate;
        static ReadBitsDelegate? readBitsDelegate;
        static ReadBoolDelegate? readBoolDelegate;
        static ReadFloatDelegate? readFloatDelegate;
        static ReadVarIntDelegate? readVarIntDelegate;
        static ReadDoubleDelegate? readDoubleDelegate;

        public static void Initialize(Parameters bufferParameters)
        {
            readBytesDelegate = (ReadBytesDelegate)Marshal.GetDelegateForFunctionPointer(bufferParameters.ReadBytes, typeof(ReadBytesDelegate));
            readBitsDelegate = (ReadBitsDelegate)Marshal.GetDelegateForFunctionPointer(bufferParameters.ReadBits, typeof(ReadBitsDelegate));
            readBoolDelegate = (ReadBoolDelegate)Marshal.GetDelegateForFunctionPointer(bufferParameters.ReadBool, typeof(ReadBoolDelegate));
            readFloatDelegate = (ReadFloatDelegate)Marshal.GetDelegateForFunctionPointer(bufferParameters.ReadFloat, typeof(ReadFloatDelegate));
            readVarIntDelegate = (ReadVarIntDelegate)Marshal.GetDelegateForFunctionPointer(bufferParameters.ReadVarInt, typeof(ReadVarIntDelegate));
            readDoubleDelegate = (ReadDoubleDelegate)Marshal.GetDelegateForFunctionPointer(bufferParameters.ReadDouble, typeof(ReadDoubleDelegate));
        }

        public BufferReader(Buffer buffer)
        {
            // Keep of ref counting
            _buffer = buffer;

            _internal.Buffer = _buffer.Address;
            _internal.Position = 0;
        }

        public bool ReadBits(out ulong value, ulong length)
        {
            fixed (Internal* ptr = &_internal)
                return readBitsDelegate!(ptr, out value, length);
        }

        public bool ReadBytes(byte[] data)
        {
            fixed (Internal* ptr = &_internal)
                return readBytesDelegate!(ptr, data, (ulong)data.LongLength);
        }

        public bool ReadBool()
        {
            fixed (Internal* ptr = &_internal)
                return readBoolDelegate!(ptr);
        }

        public float ReadFloat()
        {
            fixed (Internal* ptr = &_internal)
                return readFloatDelegate!(ptr);
        }

        public ulong ReadVarInt()
        {
            fixed (Internal* ptr = &_internal)
                return readVarIntDelegate!(ptr);
        }

        public double ReadDouble()
        {
            fixed (Internal* ptr = &_internal)
                return readDoubleDelegate!(ptr);
        }

        public string ReadString()
        {
            var length = ReadVarInt() & 0xFFFF;
            var data = new byte[length];
            ReadBytes(data);

            return Encoding.UTF8.GetString(data, 0, data.Length);
        }
    }
}
