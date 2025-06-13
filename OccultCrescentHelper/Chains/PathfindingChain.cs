using System;
using System.Numerics;
using OccultCrescentHelper.Data;
using Ocelot.Chain;
using Ocelot.IPC;

namespace OccultCrescentHelper.Chains;

public class PathfindingChain : ChainFactory
{
    private VNavmesh vnav;

    private Vector3 destination;

    private EventData data;

    private bool useCustomPaths = false;

    public PathfindingChain(VNavmesh vnav, Vector3 destination, EventData data, bool useCustomPaths, float maxRadius = 1f, float minRadius = 0f)
    {
        this.vnav = vnav;
        this.data = data;
        this.useCustomPaths = useCustomPaths;

        float angle = (float)(Random.Shared.NextDouble() * MathF.Tau);
        float distance = minRadius + (float)(Random.Shared.NextDouble() * (maxRadius - minRadius));

        float offsetX = MathF.Cos(angle) * distance;
        float offsetZ = MathF.Sin(angle) * distance;

        this.destination = new Vector3(destination.X + offsetX, destination.Y, destination.Z + offsetZ);
    }

    protected override Chain Create(Chain chain)
    {
        if (useCustomPaths && data.pathFactory != null)
        {
            return Chain.Create("Prowler")
                .Then(new ProwlerChain(vnav, data.pathFactory, destination));
        }

        return Chain.Create("Pathfinding")
            .Then(new PathfindAndMoveToChain(vnav, destination));
    }
}
