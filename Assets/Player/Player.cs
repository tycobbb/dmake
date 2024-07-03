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

    // -- refs --
    [Header("refs")]
    [Tooltip("the physics body")]
    [SerializeField] Rigidbody m_Body;

    // -- props --
    /// the current timer for the move action
    EaseTimer m_Move;

    /// the forward direction for this move
    Vector3 m_Move_Forward;

    /// the current timer for the move rotation
    EaseTimer m_Move_Rotation;

    /// the initial rotation when the move began
    Quaternion m_Move_Rotation_From;

    /// the final rotation of the move
    Quaternion m_Move_Rotation_To;

    /// the current rotation of the move
    Quaternion m_Move_Rotation_Curr;

    /// the rotation reflection from hitting a wall
    Quaternion m_Move_Rotation_Reflect;

    /// the current timer for the move rotation
    Vector3 m_Move_Rotation_InitialForward;

    // -- lifecycle --
    void Start() {
        m_Move = new EaseTimer();
        m_Move_Rotation = new EaseTimer(new(0f, m_Tuning.Move_Rotation_Curve));

        m_Move_Rotation_Curr = Quaternion.identity;
        m_Move_Rotation_Reflect = Quaternion.identity;
        m_Move_Rotation_InitialForward = transform.forward;
    }

    void Update() {
        var isMoveJustPressed = Keyboard.current.spaceKey.wasPressedThisFrame;
        if (m_Move.IsInactive && isMoveJustPressed) {
            // start move
            m_Move.Duration = m_Tuning.Move_Duration.Sample();
            m_Move.Start();

            // start move rotation
            var nextForward = Random.insideUnitCircle.normalized.XZ();
            m_Move_Rotation_From = m_Move_Rotation_Curr;
            m_Move_Rotation_To = m_Move_Rotation_From * Quaternion.LookRotation(nextForward, Vector3.up);

            m_Move_Rotation.Duration = m_Tuning.Move_Rotation_Duration.Sample();
            m_Move_Rotation.Start();
        }
    }

    void FixedUpdate() {
        var delta = Time.deltaTime;

        // while active, move pos
        if (m_Move.TryTick()) {
            var currPos = m_Body.position;
            var moveLen = m_Tuning.Move_Speed.Evaluate(m_Move.Pct) * delta;
            var nextPos = currPos + moveLen * transform.forward;

            m_Body.MovePosition(nextPos);
        }

        // while active, rotate along move curve
        if (m_Move_Rotation.TryTick()) {
            var nextMoveRot = Quaternion.Slerp(
                m_Move_Rotation_From,
                m_Move_Rotation_To,
                m_Move_Rotation.Pct
            );

            m_Move_Rotation_Curr = nextMoveRot;
        }

        // always set current rotation
        var nextRot = (
            m_Move_Rotation_Curr *
            m_Move_Rotation_Reflect
        );

        m_Body.MoveRotation(nextRot);
    }

    // -- events --
    void OnCollisionEnter(Collision other) {
        // ignore everything besides first contact (bad idea?
        var contact = other.GetContact(0);

        // get current forward based on accumulated rotation (relevant if multiple
        // collisions in a single frame?)
        var currRotation = m_Move_Rotation_Reflect * m_Move_Rotation_Curr;
        var currFwd = currRotation * m_Move_Rotation_InitialForward;

        // reflect direction over the normal (bounce)
        var dirNormal = contact.normal;
        var dirCross = Vector3.ProjectOnPlane(currFwd, dirNormal);
        var dirInline = currFwd - dirCross;
        var dirReflect = dirCross - dirInline;

        // accumulate the reflection
        var rotation = Quaternion.FromToRotation(currFwd, dirReflect);
        m_Move_Rotation_Reflect *= rotation;
    }
}