using System;
using System.Numerics;
using ECommons.Automation.NeoTaskManager;
using Ocelot.Chain;
using Ocelot.Chain.ChainEx;
using Ocelot.IPC;

namespace OccultCrescentHelper.Chains;

public class PathfindAndMoveToChain : ChainFactory
{
    private VNavmesh vnav;

    private Vector3 destination;

    public PathfindAndMoveToChain(VNavmesh vnav, Vector3 destination, float maxRadius = 1f, float minRadius = 0f)
    {
        this.vnav = vnav;

        float angle = (float)(Random.Shared.NextDouble() * MathF.Tau);
        float distance = minRadius + (float)(Random.Shared.NextDouble() * (maxRadius - minRadius));

        float offsetX = MathF.Cos(angle) * distance;
        float offsetZ = MathF.Sin(angle) * distance;

        this.destination = new Vector3(destination.X + offsetX, destination.Y, destination.Z + offsetZ);
    }

    protected override Chain Create(Chain chain)
    {
        return chain
            .PathfindAndMoveTo(vnav, destination);
    }

    public override TaskManagerConfiguration? Config()
    {
        return new() {
            TimeLimitMS = 180000,
        };
    }
}
