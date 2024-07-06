using UnityEngine;
using UnityEditor;

using E = UnityEditor.EditorGUI;
using U = UnityEditor.EditorGUIUtility;

namespace Soil.Editor {

[CustomPropertyDrawer(typeof(FloatRange))]
public sealed class FloatRangeDrawer: PropertyDrawer {
    // -- constants --
    /// the width of the range separator label ("...")
    const float k_SeparatorWidth = 16f;

    /// the separator style
    static GUIStyle s_Separator;

    // -- lifecycle --
    public override void OnGUI(Rect r, SerializedProperty prop, GUIContent label) {
        E.BeginProperty(r, label, prop);

        // draw label w/ indent
        E.LabelField(r, label);

        // reset indent so that it doesn't affect inline fields
        var indent = E.indentLevel;
        E.indentLevel = 0;

        // move rect past the label
        var lw = U.labelWidth + Theme.Gap1;
        r.x += lw;
        r.width -= lw;

        // draw the range input
        var units = prop.FindAttribute<UnitsAttribute>();
        DrawInput(r, prop, units);

        // reset indent level
        E.indentLevel = indent;

        E.EndProperty();
    }

    // -- commands --
    /// draw the range input
    public static void DrawInput(
        Rect r,
        SerializedProperty prop,
        UnitsAttribute units = null
    ) {
        var min = prop.FindProp(nameof(FloatRange.Min));
        var max = prop.FindProp(nameof(FloatRange.Max));
        DrawInput(r, min, max, units);
    }

    /// draw the range input
    public static void DrawInput(
        Rect r,
        SerializedProperty min,
        SerializedProperty max,
        UnitsAttribute units = null
    ) {
        var isRange = min != null && max != null;

        // get the width of the labels
        var lw = 0f;
        if (isRange) {
            lw += k_SeparatorWidth + Theme.Gap1;
        }

        // get the width of the units label
        var uw = 0f;
        if (units != null) {
            uw += UnitsStyle().CalcSize(new GUIContent(units.Label)).x;
            lw += uw + Theme.Gap1;
        }

        // calc width of each field from remaining space
        var fw = r.width - lw;
        if (isRange) {
            fw = (fw - Theme.Gap1) / 2;
        }

        // draw the min field
        if (min != null) {
            r.width = fw;
            min.floatValue = E.FloatField(r, min.floatValue);
            r.x += fw + Theme.Gap1;
        }

        // draw the separator
        if (isRange) {
            r.width = k_SeparatorWidth;
            E.LabelField(r, "...", SeparatorStyle());
            r.x += k_SeparatorWidth + Theme.Gap1;
        }

        // draw the max field
        if (max != null) {
            r.width = fw;
            max.floatValue = E.FloatField(r, max.floatValue);
        }

        // draw the units
        if (units != null) {
            r.x += fw + Theme.Gap1;
            r.width = uw;
            E.LabelField(r, units.Label, UnitsStyle());
        }
    }

    // -- queries --
    /// the separator text style
    static GUIStyle SeparatorStyle() {
        if (s_Separator != null) {
            return s_Separator;
        }

        var separator = new GUIStyle(GUI.skin.label);
        separator.alignment = TextAnchor.MiddleCenter;
        s_Separator = separator;

        return separator;
    }

    /// the units text style
    static GUIStyle UnitsStyle() {
        return GUI.skin.label;
    }
}

}