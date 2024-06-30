using Soil;
using UnityEngine;

/// the player tuning
[CreateAssetMenu(fileName = "PlayerTuning", menuName = "dmake/PlayerTuning", order = 0)]
sealed class PlayerTuning: ScriptableObject {
    // -- move --
    [Header("move")]
    [Tooltip("the move timer duration range")]
    public FloatRange Move_Duration;

    [Tooltip("the speed as fn of pct complete")]
    public MapOutCurve Move_Speed;

    [Tooltip("the move rotation timer duration range")]
    public FloatRange Move_Rotation_Duration;

    [Tooltip("the move rotation timer curve")]
    public AnimationCurve Move_Rotation_Curve;
}