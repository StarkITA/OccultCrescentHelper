using System;

namespace OccultCrescentHelper.ConfigAttributes;

[AttributeUsage(AttributeTargets.Property)]
public class TooltipAttribute : Attribute
{
    public string text { get; }

    public TooltipAttribute(string text)
    {
        this.text = text;
    }
}
