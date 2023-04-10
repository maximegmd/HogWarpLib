using HogWarp.Lib.Interop.Attributes;
using HogWarp.Lib.System;

namespace HogWarp.Lib.Game
{
    public unsafe partial class PlayerManager
    {
        private Dictionary<nint, Player> _players = new Dictionary<nint, Player>();

        public IEnumerable<Player> Players => _players.Values;

        [Function]
        private partial void SendModMessage(nint player, string mod, ushort opcode, nint ptr);

        [Function]
        private partial void BroadcastModMessage(string mod, ushort opcode, nint ptr);

        private nint Address;

        internal PlayerManager(nint address)
        {
            Address = address;
        }

        public void SendTo(Player player, string mod, ushort opcode, BufferWriter data)
        {
            SendModMessage((nint)player.Address, mod, opcode, data.Address);
        }

        public void Broadcast(string mod, ushort opcode, BufferWriter data)
        {
            BroadcastModMessage(mod, opcode, data.Address);
        }

        internal void Add(Player player)
        {
            _players.Add((nint)player.Address, player);
        }

        internal Player? Remove(nint address)
        {
            var player = GetByPtr(address);
            if (player != null)
                _players.Remove(address);

            return player;
        }

        internal Player? GetByPtr(nint address)
        {
            if (_players.TryGetValue(address, out var player))
                return player;

            return null;
        }
    }
}
