using System.Reflection;
using PrismBot.SDK;
using YukariToolBox.LightLog;

namespace PrismBot;

public static class PluginLoader
{
    private static readonly string PluginsDirectory = Path.Combine(AppContext.BaseDirectory, "plugins");
    private static readonly Dictionary<string, Assembly> LoadedAssemblies = new();
    private static readonly List<Plugin> Plugins = new();

    public static void LoadPlugins()
    {
        void CreateAndAddPluginInstances(Assembly assembly)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                if (!type.IsSubclassOf(typeof(Plugin)) || !type.IsPublic || type.IsAbstract)
                    continue;

                Plugin pluginInstance;
                try
                {
                    pluginInstance = (Activator.CreateInstance(type) as Plugin)!;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Could not create an instance of plugin class \"{type.FullName}\".", ex);
                }

                Plugins.Add(pluginInstance);
            }
        }

        // 加载本程序集的插件
        CreateAndAddPluginInstances(Assembly.GetExecutingAssembly());


        var pluginPaths = Directory.GetFiles(PluginsDirectory, "*.dll");
        foreach (var pluginPath in pluginPaths)
            try
            {
                if (LoadedAssemblies.TryGetValue(pluginPath, out _))
                    continue;

                Assembly assembly;
                try
                {
                    assembly = Assembly.LoadFrom(pluginPath);
                }
                catch (BadImageFormatException)
                {
                    continue;
                }

                LoadedAssemblies.Add(pluginPath, assembly);


                CreateAndAddPluginInstances(assembly);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load assembly \"{pluginPath}\".", ex);
            }


        foreach (var p in Plugins)
        {
            try
            {
                p.OnLoad();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Plugin \"{p.GetPluginName()}\" has thrown an exception during initialization.", ex);
            }

            Log.Info("Plugin Loader", $"{p.GetPluginName()} v{p.GetVersion()} (by {p.GetAuthor()}) initiated");
        }

        Log.Info("Plugin Loader", $"已加载 {Plugins.Count} 个插件");
    }
}