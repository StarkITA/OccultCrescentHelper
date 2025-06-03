using System;

namespace OccultCrescentHelper.ConfigAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RenderIfAttribute : Attribute
{
    public string[] DependentPropertyNames { get; }

    public RenderIfAttribute(params string[] dependentPropertyNames)
    {
        DependentPropertyNames = dependentPropertyNames;
    }
}
