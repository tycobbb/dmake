using UnityEngine;
using UnityEditor;

using E = UnityEditor.EditorGUI;
using U = UnityEditor.EditorGUIUtility;

namespace Soil.Editor {

[CustomPropertyDrawer(typeof(FloatRange))]
public sealed class FloatRangeDrawer: PropertyDrawer {
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
        Draw.FloatRangeField(r, min, max, units);
    }
}

}