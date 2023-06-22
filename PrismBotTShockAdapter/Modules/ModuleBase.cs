using System.Reflection;

namespace PrismBotTShockAdapter.Modules;

public abstract class ModuleBase
{
    public static readonly List<ModuleBase> Modules = new();

    public static void LoadModulesFromAssembly(Assembly assembly)
    {
        foreach (var type in assembly.GetExportedTypes())
        {
            if (!type.IsSubclassOf(typeof(ModuleBase)) || !type.IsPublic || type.IsAbstract)
                continue;

            ModuleBase moduleInstance;
            try
            {
                moduleInstance = (Activator.CreateInstance(type) as ModuleBase)!;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Could not create an instance of Module class \"{type.FullName}\".", ex);
            }

            Modules.Add(moduleInstance);
        }
    }

    public static void InitializeModules()
    {
        foreach (var m in Modules)
        {
            m.Initialize();
        }
    }

    public abstract void Initialize();
}