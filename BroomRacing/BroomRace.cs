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
            public string Name;
            public List<Player> Players;
        }

        List<RaceRings> races = new List<RaceRings>();
        public string racesFilePath = "plugins\\BroomRacing\\races.json";

        List<RaceSetups> activeRaces = new List<RaceSetups>(); 

        public void Initialize(Server server)
        {
            _server = server;
            _server.UpdateEvent += Update;
            _server.ChatEvent += Chat;
            _server.RegisterMessageHandler(Name, HandleMessage);
            LoadRaces();
        }

        public void Update(float deltaSeconds)
        {

        }

        public void Chat(Player player, string message, ref bool cancel)
        {
            if (message == "/racebuilder")
            {
                var buffer = new Buffer(2);
                var writer = new BufferWriter(buffer);
                _server!.PlayerManager.SendTo(player, Name, 16, writer);
                cancel = true;
            }
            Console.WriteLine($"Chat: {message}");
        }

        public void HandleMessage(Player player, ushort opcode, Buffer buffer)
        {
            var reader = new BufferReader(buffer);

            if(opcode == 34)
            {
                RaceRings currentRace;

                var raceName = reader.ReadString();
                var raceSize = reader.ReadVarInt();

                currentRace.Name = raceName;
                currentRace.Rings = new FTransform[raceSize - 1];

                int raceRingsIndex = 0;

                foreach(FTransform transform in currentRace.Rings)
                {
                    FTransform currentTransform;

                    currentTransform.Location.X = reader.ReadFloat();
                    currentTransform.Location.Y = reader.ReadFloat();
                    currentTransform.Location.Z = reader.ReadFloat();
                    currentTransform.Rotation.X = reader.ReadFloat();
                    currentTransform.Rotation.Y = reader.ReadFloat();
                    currentTransform.Rotation.Z = reader.ReadFloat();
                    currentTransform.Scale.X = 1;
                    currentTransform.Scale.Y = 1;
                    currentTransform.Scale.Z = 1;

                    currentRace.Rings[raceRingsIndex] = currentTransform;
                    raceRingsIndex += 1;
                } 

                Console.WriteLine($"Saving Race: {raceName}");
                races.Add(currentRace);
                SaveRaces();
            }
            else if (opcode == 64)
            {
                SendRaces(player);
            }
            else if (opcode == 12)
            {
                var selectedRace = reader.ReadVarInt();
                SetupRace(player, Convert.ToInt32(selectedRace));
            }
        }

        private void SendRaces(Player player)
        {
            var buffer = new Buffer(1000);
            var writer = new BufferWriter(buffer);

            ulong racesCount = Convert.ToUInt64(races.Count);

            writer.Write(racesCount);

            foreach(var race in races)
            {
                writer.WriteString(race.Name);
            }

            Console.WriteLine($"Sending Races...");
            _server!.PlayerManager.SendTo(player, Name, 64, writer);
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
                {
                    raceSetup.Players = new List<Player>();
                }

                raceSetup.Players.Add(player);

                activeRaces.Add(raceSetup);
            }
            else
            {
                Console.WriteLine($"Race exists, adding player...");
                activeRaces[raceIndex].Players.Add(player);
            }
        }
    }
}