using System.Numerics;
using System;
using System.Runtime.InteropServices;
using System.Text;
using SBuffer = System.Buffer;

namespace HogWarp.Lib.System
{
    public unsafe class BufferWriter
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Parameters
        {
            internal IntPtr WriteBits;
            internal IntPtr WriteBytes;
            internal IntPtr WriteBool;
            internal IntPtr WriteFloat;
            internal IntPtr WriteVarInt;
            internal IntPtr WriteDouble;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct Internal
        {
            [FieldOffset(0)] public ulong Position;
            [FieldOffset(8)] public Buffer.Internal* Buffer;
        }

        private Internal _internal;
        private Buffer _buffer;

        public IntPtr Address 
        { 
            get
            {
                fixed (Internal* ptr = &_internal)
                    return (IntPtr)ptr;
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool WriteBytesDelegate(Internal* buffer, [In, Out][MarshalAs(UnmanagedType.LPArray)] byte[] data, ulong length);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool WriteBitsDelegate(Internal* buffer, ulong value, ulong length);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void WriteBoolDelegate(Internal* buffer, bool value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void WriteFloatDelegate(Internal* buffer, float value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void WriteVarIntDelegate(Internal* buffer, ulong value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void WriteDoubleDelegate(Internal* buffer, double value);

        static WriteBytesDelegate? writeBytesDelegate;
        static WriteBitsDelegate? writeBitsDelegate;
        static WriteBoolDelegate? writeBoolDelegate;
        static WriteFloatDelegate? writeFloatDelegate;
        static WriteVarIntDelegate? writeVarIntDelegate;
        static WriteDoubleDelegate? writeDoubleDelegate;


        public static void Initialize(Parameters bufferParameters)
        {
            writeBytesDelegate = (WriteBytesDelegate)Marshal.GetDelegateForFunctionPointer(bufferParameters.WriteBytes, typeof(WriteBytesDelegate));
            writeBitsDelegate = (WriteBitsDelegate)Marshal.GetDelegateForFunctionPointer(bufferParameters.WriteBits, typeof(WriteBitsDelegate));
            writeBoolDelegate = (WriteBoolDelegate)Marshal.GetDelegateForFunctionPointer(bufferParameters.WriteBool, typeof(WriteBoolDelegate));
            writeFloatDelegate = (WriteFloatDelegate)Marshal.GetDelegateForFunctionPointer(bufferParameters.WriteFloat, typeof(WriteFloatDelegate));
            writeVarIntDelegate = (WriteVarIntDelegate)Marshal.GetDelegateForFunctionPointer(bufferParameters.WriteVarInt, typeof(WriteVarIntDelegate));
            writeDoubleDelegate = (WriteDoubleDelegate)Marshal.GetDelegateForFunctionPointer(bufferParameters.WriteDouble, typeof(WriteDoubleDelegate));
        }

        public BufferWriter(Buffer buffer)
        {
            // Keep of ref counting
            _buffer = buffer;

            _internal.Buffer = _buffer.Address;
            _internal.Position = 0;
        }

        public bool WriteBits(ulong value, ulong length)
        {
            fixed (Internal* ptr = &_internal)
                return writeBitsDelegate!(ptr, value, length);
        }

        public void Write<T>(T value) where T : IBinaryInteger<T>
        {
            fixed (Internal* ptr = &_internal)
            {
                var data = new byte[value.GetByteCount()];
                value.WriteLittleEndian(data);
                Write(data);
            }
        }

        public bool Write(byte[] data)
        {
            fixed (Internal* ptr = &_internal)
                return writeBytesDelegate!(ptr, data, (ulong)data.LongLength);
        }

        public void Write(bool value)
        {
            fixed (Internal* ptr = &_internal)
                writeBoolDelegate!(ptr, value);
        }

        public void WriteVarInt(ulong value)
        {
            fixed (Internal* ptr = &_internal)
                writeVarIntDelegate!(ptr, value);
        }

        public void Write(float value)
        {
            fixed (Internal* ptr = &_internal)
                writeFloatDelegate!(ptr, value);
        }

        public void Write(double value)
        {
            fixed (Internal* ptr = &_internal)
                writeDoubleDelegate!(ptr, value);
        }

        public void WriteString(string value)
        {
            byte[] utfBytes = Encoding.UTF8.GetBytes(value);
            WriteVarInt((ulong)utfBytes.LongLength & 0xFFFF);
            Write(utfBytes);
        }
    }
}
