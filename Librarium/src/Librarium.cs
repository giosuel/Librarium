using BepInEx;

namespace Librarium;

[BepInPlugin(LCMPluginInfo.PLUGIN_GUID, LCMPluginInfo.PLUGIN_NAME, LCMPluginInfo.PLUGIN_VERSION)]
public class Librarium : BaseUnityPlugin
{
    private void Awake()
    {
        Logger.LogInfo("Librarium has been loaded successfully! \\o/");
    }
}