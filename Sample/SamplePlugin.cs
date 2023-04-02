using HogWarp.Lib;
using HogWarp.Lib.Events;
using HogWarp.Lib.Game;
using HogWarp.Lib.System;
using System.Net.NetworkInformation;
using Buffer = HogWarp.Lib.System.Buffer;

namespace Sample
{
    public class SamplePlugin : IPluginBase
    {
        public string Name => "Example";

        public string Description => "This plugin shows how to create a basic plugin";

        private Server? _server;

        public void Initialize(Server server)
        {
            _server = server;
        }

        public void ProcessEvent(Update serverEvent)
        {
            //Console.WriteLine($"Update! {serverEvent.DeltaSeconds}, year {_server!.World.Year}");
        }

        public bool ProcessEvent(Chat serverEvent)
        {
            Console.WriteLine($"Chat: {serverEvent.Message}");
            return false;
        }

        public void ProcessEvent(PlayerJoin playerJoin)
        {
            Console.WriteLine("Player joined!");

            SendPing(playerJoin.Player, 0);
        }

        public void ProcessEvent(ClientMessage clientMessage)
        {
            var reader = new BufferReader(clientMessage.Message);

            if(clientMessage.Opcode == 42)
            {
                var ping = reader.ReadVarInt();

                Console.WriteLine($"Ping: {ping}");

                SendPing(clientMessage.Player, ping);
            }
        }

        private void SendPing(Player player, ulong id)
        {
            var buffer = new Buffer(1000);
            var writer = new BufferWriter(buffer);
            writer.Write(id);

            _server!.PlayerManager.SendTo(player, Name, 43, writer);
        }
    }
}