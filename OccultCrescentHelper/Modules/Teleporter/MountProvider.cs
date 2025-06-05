using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using Ocelot.Config.Handlers;

namespace OccultCrescentHelper.Modules.Teleporter;

public class MountProvider : ExcelSheetItemProvider<Mount>
{
    public unsafe override bool Filter(Mount item) => PlayerState.Instance()->IsMountUnlocked(item.RowId);

    public override string GetLabel(Mount item) => System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.Singular.ToString());

}
