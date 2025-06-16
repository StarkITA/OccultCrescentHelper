using System;
using ECommons.DalamudServices;
using OccultCrescentHelper.Data;
using OccultCrescentHelper.Modules.Buff;
using Ocelot.IPC;
using Ocelot.Modules;

namespace OccultCrescentHelper.Chains;

public class ChainHelper
{
    private static ChainHelper _instance = null;

    private static ChainHelper instance {
        get {
            if (_instance == null)
            {
                throw new InvalidOperationException("ChainHelper has not been initialized. Call Initialize(plugin) first.");
            }
            return _instance;
        }
    }

    private Plugin plugin;

    private static ModuleManager modules => instance.plugin.modules;

    private static IPCManager ipc => instance.plugin.ipc;

    private ChainHelper(Plugin plugin)
    {
        this.plugin = plugin;
    }

    public static void Initialize(Plugin plugin) => _instance ??= new ChainHelper(plugin);

    public static ReturnChain ReturnChain(bool approachAetherye = true)
    {
        var buffs = modules.GetModule<BuffModule>();

        return new ReturnChain(
            ZoneData.aetherytes[Svc.ClientState.TerritoryType],
            buffs,
            ipc.GetProvider<YesAlready>(),
            ipc.GetProvider<VNavmesh>(),
            approachAetherye: approachAetherye
        );
    }
}

