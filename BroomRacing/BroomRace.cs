using HogWarp.Lib;
using HogWarp.Lib.Game;
using HogWarp.Lib.Game.Data;
using HogWarp.Lib.System;
using Newtonsoft.Json;
using Buffer = HogWarp.Lib.System.Buffer;

namespace BroomRacing
{
    public class BroomRace : IPluginBase
    {
        public string Name => "RaceBuilder";
        public string Description => "Build Races & Race with others";
        private Server? _server;

        struct RaceRings
        {
            public string Name;
            public FTransform[] Rings;
        }

        struct RaceSetups
        {
            public string Name = "";
            public List<Player> Players = new List<Player>();
            public Dictionary<Player, FTimespan> PlayerTimes = new Dictionary<Player, FTimespan>();

            public RaceSetups()
            {
            }
        }

        public string racesFilePath = Path.Join("plugins", "BroomRacing", "races.json");

        List<RaceRings> races = new List<RaceRings>();
        List<RaceSetups> activeRaces = new List<RaceSetups>();

        public void Initialize(Server server)
        {
            _server = server;
            _server.UpdateEvent += Update;
            _server.ChatEvent += Chat;
            _server.PlayerLeaveEvent += PlayerLeave;
            _server.RegisterMessageHandler(Name, HandleMessage);
            LoadRaces();
        }

        public void Update(float deltaSeconds)
        {
            
        }

        public void PlayerLeave(Player player)
        {
            _server!.Information("Player Left!");
            // Remove player from all active Races
            foreach (var r in activeRaces)
            {
                foreach (var p in r.Players)
                {
                    if(p.DiscordId == player.DiscordId)
                    {
                        r.Players.Remove(p);
                        break;
                    }
                }
            }
        }

        public void Chat(Player player, string message, ref bool cancel)
        {
            if (message == "/racebuilder")
            {
                var buffer = new Buffer(2);
                var writer = new BufferWriter(buffer);
                _server!.PlayerManager.SendTo(player, Name, 34, writer);

                cancel = true;
            }
            else if (message.Contains("/joinrace"))
            {
                var split = message.Split("/joinrace ");
                _server!.Information($"Join Race: {split[1]}");

                int raceIndex = races.FindIndex(Race => Race.Name == split[1]);

                if (raceIndex < 0)
                    player.SendMessage("Could not find race");
                else
                    SetupRace(player, raceIndex);

                cancel = true;
            }
            else if (message == "/startrace")
            {
                int activeRaceIndex = activeRaces.FindIndex(Race => Race.Players[0].DiscordId == player.DiscordId);

                if (activeRaceIndex != -1)
                    SpawnRace(activeRaceIndex);
                else
                    player.SendMessage("Race not found, or you are not the race host.");

                cancel = true;
            }
            _server!.Information($"Chat: {message}");
        }

        public void HandleMessage(Player player, ushort opcode, Buffer buffer)
        {
            var reader = new BufferReader(buffer);

            if(opcode == 32)
            {
                RaceRings currentRace;

                reader.Read(out currentRace.Name);
                reader.ReadVarInt(out var raceSize);
                raceSize &= 0xFFFF;

                currentRace.Rings = new FTransform[raceSize];

                for (int i = 0; i < currentRace.Rings.Length; ++i)
                {
                    reader.Read(out currentRace.Rings[i]);
                } 

                _server!.Information($"Saving Race: {currentRace.Name}");
                races.Add(currentRace);
                SaveRaces();
            }
            else if (opcode == 33)
            {
                SendRaces(player);
            }
            else if (opcode == 34)
            {
                reader.Read(out int selectedRace);
                SetupRace(player, selectedRace);
            }
            else if (opcode == 35)
            {
                reader.Read(out string raceName);
                reader.Read(out FTimespan raceTime);
                AddRaceTime(player, raceName, raceTime);
            }
        }

        public void LoadRaces()
        {
            _server!.Information("Loading Races...");

            if (File.Exists(racesFilePath))
            {
                races = JsonConvert.DeserializeObject<List<RaceRings>>(File.ReadAllText(racesFilePath))!;
                _server!.Information($"Loaded {races.Count} races");
            }
            else
            {
                _server!.Warning("No races exist!");
            }
        }

        private void SaveRaces()
        {
            File.WriteAllText(racesFilePath, JsonConvert.SerializeObject(races, Formatting.Indented));
        }

        private void SendRaces(Player player)
        {
            var buffer = new Buffer(10000);
            var writer = new BufferWriter(buffer);

            writer.WriteVarInt(Convert.ToUInt64(races.Count));

            foreach(var race in races)
            {
                writer.WriteString(race.Name);
            }

            _server!.Information($"Sending Races...");
            _server!.PlayerManager.SendTo(player, Name, 33, writer);
        }

        private void SetupRace(Player player, int selectedRace)
        {
            var race = races[selectedRace];

            _server!.Information($"Setting up Race: {race.Name}");

            int raceIndex = activeRaces.FindIndex(ActiveRace => ActiveRace.Name == race.Name);

            if (raceIndex == -1)
            {
                RaceSetups raceSetup = new RaceSetups();
                raceSetup.Name = race.Name;
                raceSetup.Players.Add(player);

                activeRaces.Add(raceSetup);

                foreach (var serverPlayer in _server!.PlayerManager.Players)
                {
                    if (serverPlayer.DiscordId == player.DiscordId)
                        serverPlayer.SendMessage($"You are race Host type '/startrace' to begin.");
                    else
                        serverPlayer.SendMessage($"{race.Name} has been setup, type '/joinrace {race.Name}' to join.");
                }
            }
            else
            {
                int playerIndex = activeRaces.FindIndex(Race => Race.Players[0].DiscordId == player.DiscordId);

                if (playerIndex != -1)
                    player.SendMessage("You are already in this race.");
                else
                {
                    _server!.Information($"Race exists, adding player...");
                    activeRaces[raceIndex].Players.Add(player);
                    foreach(var racePlayer in activeRaces[raceIndex].Players)
                    {
                        racePlayer.SendMessage($"{player.Name} has joined the race.");
                    }
                }
            }
        }

        private void SpawnRace(int activeRaceIndex)
        {
            var buffer = new Buffer(10000);
            var writer = new BufferWriter(buffer);

            int raceIndex = races.FindIndex(Race => Race.Name == activeRaces[activeRaceIndex].Name);
            var currentRace = races[raceIndex];

            writer.WriteString(races[raceIndex].Name);
            writer.WriteVarInt(Convert.ToUInt64(races[raceIndex].Rings.Length));

            _server!.Information($"Building Race");

            for (int i = 0; i < currentRace.Rings.Length; ++i)
            {
                writer.Write(currentRace.Rings[i]);
            }

            foreach (var racePlayer in activeRaces[activeRaceIndex].Players)
            {
                _server!.PlayerManager.SendTo(racePlayer, Name, 32, writer);
                _server!.Information($"Sending {races[raceIndex].Name} to Player with {races[raceIndex].Rings.Length} rings");
            }
        }

        private void AddRaceTime(Player player, string raceName, FTimespan raceTime)
        {
            _server!.Information($"Race Index: {raceName}, Race Time: {raceTime.Minutes}:{raceTime.Seconds}:{raceTime.Milliseconds / 10}");

            int activeRaceIndex = activeRaces.FindIndex(ActiveRace => ActiveRace.Name == raceName);

            if (activeRaceIndex != -1)
            {
                activeRaces[activeRaceIndex].PlayerTimes.Add(player, raceTime);
                var raceTimes = activeRaces[activeRaceIndex].PlayerTimes;

                if (activeRaces[activeRaceIndex].Players.Count == raceTimes.Count)
                {
                    var times = raceTimes.OrderBy(pair => pair.Value.Days)
                        .ThenBy(pair => pair.Value.Hours)
                        .ThenBy(pair => pair.Value.Minutes)
                        .ThenBy(pair => pair.Value.Seconds)
                        .ThenBy(pair => pair.Value.Milliseconds).ToList();

                    foreach (var racePlayer in activeRaces[activeRaceIndex].Players)
                    {
                        racePlayer.SendMessage("Race Times");
                        foreach (var playerTimes in times)
                        {
                            racePlayer.SendMessage($"{playerTimes.Key.Name} - {playerTimes.Value.Minutes}:{playerTimes.Value.Seconds}:{playerTimes.Value.Milliseconds / 10}");
                        }
                    }
                    activeRaces.RemoveAt(activeRaceIndex);
                }
            }
        }
    }
}