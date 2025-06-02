using Dalamud.Plugin.Services;

namespace OccultCrescentHelper.Modules;

public abstract class Module
{
    public readonly Plugin plugin;

    public readonly Config _config;

    public virtual bool enabled
    {
        get => true;
    }

    public Module(Plugin plugin)
    {
        this.plugin = plugin;
        this._config = plugin.config;
    }
}
