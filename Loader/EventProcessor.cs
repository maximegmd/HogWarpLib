using HogWarp.Lib;
using System.Reflection;

namespace HogWarp.Loader
{
    internal static class EventProcessor<T>
    {
        private static List<(IPluginBase, MethodInfo)> _plugins = new List<(IPluginBase, MethodInfo)>();

        static EventProcessor()
        {
            foreach (var plugin in PluginManager.Plugins)
            {
                var method = plugin.GetType().GetMethod("ProcessEvent", new Type[] { typeof(T) });
                if(method != null)
                {
                    _plugins.Add((plugin, method!));
                }
            }
        }

        public static void Dispatch(T serverEvent)
        {
            var arg = new object[] { serverEvent! };
            foreach (var plugin in _plugins)
            {
                plugin.Item2.Invoke(plugin.Item1, arg);
            }
        }
    }
}
