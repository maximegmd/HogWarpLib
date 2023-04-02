using HogWarp.Lib;
using System.Reflection;

namespace HogWarp.Loader
{
    internal static class EventProcessor<T>
    {
        private static List<(IPluginBase, MethodInfo)> _plugins = new List<(IPluginBase, MethodInfo)>();
        private static Dictionary<string, (IPluginBase, MethodInfo)> _namedPlugins = new Dictionary<string, (IPluginBase, MethodInfo)>();

        static EventProcessor()
        {
            foreach (var plugin in PluginManager.Plugins)
            {
                var method = plugin.GetType().GetMethod("ProcessEvent", new Type[] { typeof(T) });
                if(method != null)
                {
                    _plugins.Add((plugin, method!));
                    _namedPlugins.Add(plugin.Name, (plugin, method!));
                }
            }
        }

        public static void DispatchTo(string name, T serverEvent)
        {
            if(_namedPlugins.TryGetValue(name, out var plugin))
            {
                var arg = new object[] { serverEvent! };

                try
                {
                    plugin.Item2.Invoke(plugin.Item1, arg);
                }
                catch { }
            }
        }

        public static void Dispatch(T serverEvent)
        {
            var arg = new object[] { serverEvent! };
            foreach (var plugin in _plugins)
            {
                try
                {
                    plugin.Item2.Invoke(plugin.Item1, arg);
                }
                catch { }
            }
        }

        public static bool DispatchCancellable(T serverEvent)
        {
            var arg = new object[] { serverEvent! };
            bool cancel = false;

            foreach (var plugin in _plugins)
            {
                try
                {
                    var result = plugin.Item2.Invoke(plugin.Item1, arg);
                    if (Convert.ToBoolean(result))
                        cancel = true;
                }
                catch { }
            }

            return cancel;
        }
    }
}
