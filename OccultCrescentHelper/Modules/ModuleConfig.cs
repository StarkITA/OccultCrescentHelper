using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using ImGuiNET;
using Lumina.Excel.Sheets;
using OccultCrescentHelper.ConfigAttributes;

namespace OccultCrescentHelper.Modules;

public abstract class ModuleConfig
{
    private List<MKDSupportJob> jobs;

    public ModuleConfig()
    {
        jobs = Svc.Data.GetExcelSheet<MKDSupportJob>().ToList();
    }

    protected bool Checkbox(string label, Func<bool> getter, Action<bool> setter)
    {
        bool value = getter();
        if (ImGui.Checkbox(label, ref value) && value != getter())
        {
            setter(value);
            return true;
        }

        return false;
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
            if (!prop.CanRead)
                continue;

            if (prop.GetMethod.IsStatic)
                continue;

            var value = prop.GetValue(this);

            var experimental = prop.GetCustomAttribute<ExperimentalFeatureAttribute>() is not null;

            var renderIfAttr = prop.GetCustomAttribute<RenderIfAttribute>();
            if (renderIfAttr != null)
            {
                var dependentProp = GetType().GetProperty(renderIfAttr.DependentPropertyName);
                if (dependentProp == null)
                    continue;

                var dependentValue = dependentProp.GetValue(this);
                if (dependentValue is bool boolValue && !boolValue)
                    continue;
            }

            // CheckboxConfig
            if (prop.GetCustomAttribute<CheckboxConfigAttribute>() is not null && prop.PropertyType == typeof(bool))
            {
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
                    dirty = true;
                }
            }

            // PhantomJobConfig

            if (prop.GetCustomAttribute<PhantomJobConfigAttribute>() is not null && prop.PropertyType == typeof(uint))
            {
                var labelAttr = prop.GetCustomAttribute<LabelAttribute>();
                string label = labelAttr?.Label ?? prop.Name;

                uint currentValue = (uint)(prop.GetValue(this) ?? 0);

                // Find the current job from your Jobs list (you'll need to access your jobs list here)
                var currentJob = jobs.FirstOrDefault(job => job.RowId == currentValue);

                if (ImGui.BeginCombo($"{label}##{prop.GetHashCode()}", currentJob.Unknown0.ToString()))
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
            }

            var tooltip = prop.GetCustomAttribute<TooltipAttribute>();

            if (tooltip != null && ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(tooltip.text);
            }
        }

        ImGui.Unindent(16);

        ImGui.Separator();

        return dirty;
    }
}
