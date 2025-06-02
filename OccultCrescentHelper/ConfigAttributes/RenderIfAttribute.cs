using System;

namespace OccultCrescentHelper.ConfigAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RenderIfAttribute : Attribute
{
    public string DependentPropertyName { get; }

    public RenderIfAttribute(string dependentPropertyName)
    {
        DependentPropertyName = dependentPropertyName;
    }
}
