using Soil;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

/// the player controller
sealed class Player: MonoBehaviour {
    // -- cfg --
    [Header("cfg")]
    [Tooltip("the tuning")]
    [SerializeField] PlayerTuning m_Tuning;

    // -- props --
    /// the current timer for the move action
    EaseTimer m_Move;

    /// the forward direction for this move
    Vector3 m_Move_Forward;

    /// the current timer for the move rotation
    EaseTimer m_Move_Rotation;

    /// the initial rotation when the move began
    Quaternion m_Move_Rotation_From;

    /// the initial rotation when the move began
    Quaternion m_Move_Rotation_To;

    // -- lifecycle --
    void Start() {
        m_Move = new EaseTimer();
        m_Move_Rotation = new EaseTimer(new(0f, m_Tuning.Move_Rotation_Curve));
    }

    void Update() {
        var isMoveJustPressed = Keyboard.current.spaceKey.wasPressedThisFrame;
        if (m_Move.IsInactive && isMoveJustPressed) {
            var nextForward = Random.insideUnitCircle.normalized.XZ();

            // start move
            m_Move_Forward = nextForward;
            m_Move.Duration = m_Tuning.Move_Duration.Sample();
            m_Move.Start();

            // start move rotation
            m_Move_Rotation_From = transform.rotation;
            m_Move_Rotation_To = Quaternion.LookRotation(nextForward, Vector3.up);
            m_Move_Rotation.Duration = m_Tuning.Move_Rotation_Duration.Sample();
            m_Move_Rotation.Start();
        }
    }

    void FixedUpdate() {
        var delta = Time.deltaTime;
        var trs = transform;

        if (m_Move.TryTick()) {
            var moveSpeed = m_Tuning.Move_Speed.Evaluate(m_Move.Pct);
            trs.position += moveSpeed * delta * m_Move_Forward;
        }

        if (m_Move_Rotation.TryTick()) {
            trs.rotation = Quaternion.Slerp(
                m_Move_Rotation_From,
                m_Move_Rotation_To,
                m_Move_Rotation.Pct
            );
        }
    }
}