using HogWarp.Lib.Game;
using Serilog;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HogWarp.Loader")]

namespace HogWarp.Lib
{
    public delegate void UpdateDelegate(float DeltaSeconds);
    public delegate void ShutdownDelegate();
    public delegate void PlayerJoinDelegate(Player player);
    public delegate void PlayerLeaveDelegate(Player player);
    public delegate void ChatDelegate(Player player, string message, ref bool cancel);
    public delegate void MessageDelegate(Player player, ushort opcode, Lib.System.Buffer buffer);

    public class Server
    {
        public readonly World World;
        public readonly PlayerManager PlayerManager;
        public readonly ILogger Log;

        public event UpdateDelegate? UpdateEvent;
        public event ShutdownDelegate? ShutdownEvent;
        public event PlayerJoinDelegate? PlayerJoinEvent;
        public event PlayerLeaveDelegate? PlayerLeaveEvent;
        public event ChatDelegate? ChatEvent;

        private Dictionary<string, HashSet<MessageDelegate>> _messageHandlers = new Dictionary<string, HashSet<MessageDelegate>>();

        internal Server(World world, PlayerManager playerManager)
        {
            Log = Serilog.Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/scripting.log").CreateLogger();

            World = world;
            PlayerManager = playerManager;
        }

        public void RegisterMessageHandler(string modName, MessageDelegate messageDelegate)
        {
            HashSet<MessageDelegate>? handlers;
            if (!_messageHandlers.TryGetValue(modName, out handlers))
            {
                handlers = new HashSet<MessageDelegate>();
                _messageHandlers.Add(modName, handlers);
            }

            handlers.Add(messageDelegate);
        }

        public void UnregisterMessageHandler(string modName, MessageDelegate messageDelegate)
        {
            if (_messageHandlers.TryGetValue(modName, out var handlers))
            {
                handlers.Remove(messageDelegate);
            }
        }

        internal void OnUpdate(float deltaSeconds)
        {
            UpdateEvent?.Invoke(deltaSeconds);
        }

        internal void OnShutdown()
        {
            ShutdownEvent?.Invoke();
        }

        internal void OnPlayerJoin(Player player)
        {
            PlayerJoinEvent?.Invoke(player);
        }

        internal void OnPlayerLeave(Player player)
        {
            PlayerLeaveEvent?.Invoke(player);
        }

        internal void OnChat(Player player, string message, out bool cancel)
        {
            cancel = false;
            ChatEvent?.Invoke(player, message, ref cancel);
        }

        internal void OnMessage(Player player, string modName, ushort opcode, Lib.System.Buffer buffer)
        {
            if (_messageHandlers.TryGetValue(modName, out var handlers))
                foreach (var h in handlers)
                    h.Invoke(player, opcode, buffer);
        }
    }
}
