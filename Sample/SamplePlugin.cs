using HogWarp.Lib;
using HogWarp.Lib.Events;

namespace Sample
{
    public class SamplePlugin : IPluginBase
    {
        public string Name => "Sample plugin";

        public string Description => "This plugin shows how to create a basic plugin";

        private Server? _server;

        public void Initialize(Server server)
        {
            _server = server;
        }

        public void ProcessEvent(Update serverEvent)
        {
            Console.WriteLine($"Update! {serverEvent.DeltaSeconds}, year {_server!.World.Year}");
        }
    }
}