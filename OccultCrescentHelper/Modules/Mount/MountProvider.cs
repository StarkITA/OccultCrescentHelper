using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Ocelot.Config.Handlers;
using ExcelMount = Lumina.Excel.Sheets.Mount;

namespace OccultCrescentHelper.Modules.Mount;

public class MountProvider : ExcelSheetItemProvider<ExcelMount>
{
    public unsafe override bool Filter(ExcelMount item) => PlayerState.Instance()->IsMountUnlocked(item.RowId);

    public override string GetLabel(ExcelMount item) => System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.Singular.ToString());
}
