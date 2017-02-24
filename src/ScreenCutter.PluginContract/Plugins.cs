using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ScreenCutter.PluginContract
{
    public static class Plugins
    {
        private static Assembly[] pluginAssemblies;

        private static Type[] pluginTypes;

        private static IPlugin[] plugins;

        static Plugins ()
        {
            Plugins.pluginAssemblies = Plugins.LoadPluginAssembles(Environment.CurrentDirectory, "*Plugin.dll");
            
            Plugins.pluginTypes = Plugins.LoadPluginTypes(Plugins.pluginAssemblies);
            
            Plugins.plugins = Plugins.LoadPlugins(Plugins.pluginTypes);
        }
        
        public static T Get<T>(string name) where T : class, IPlugin
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Type must be interface", "T");
            }
            
            foreach (var plugin in Plugins.plugins)
            {
                var pluginType = plugin.GetType();
                if (pluginType.FullName == name
                    && pluginType.GetInterface(typeof(T).FullName) != null)
                {
                    return (T)plugin;
                }
            }

            return null;
        }

        private static Assembly[] LoadPluginAssembles(string path, string searchPattern)
        {
            var dllFileNames = Directory.GetFiles(path, searchPattern);
            var assemblies = new List<Assembly>(dllFileNames.Length);

            foreach (var dllFile in dllFileNames)
            {
                var an = AssemblyName.GetAssemblyName(dllFile);
                var assembly = Assembly.Load(an);

                if (assembly != null)
                {
                    assemblies.Add(assembly);
                }
            }

            return assemblies.ToArray();
        }

        private static Type[] LoadPluginTypes(IEnumerable<Assembly> assemblies)
        {
            var pluginType = typeof(IPlugin);
            var pluginTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                if (assembly == null)
                {
                    continue;
                }
                
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsInterface || type.IsAbstract)
                    {
                        continue;
                    }
                    else
                    {
                        if (type.GetInterface(pluginType.FullName) != null)
                        {
                            pluginTypes.Add(type);
                        }
                    }
                }
            }

            return pluginTypes.ToArray();
        }

        private static IPlugin[] LoadPlugins(IEnumerable<Type> types)
        {
            var plugins = new List<IPlugin>();

            foreach (var type in types)
            {
                var plugin = (IPlugin)Activator.CreateInstance(type);
                plugins.Add(plugin);
            }

            return plugins.ToArray();
        }
    }
}
