using System;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using ImGuiNET;
using Lumina.Excel.Sheets;
using OccultCrescentHelper.Api;

namespace OccultCrescentHelper.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin plugin;

    public MainWindow(Plugin plugin)
        : base(plugin.Name, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        plugin.treasures.Draw();
        plugin.carrots.Draw();
        plugin.currency.Draw();
        plugin.fates.Draw();

        // BNpcBase

        if (ImGui.Button("Print Target Info!"))
        {
            foreach (var npc in Svc.Objects.OfType<IBattleNpc>())
            {
                IntPtr ptr = npc.Address;
                unsafe
                {
                    GameObject* obj = (GameObject*)ptr.ToPointer();
                    var spawn = obj->DefaultPosition;

                    MonsterPayload payload = new MonsterPayload()
                    {
                        name = npc.Name.ToString(),
                        spawn_position = new Position()
                        {
                            X = spawn.X,
                            Y = spawn.Y,
                            Z = spawn.Z,
                        },
                    };
                }
                // Svc.Log.Info(npc.DataId.ToString());
                // Svc.Log.Info(npc.SubKind.ToString());
                // Svc.Log.Info(npc.Level.ToString());

                // var d/ata = Svc.Data.GetExcelSheet<BNpcBase>().ToList().FirstOrDefault(r => r.RowId == npc.NameId);

                // Svc.Log.Info(
                //     $"{npc.NameId}: {data} ({data.Unknown0}, {data.Unknown1}, {data.Unknown2}, {data.Unknown3}, {data.Unknown10}, {data.Unknown4}, {data.Unknown6}, {data.Unknown7}, {data.Unknown_70}, {data.Unknown8})"
                // );
            }
        }
    }
}
