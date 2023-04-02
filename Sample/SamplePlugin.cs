using HogWarp.Lib;
using HogWarp.Lib.Events;
using HogWarp.Lib.System;

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

        public void ProcessEvent(ClientMessage clientMessage)
        {
            var reader = new BufferReader(clientMessage.Message);
            Console.WriteLine($"Received packet with opcode {clientMessage.Opcode}");
            if(clientMessage.Opcode == 42)
            {
                Console.WriteLine($"Ping: {reader.ReadVarInt()}");
            }
        }
    }
}