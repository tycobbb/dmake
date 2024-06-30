using UnityEngine;
using UnityEditor;

using E = UnityEditor.EditorGUI;
using U = UnityEditor.EditorGUIUtility;

namespace Soil.Editor {

// TODO: show the current time when the game is playing
[CustomPropertyDrawer(typeof(EaseTimer))]
sealed class EaseTimerConfigDrawer: PropertyDrawer {
    // -- constants --
    /// the width of the curve
    const float k_CurveWidth = 40f;

    // -- PropertyDrawer --
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

        // draw the config
        DrawConfig(r, prop);

        // reset indent level
        E.indentLevel = indent;

        E.EndProperty();
    }

    // -- commands --
    /// draw the config fields
    public static void DrawConfig(Rect r, SerializedProperty prop) {
        // get attrs
        var value = prop.FindPropertyRelative(nameof(EaseTimer.Config.Duration));
        var curve = prop.FindPropertyRelative(nameof(EaseTimer.Config.Curve));

        // draw the curve
        var rc = r;
        rc.width = k_CurveWidth;
        rc.y -= 1;
        rc.height += 1;
        curve.animationCurveValue = E.CurveField(rc, curve.animationCurveValue);

        // move past the curve
        var delta = rc.width + Theme.Gap3;
        r.x += delta;
        r.width -= delta;

        // draw the duration
        value.floatValue = E.FloatField(r, value.floatValue);
    }
}

}