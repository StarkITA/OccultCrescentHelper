using System.Numerics;
using Lumina.Excel.Sheets;
using OccultCrescentHelper.Enums;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.Teleporter;

[OcelotModule(1)]
public class TeleporterModule : Module<Plugin, Config>
{
    public override TeleporterConfig config {
        get => _config.TeleporterConfig;
    }

    public readonly Teleporter teleporter;

    public TeleporterModule(Plugin plugin, Config config)
        : base(plugin, config)
    {
        teleporter = new(this);
    }

    public void Teleport(Aethernet aethernet, Vector3? destination = null) => teleporter.Teleport(aethernet, destination ?? Vector3.Zero);

    public bool IsReady() => teleporter.IsReady();

    public Aethernet GetClosestAethernet(Vector3 position) => teleporter.GetClosestAethernet(position);
}
