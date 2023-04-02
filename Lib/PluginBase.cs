namespace HogWarp.Lib
{
    public interface IPluginBase
    { 
        string Name { get; }
        string Description { get; }

        void Initialize(Server server);
    }
}
