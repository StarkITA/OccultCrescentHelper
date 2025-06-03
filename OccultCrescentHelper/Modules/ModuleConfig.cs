using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface;
using ECommons.DalamudServices;
using ImGuiNET;
using Lumina.Excel.Sheets;
using OccultCrescentHelper.ConfigAttributes;

namespace OccultCrescentHelper.Modules;

public abstract class ModuleConfig
{
    private readonly List<MKDSupportJob> jobs;
    private readonly Dictionary<Type, Func<PropertyInfo, bool, (bool handled, bool dirty)>> renderers;

    public ModuleConfig()
    {
        jobs = Svc.Data.GetExcelSheet<MKDSupportJob>().ToList();

        renderers = new() { [typeof(CheckboxConfigAttribute)] = DrawCheckbox, [typeof(PhantomJobConfigAttribute)] = DrawPhantomJobSelector };
    }

    public virtual bool Draw()
    {
        bool dirty = false;

        var title = GetType().GetCustomAttribute<TitleAttribute>();
        if (title != null)
        {
            ImGui.TextColored(new Vector4(1f, 0.75f, 0.25f, 1f), $"{title.text}:");
        }

        ImGui.Indent(16);
        foreach (var prop in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanRead || prop.GetMethod.IsStatic || !ShouldRender(prop))
                continue;

            bool isExperimental = prop.GetCustomAttribute<ExperimentalFeatureAttribute>() != null;

            foreach (var kv in renderers)
            {
                if (prop.GetCustomAttribute(kv.Key) != null)
                {
                    var (handled, modified) = kv.Value.Invoke(prop, isExperimental);
                    if (handled && modified)
                        dirty = true;
                }
            }

            ShowTooltip(prop);
        }
        ImGui.Unindent(16);
        Helpers.VSpace();
        ImGui.Separator();
        return dirty;
    }

    private (bool handled, bool dirty) DrawCheckbox(PropertyInfo prop, bool experimental)
    {
        if (prop.PropertyType != typeof(bool))
            return (false, false);

        var labelAttr = prop.GetCustomAttribute<LabelAttribute>();
        string label = labelAttr?.Label ?? prop.Name;

        bool currentValue = (bool)(prop.GetValue(this) ?? false);
        bool valueCopy = currentValue;

        if (experimental)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.9f, 0.0f, 1.0f));
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.TextUnformatted(FontAwesomeIcon.ExclamationTriangle.ToIconString());
            ImGui.PopFont();
            ImGui.PopStyleColor();

            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Experimental");
            }

            ImGui.SameLine();
        }

        if (ImGui.Checkbox($"{label}##{prop.GetHashCode()}", ref valueCopy) && valueCopy != currentValue)
        {
            prop.SetValue(this, valueCopy);
            return (true, true);
        }

        return (true, false);
    }

    private (bool handled, bool dirty) DrawPhantomJobSelector(PropertyInfo prop, bool experimental)
    {
        if (prop.PropertyType != typeof(uint))
            return (false, false);

        var labelAttr = prop.GetCustomAttribute<LabelAttribute>();
        string label = labelAttr?.Label ?? prop.Name;

        uint currentValue = (uint)(prop.GetValue(this) ?? 0);

        var currentJob = jobs.FirstOrDefault(job => job.RowId == currentValue);

        bool dirty = false;
        if (ImGui.BeginCombo($"{label}##{prop.GetHashCode()}", currentJob.Unknown0.ToString() ?? "None"))
        {
            foreach (var job in jobs)
            {
                if (job.RowId <= 0)
                    continue;

                bool isSelected = job.RowId == currentValue;

                if (ImGui.Selectable(job.Unknown0.ToString(), isSelected))
                {
                    prop.SetValue(this, job.RowId);
                    dirty = true;
                }

                if (isSelected)
                    ImGui.SetItemDefaultFocus();
            }

            ImGui.EndCombo();
        }

        return (true, dirty);
    }

    private bool ShouldRender(PropertyInfo prop)
    {
        var renderIfAttr = prop.GetCustomAttribute<RenderIfAttribute>();
        if (renderIfAttr == null)
            return true;

        foreach (var propName in renderIfAttr.DependentPropertyNames)
        {
            var dependentProp = GetType().GetProperty(propName);
            if (dependentProp == null || dependentProp.PropertyType != typeof(bool))
                return false;

            if (!(bool)(dependentProp.GetValue(this) ?? false))
                return false;
        }

        return true;
    }

    private void ShowTooltip(PropertyInfo prop)
    {
        var tooltip = prop.GetCustomAttribute<TooltipAttribute>();
        if (tooltip != null && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(tooltip.text);
        }
    }
}
