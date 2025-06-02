using System;

namespace OccultCrescentHelper.ConfigAttributes;

[AttributeUsage(AttributeTargets.Property)]
public class LabelAttribute : Attribute
{
    public string Label { get; }

    public LabelAttribute(string label)
    {
        Label = label;
    }
}
