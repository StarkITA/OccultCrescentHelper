using Dalamud.Memory;
using ECommons;
using ECommons.DalamudServices;
using ECommons.UIHelpers.AddonMasterImplementations;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;

namespace OccultCrescentHelper;

public unsafe partial class MKDInfo : AddonMasterBase<AtkUnitBase>
{
    public MKDInfo(nint addon)
        : base(addon) { }

    public MKDInfo(void* addon)
        : base(addon) { }

    public unsafe void Debug()
    {
        var values = (nint)Addon->AtkValues;
        for (int i = 0; i < Addon->AtkValuesCount; i++)
        {
            var value = Addon->AtkValues[i];
            Svc.Log.Debug(
                $"Value {i}: Type={value.Type}, Int={value.Int}, UInt={value.UInt}, Float={value.Float}, String={MemoryHelper.ReadSeStringNullTerminated((nint)value.String.Value).GetText()}"
            );
        }
    }

    public int Level
    {
        get
        {
            var node = (AtkTextNode*)Addon->UldManager.NodeList[32];

            if (int.TryParse(node->NodeText.GetText(), out int result))
            {
                return result;
            }

            return 0;
        }
        // get
        // {
        //     string raw = MemoryHelper
        //         .ReadSeStringNullTerminated((nint)Addon->AtkValues[0].String.Value)
        //         .GetText();
        //     if (int.TryParse(raw, out int result))
        //         return result;
        //     return -1;
        // }
    }

    public int PhantomJobId
    {
        get
        {
            string raw = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[12].String.Value).GetText();
            if (int.TryParse(raw, out int result))
                return result;
            return -1;
        }
    }

    public MKDSupportJob? PhantomJob
    {
        get { return Svc.Data.GetExcelSheet<MKDSupportJob>().FirstOrNull((job) => job.RowId == PhantomJobId); }
    }

    public override string AddonDescription => "Occult Crescent Common Information";
}
