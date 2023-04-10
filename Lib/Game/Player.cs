using HogWarp.Lib.Interop.Attributes;
using System.Runtime.InteropServices;
using System.Text;

namespace HogWarp.Lib.Game
{
    public unsafe partial class Player : Character
    {
        [Function]
        private partial nint GetDiscordId();

        [Function]
        private partial nint GetName();

        [Function]
        public partial void Kick();

        [Function(Generate = false)]
        private partial void SendMessage(byte[] data, ulong length);

        public string DiscordId { get; private set; }
        public string Name { get; private set; }

        internal Player(IntPtr Address)
            : base(Address)
        {
            DiscordId = Marshal.PtrToStringUTF8(GetDiscordId())!;
            Name = Marshal.PtrToStringUTF8(GetName())!;
        }

        public void SendMessage(string data)
        {
            var d = Encoding.UTF8.GetBytes(data);
            SendMessage(d, (ulong)d.Length);
        }

        private partial void SendMessage(byte[] data, ulong length)
        {
            SendMessageInternal((IntPtr)Address, data, length);
        }
    }
}
