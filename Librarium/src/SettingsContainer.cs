using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using Librarium.Binding;

namespace Librarium;

public class SettingsContainer(ConfigFile config)
{
    private ConfigFile config = config;

    public void Load()
    {
        GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .ToList()
            .ForEach(field => (field.GetValue(this) as IRefreshable)?.Refresh());
    }

    public void Reset()
    {
        GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .ToList()
            .ForEach(field => (field.GetValue(this) as IResettable)?.Reset());
    }
}