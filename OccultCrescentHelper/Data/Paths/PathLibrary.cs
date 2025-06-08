using System;
using System.Collections.Generic;
using Ocelot.Prowler;

namespace OccultCrescentHelper.Data.Paths;

public abstract class PathLibrary
{
    protected static Func<List<IProwlerAction>> GetRandomPath(List<Func<List<IProwlerAction>>> paths)
    {
        var random = new Random();
        int index = random.Next(paths.Count);
        return paths[index];
    }
}
