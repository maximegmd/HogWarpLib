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

        public event UpdateDelegate? UpdateEvent;
        public event ShutdownDelegate? ShutdownEvent;
        public event PlayerJoinDelegate? PlayerJoinEvent;
        public event PlayerLeaveDelegate? PlayerLeaveEvent;
        public event ChatDelegate? ChatEvent;

        private Dictionary<string, HashSet<MessageDelegate>> _messageHandlers = new Dictionary<string, HashSet<MessageDelegate>>();

        internal Server(World world, PlayerManager playerManager)
        {
            Serilog.Log.Logger = new LoggerConfiguration()
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

        public void Debug(string message)
        {
            Serilog.Log.Debug(message);
        }

        public void Information(string message)
        {
            Serilog.Log.Information(message);
        }

        public void Warning(string message)
        {
            Serilog.Log.Warning(message);
        }

        public void Error(string message)
        {
            Serilog.Log.Error(message);
        }

        public void Fatal(string message)
        {
            Serilog.Log.Fatal(message);
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
            if (UpdateEvent == null)
                return;

            foreach (UpdateDelegate handler in UpdateEvent!.GetInvocationList())
            {
                try
                {
                    handler?.Invoke(deltaSeconds);
                }
                catch (Exception ex)
                {
                    Warning(ex.ToString());
                }
            }
        }

        internal void OnShutdown()
        {
            if (ShutdownEvent == null)
                return;

            foreach (ShutdownDelegate handler in ShutdownEvent!.GetInvocationList())
            {
                try
                {
                    handler?.Invoke();
                }
                catch (Exception ex)
                {
                    Warning(ex.ToString());
                }
            }
        }

        internal void OnPlayerJoin(Player player)
        {
            if (PlayerJoinEvent == null)
                return;

            foreach (PlayerJoinDelegate handler in PlayerJoinEvent!.GetInvocationList())
            {
                try
                {
                    handler?.Invoke(player);
                }
                catch (Exception ex)
                {
                    Warning(ex.ToString());
                }
            }
        }

        internal void OnPlayerLeave(Player player)
        {
            if (PlayerLeaveEvent == null)
                return;

            foreach (PlayerLeaveDelegate handler in PlayerLeaveEvent!.GetInvocationList())
            {
                try
                {
                    handler?.Invoke(player);
                }
                catch (Exception ex)
                {
                    Warning(ex.ToString());
                }
            }

        }

        internal void OnChat(Player player, string message, out bool cancel)
        {
            cancel = false;

            if (ChatEvent == null)
                return;

            
            foreach (ChatDelegate handler in ChatEvent!.GetInvocationList())
            {
                try
                {
                    handler?.Invoke(player, message, ref cancel);
                }
                catch (Exception ex)
                {
                    Warning(ex.ToString());
                }
            }
        }

        internal void OnMessage(Player player, string modName, ushort opcode, Lib.System.Buffer buffer)
        {
            if (_messageHandlers.TryGetValue(modName, out var handlers))
            {
                foreach (var h in handlers)
                {
                    try
                    {
                        h.Invoke(player, opcode, buffer);
                    }
                    catch (Exception ex)
                    {
                        Warning(ex.ToString());
                    }
                }
            }
        }
    }
}
