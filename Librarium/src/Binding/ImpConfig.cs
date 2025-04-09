#region

using System;
using BepInEx.Configuration;

#endregion

namespace Librarium.Binding;

/// <summary>
///     An ImpBinding that is linked to a BepInEx config.
///
///     It is recommended to set the ignoreBroadcasts flag when multiple configs have the same expensive update function.
/// </summary>
public sealed class ImpConfig<T> : ImpBinding<T>
{
    private readonly ConfigEntry<T> config;
    private readonly Func<bool> isDisabled;

    /// <summary>
    /// If set to true, this config will be loaded even if the disabled callback returns true.
    /// </summary>
    private readonly bool allowWhenDisabled;

    public new T Value => (!isDisabled?.Invoke() ?? true) || allowWhenDisabled ? base.Value : DefaultValue;

    public ImpConfig(
        ConfigFile configFile,
        string section,
        string key,
        T defaultValue,
        Action<T> primaryUpdate = null,
        Action<T> secondaryUpdate = null,
        bool ignoreRefresh = false,
        bool allowWhenDisabled = false,
        string description = null,
        Func<bool> isDisabled = null
    ) : base(defaultValue, default, primaryUpdate, secondaryUpdate, ignoreRefresh)
    {
        this.allowWhenDisabled = allowWhenDisabled;
        this.isDisabled = isDisabled;

        config = configFile.Bind(
            section, key,
            defaultValue,
            configDescription: !string.IsNullOrEmpty(description) ? new ConfigDescription(description) : null
        );
        base.Value = config.Value;
    }

    public override void Set(T updatedValue, bool invokePrimary = true, bool invokeSecondary = true)
    {
        config.Value = updatedValue;
        base.Set(updatedValue, invokePrimary, invokeSecondary);
    }

    public override void Refresh()
    {
        base.Value = (!isDisabled?.Invoke() ?? true) || allowWhenDisabled ? base.Value : DefaultValue;
        base.Refresh();
    }
}