﻿using HogWarp.Lib;
using HogWarp.Lib.Game;
using HogWarp.Lib.System;
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
            _server.UpdateEvent += Update;
            _server.ChatEvent += Chat;
            _server.PlayerJoinEvent += PlayerJoin;
            _server.RegisterMessageHandler(Name, HandleMessage);
        }

        public void Update(float deltaSeconds)
        {
        }

        public void Chat(Player player, string message, ref bool cancel)
        {
            Serilog.Log.Information($"Chat: {message}");
        }

        public void PlayerJoin(Player player)
        {
            Serilog.Log.Information("Player joined!");

            SendPing(player, 0);
        }

        public void HandleMessage(Player player, ushort opcode, Buffer buffer)
        {
            var reader = new BufferReader(buffer);

            if(opcode == 43)
            {
                reader.ReadBits(out var ping, 64);

                Serilog.Log.Information($"Ping: {ping}");

                SendPing(player, ping);
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