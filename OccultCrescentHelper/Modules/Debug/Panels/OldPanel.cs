// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Numerics;
// using Dalamud.Game.ClientState.Conditions;
// using Dalamud.Plugin.Services;
// using ECommons.DalamudServices;
// using ECommons.Throttlers;
// using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
// using ImGuiNET;
// using Lumina.Data.Files;
// using Lumina.Excel.Sheets;
// using OccultCrescentHelper.Data;
// using OccultCrescentHelper.Enums;
// using OccultCrescentHelper.Modules.Teleporter;
// using Ocelot;
// using Ocelot.Chain;
// using Ocelot.IPC;

// namespace OccultCrescentHelper.Modules.Debug.Panels;

// public class dPanel
// {

//     private enum PathType
//     {
//         Move,
//         Jump,
//     }

//     private class PathEntry
//     {
//         public Vector3 pos;
//         public PathType type;

//         public PathEntry(Vector3 pos, PathType type)
//         {
//             this.pos = pos;
//             this.type = type;
//         }

//         public static PathEntry Move(Vector3 pos) => new(pos, PathType.Move);
//         public static PathEntry Jump(Vector3 pos) => new(pos, PathType.Jump);

//         public override bool Equals(object? obj)
//         {
//             return obj is PathEntry other &&
//                    pos.Equals(other.pos) &&
//                    type == other.type;
//         }

//         public override int GetHashCode() => HashCode.Combine(pos, type);
//     }

//     private List<PathEntry> path = [];

//     private bool running = false;




//     public void Draw(DebugModule module)
//     {
//         OcelotUI.Region("OCH##DEbug", () => {
//             OcelotUI.Title("Debug:");
//             OcelotUI.Indent(() => {
//                 if (ImGui.CollapsingHeader("Teleporter"))
//                 {
//                     Teleporter(module);
//                     OcelotUI.VSpace();
//                 }

//                 if (ImGui.CollapsingHeader("VNav"))
//                 {
//                     VNav(module);
//                     OcelotUI.VSpace();
//                 }

//                 if (ImGui.CollapsingHeader("Fates"))
//                 {
//                     Fates(module);
//                     OcelotUI.VSpace();
//                 }

//                 if (ImGui.CollapsingHeader("CriticalEncounters"))
//                 {
//                     CriticalEncounters(module);
//                     OcelotUI.VSpace();
//                 }

//                 if (ImGui.CollapsingHeader("PathMaker"))
//                 {
//                     PathMaker(module);
//                     OcelotUI.VSpace();
//                 }

//                 if (ImGui.CollapsingHeader("Chain Manager"))
//                 {
//                     ChainManager(module);
//                     OcelotUI.VSpace();
//                 }
//             });
//         });
//     }

//     private bool jumping = false;

//     public void Tick(DebugModule module)
//     {
//         if (!running)
//         {
//             return;
//         }

//         Vector3 snap(Vector3 pos) => new(
//             MathF.Round(pos.X, 2),
//             MathF.Round(pos.Y, 2),
//             MathF.Round(pos.Z, 2)
//         );

//         if (EzThrottler.Throttle("Path Generator", 100))
//         {
//             var player = Svc.ClientState.LocalPlayer;
//             if (player == null)
//             {
//                 return;
//             }

//             var jumpCondition = Svc.Condition[ConditionFlag.Jumping];
//             var position = snap(player.Position);

//             if (!jumping && jumpCondition)
//             {
//                 var jump = PathEntry.Jump(position);
//                 if (!path.Contains(jump))
//                 {
//                     path.Add(jump);
//                 }
//                 jumping = true;
//             }

//             if (jumpCondition)
//             {
//                 return;
//             }

//             jumping = false;

//             var move = PathEntry.Move(position);
//             if (!path.Contains(move))
//             {
//                 path.Add(move);
//             }
//         }
//     }

//     private unsafe void PathMaker(DebugModule module)
//     {
//         OcelotUI.Title("Path Maker:");
//         OcelotUI.Indent(() => {
//             var label = running ? "Stop" : "Start";

//             if (ImGui.Button(label))
//             {
//                 running = !running;
//                 if (running)
//                 {
//                     // Clear on start
//                     path.Clear();
//                 }
//             }

//             var moveNodes = path.Where(e => e.type == PathType.Move).Select(e => $"[{e.pos.X:f2}f, {e.pos.Y:f2}f, {e.pos.Z:f2}f]");
//             var jumpNodes = path.Where(e => e.type == PathType.Jump).Select(e => $"[{e.pos.X:f2}f, {e.pos.Y:f2}f, {e.pos.Z:f2}f]");

//             var output = "Prowler.PathWithJumps([\n    " +
//                 string.Join(",\n    ", moveNodes) +
//             "\n], [\n    " +
//                 string.Join(",\n    ", jumpNodes) +
//             "\n]),";

//             // var output = string.Join("\n", nodes);
//             ImGui.PushItemWidth(-1); // Use all available width
//             ImGui.InputTextMultiline("##PathOutput", ref output, 4096, ImGui.GetContentRegionAvail(), ImGuiInputTextFlags.ReadOnly);
//             ImGui.PopItemWidth();
//         });
//     }



// }
