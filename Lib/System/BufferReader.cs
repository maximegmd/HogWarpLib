using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using SBuffer = System.Buffer;

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
            // Keep for ref counting
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

        public void Read(out bool value)
        {
            fixed (Internal* ptr = &_internal)
                value = readBoolDelegate!(ptr);
        }

        public void Read(out byte value)
        {
            ReadBits(out var tmp, sizeof(byte) * 8);
            value = (byte)tmp;
        }

        public void Read(out ushort value)
        {
            ReadBits(out var tmp, sizeof(ushort) * 8);
            var data = BitConverter.GetBytes((ushort)tmp);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(data);

            value = BitConverter.ToUInt16(data);
        }

        public void Read(out uint value)
        {
            ReadBits(out var tmp, sizeof(uint) * 8);
            var data = BitConverter.GetBytes((uint)tmp);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(data);

            value = BitConverter.ToUInt32(data);
        }

        public void Read(out ulong value)
        {
            ReadBits(out var tmp, sizeof(ulong) * 8);
            var data = BitConverter.GetBytes(tmp);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(data);

            value = BitConverter.ToUInt64(data);
        }

        public void Read(out short value)
        {
            ReadBits(out var tmp, sizeof(ushort) * 8);
            var data = BitConverter.GetBytes((ushort)tmp);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(data);

            value = BitConverter.ToInt16(data);
        }

        public void Read(out int value)
        {
            ReadBits(out var tmp, sizeof(int) * 8);
            var data = BitConverter.GetBytes((uint)tmp);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(data);

            value = BitConverter.ToInt32(data);
        }

        public void Read(out long value)
        {
            ReadBits(out var tmp, sizeof(long) * 8);
            var data = BitConverter.GetBytes((ulong)tmp);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(data);

            value = BitConverter.ToInt64(data);
        }


        public void Read(out float value)
        {
            fixed (Internal* ptr = &_internal)
                value = readFloatDelegate!(ptr);
        }

        public void ReadVarInt(out ulong value)
        {
            fixed (Internal* ptr = &_internal)
                value = readVarIntDelegate!(ptr);
        }

        public void ReadDouble(out double value)
        {
            fixed (Internal* ptr = &_internal)
                value = readDoubleDelegate!(ptr);
        }

        public void Read(out string value)
        {
            ReadVarInt(out var length);
            length &= 0xFFFF;
            var data = new byte[length];
            ReadBytes(data);

            value = Encoding.UTF8.GetString(data, 0, data.Length);
        }
    }
}
