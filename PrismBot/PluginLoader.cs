using System.Reflection;
using PrismBot.SDK;
using YukariToolBox.LightLog;

namespace PrismBot;

public static class PluginLoader
{
    private static readonly string _pluginsDirectory = Path.Combine(AppContext.BaseDirectory, "plugins");
    private static readonly Dictionary<string, Assembly> _loadedAssemblies = new();
    private static readonly List<Plugin> _plugins = new();

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

                _plugins.Add(pluginInstance);
            }
        }

        // 加载本程序集的插件
        CreateAndAddPluginInstances(Assembly.GetExecutingAssembly());


        var pluginPaths = Directory.GetFiles(_pluginsDirectory, "*.dll");
        foreach (var pluginPath in pluginPaths)
        {
            try
            {
                if (_loadedAssemblies.TryGetValue(pluginPath, out _))
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
                _loadedAssemblies.Add(pluginPath, assembly);


                CreateAndAddPluginInstances(assembly);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load assembly \"{pluginPath}\".", ex);
            }
        }


        foreach (var p in _plugins)
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

        Log.Info("Plugin Loader", $"已加载 {_plugins.Count} 个插件");
    }
}