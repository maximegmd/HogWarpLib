using HogWarp.Lib;
using HogWarp.Lib.Game;
using HogWarp.Lib.Game.Data;
using HogWarp.Lib.System;
using Newtonsoft.Json;
using System.Diagnostics;
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

        struct Leaderboard
        {
            public string Name;
            public Dictionary<string, FTimespan> PlayerTime;
        }

        struct RaceSetups
        {
            public string Name;
            public List<Player> Players;
            public Dictionary<Player, FTimespan> PlayerTimes;
        }

        public string racesFilePath = "plugins\\BroomRacing\\races.json";
        public string leaderboardFilePath = "plugins\\BroomRacing\\leaderboards.json";

        List<RaceRings> races = new List<RaceRings>();
        List<RaceSetups> activeRaces = new List<RaceSetups>();
        List<Leaderboard> leaderboards = new List<Leaderboard>();

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
            Console.WriteLine("Player Left!");
            // Remove player from all active Races
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
                Console.WriteLine($"Join Race: {split[1]}");

                int raceIndex = races.FindIndex(Race => Race.Name == split[1]);

                if (raceIndex < 0)
                    Console.WriteLine($"Could not find race");
                else
                    SetupRace(player, raceIndex);

                cancel = true;
            }
            else if (message == "/startrace")
            {
                int activeRaceIndex = activeRaces.FindIndex(Race => Race.Players[0].ToString() == player.ToString());

                if (activeRaceIndex != -1)
                    SpawnRace(activeRaceIndex);
                else
                    Console.WriteLine($"Race not Found / not race Host");

                cancel = true;
            }
            Console.WriteLine($"Chat: {message}");
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

                Console.WriteLine($"Saving Race: {currentRace.Name}");
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
            Console.WriteLine("Loading Races...");

            if (File.Exists(racesFilePath))
            {
                races = JsonConvert.DeserializeObject<List<RaceRings>>(File.ReadAllText(racesFilePath))!;
                Console.WriteLine($"Loaded {races.Count} races");
            }
            else
            {
                Console.WriteLine("No races exist!");
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

            Console.WriteLine($"Sending Races...");
            _server!.PlayerManager.SendTo(player, Name, 33, writer);
        }

        private void SetupRace(Player player, int selectedRace)
        {
            var race = races[selectedRace];

            Console.WriteLine($"Setting up Race: {race.Name}");

            int raceIndex = activeRaces.FindIndex(ActiveRace => ActiveRace.Name == race.Name);

            if (raceIndex == -1)
            {
                RaceSetups raceSetup = new RaceSetups();
                raceSetup.Name = race.Name;

                if (raceSetup.Players == null)
                    raceSetup.Players = new List<Player>();

                raceSetup.Players.Add(player);

                activeRaces.Add(raceSetup);
            }
            else
            {
                int playerIndex = activeRaces.FindIndex(Race => Race.Players[0].ToString() == player.ToString());

                if (playerIndex != -1)
                    Console.WriteLine($"Player already in race");
                else
                {
                    Console.WriteLine($"Race exists, adding player...");
                    activeRaces[raceIndex].Players.Add(player);
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

            Console.WriteLine($"Building Race");

            for (int i = 0; i < currentRace.Rings.Length; ++i)
            {
                writer.Write(currentRace.Rings[i]);
            }

            foreach (var racePlayer in activeRaces[activeRaceIndex].Players)
            {
                _server!.PlayerManager.SendTo(racePlayer, Name, 32, writer);
                Console.WriteLine($"Sending {races[raceIndex].Name} to Player with {races[raceIndex].Rings.Length} rings");
            }
        }

        private void AddRaceTime(Player player, string raceName, FTimespan raceTime)
        {
            Console.WriteLine($"Race Index: {raceName}, Race Time: {raceTime.Minutes}:{raceTime.Seconds}:{raceTime.Milliseconds / 10}");

            int activeRaceIndex = activeRaces.FindIndex(ActiveRace => ActiveRace.Name == raceName);

            if (activeRaceIndex != -1)
            {
                var raceTimes = activeRaces[activeRaceIndex].PlayerTimes;

                if (raceTimes == null)
                    raceTimes = new Dictionary<Player, FTimespan>();

                raceTimes.Add(player, raceTime);

                if (activeRaces[activeRaceIndex].Players.Count == raceTimes.Count)
                {
                    // End the Race & Print the scores!
                    Console.WriteLine($"All Players Finished!");

                    var times = raceTimes.OrderByDescending(pair => pair.Value).ToList();

                    foreach (var playerTimes in times)
                    {
                        // List Times from Race
                        Console.WriteLine($"{playerTimes.Key.ToString()} - {playerTimes.Value.Minutes}:{playerTimes.Value.Seconds}:{playerTimes.Value.Milliseconds / 10}");
                    }

                    activeRaces.RemoveAt(activeRaceIndex);
                }
            }
        }
    }
}