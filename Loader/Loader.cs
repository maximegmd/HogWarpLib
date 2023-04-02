using HogWarp.Lib;
using System.Reflection;

namespace HogWarp.Loader
{
    public static class PluginManager
    {
        private static List<IPluginBase> _plugins = new List<IPluginBase>();

        internal static IEnumerable<IPluginBase> Plugins { get => _plugins; }

        public static void InitializePlugins(Server server)
        {
            foreach(var plugin in Plugins)
            {
                try
                {
                    plugin.Initialize(server);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public static void LoadFromBase(string relativePath)
        {
            string root = Path.GetFullPath(Path.GetDirectoryName(typeof(EntryPoint).Assembly.Location!)!);
            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));

            var locations = Directory.GetDirectories(pluginLocation);
            foreach ( var location in locations )
            {
                try
                {
                    var path = Path.Combine(pluginLocation, location, "Assembly-Plugin.dll");
                    var assembly = LoadPlugin(path);
                    if (assembly != null)
                    {
                        _plugins.AddRange(CreatePlugins(assembly));
                    }
                }
                catch( Exception ex ) 
                {
                    Console.WriteLine( ex.ToString() );
                }
            }
        }

        static Assembly LoadPlugin(string relativePath)
        {
            // Navigate up to the solution root
            string root = Path.GetFullPath(Path.GetDirectoryName(typeof(EntryPoint).Assembly.Location!)!);

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading plugin from: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));

        }

        static IEnumerable<IPluginBase> CreatePlugins(Assembly assembly) 
        {
            int count = 0;

            foreach(var type in assembly.GetTypes())
            {
                if(typeof(IPluginBase).IsAssignableFrom(type))
                {
                    IPluginBase? result = Activator.CreateInstance(type) as IPluginBase;
                    if(result != null)
                    {
                        ++count;
                        yield return result!;
                    }
                }
            }

            if (count == 0)
            {
                throw new ApplicationException(
                    $"Can't find any type which implements IPluginBase in {assembly} from {assembly.Location}.");
            }
        }

        public static void Dispatch<T>(T serverEvent)
        {
            try
            {
                EventProcessor<T>.Dispatch(serverEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}