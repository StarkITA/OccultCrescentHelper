using System;

namespace OccultCrescentHelper.ConfigAttributes;

[AttributeUsage(AttributeTargets.Class)]
public class TitleAttribute : Attribute
{
    public string text { get; }

    public TitleAttribute(string text)
    {
        this.text = text;
    }
}
