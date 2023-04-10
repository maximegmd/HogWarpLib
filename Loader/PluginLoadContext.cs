using System.Reflection;
using System.Runtime.Loader;

namespace HogWarp.Loader
{
    internal class PluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;
        private AssemblyDependencyResolver _baseResolver;

        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
            _baseResolver = new AssemblyDependencyResolver(typeof(PluginLoadContext).Assembly.Location);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            if (typeof(HogWarp.Lib.Server).Assembly.GetName().Name == assemblyName.Name)
                return typeof(HogWarp.Lib.Server).Assembly;

            string? assemblyPath = _baseResolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                var assembly = LoadFromAssemblyPath(assemblyPath);
                Console.WriteLine($"{assemblyName} : {assembly}");
                return assembly;
            }

            assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string? libraryPath = _baseResolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
