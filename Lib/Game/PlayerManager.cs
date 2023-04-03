using HogWarp.Lib.Interop.Attributes;
using HogWarp.Lib.System;

namespace HogWarp.Lib.Game
{
    public unsafe partial class PlayerManager
    {
        [Function]
        private partial void SendModMessage(IntPtr player, string mod, ushort opcode, IntPtr ptr);

        [Function]
        private partial void BroadcastModMessage(string mod, ushort opcode, IntPtr ptr);

        private IntPtr Address;

        internal PlayerManager(IntPtr address)
        {
            Address = address;
        }

        public void SendTo(Player player, string mod, ushort opcode, BufferWriter data)
        {
            SendModMessage((IntPtr)player.Address, mod, opcode, data.Address);
        }

        public void Broadcast(string mod, ushort opcode, BufferWriter data)
        {
            BroadcastModMessage(mod, opcode, data.Address);
        }
    }
}
