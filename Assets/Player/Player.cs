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
    Quaternion m_Move_Rotation_Src;

    /// the final rotation of the move
    Quaternion m_Move_Rotation_Dst;

    /// the current rotation of the move
    Quaternion m_Move_Rotation_Curr;

    /// the current timer for the reflection
    EaseTimer m_Move_Reflect;

    /// the initial rotation when the reflection began
    Quaternion m_Move_Reflect_Src;

    /// the final rotation of the reflection
    Quaternion m_Move_Reflect_Dst;

    /// the current rotation of the reflection
    Quaternion m_Move_Reflect_Curr;

    /// the initial forward direction
    Vector3 m_InitialForward;

    /// the initial rotation
    Quaternion m_InitialRotation;

    // -- lifecycle --
    void Start() {

        m_Move = new EaseTimer();

        m_Move_Rotation = new EaseTimer(new(0f, m_Tuning.Move_Rotation_Curve));
        m_Move_Rotation_Curr = Quaternion.identity;

        m_Move_Reflect = new EaseTimer(m_Tuning.Move_Reflect);
        m_Move_Reflect_Curr = Quaternion.identity;

        var trs = transform;
        m_InitialForward = trs.forward;
        m_InitialRotation = trs.rotation;
    }

    void Update() {
        var isMoveJustPressed = Keyboard.current.spaceKey.wasPressedThisFrame;
        if (m_Move.IsInactive && isMoveJustPressed) {
            // start move
            m_Move.Duration = m_Tuning.Move_Duration.Sample();
            m_Move.Start();

            // accumulate the move rotation
            var currMoveRot = m_Move_Rotation_Curr;
            var nextForward = Random.insideUnitCircle.normalized.XZ();
            var nextMoveRot = currMoveRot * Quaternion.LookRotation(nextForward, Vector3.up);

            // start the move rotation
            m_Move_Rotation_Src = currMoveRot;
            m_Move_Rotation_Dst = nextMoveRot;
            m_Move_Rotation.Duration = m_Tuning.Move_Rotation_Duration.Sample();
            m_Move_Rotation.Start();
        }
    }

    void FixedUpdate() {
        var delta = Time.deltaTime;

        var currMoveRot = m_Move_Rotation_Curr;
        var currReflectRot = m_Move_Reflect_Curr;
        var nextMoveRot = currMoveRot;
        var nextReflectRot = currReflectRot;

        // while active, move pos (towards reflection destination)
        if (m_Move.TryTick()) {
            var currPos = m_Body.position;
            var currFwd = currMoveRot * currReflectRot * m_InitialForward;
            var moveLen = m_Tuning.Move_Speed.Evaluate(m_Move.Pct) * delta;
            var nextPos = currPos + moveLen * currFwd;

            m_Body.MovePosition(nextPos);
        }

        // while active, rotate along move curve
        if (m_Move_Rotation.TryTick()) {
            nextMoveRot = Quaternion.Slerp(
                m_Move_Rotation_Src,
                m_Move_Rotation_Dst,
                m_Move_Rotation.Pct
            );
        }

        // while active, reflect off of collision
        if (m_Move_Reflect.TryTick()) {
            nextReflectRot = Quaternion.Slerp(
                m_Move_Reflect_Src,
                m_Move_Reflect_Dst,
                m_Move_Reflect.Pct
            );
        }

        // rotate to interpolated rotation
        var nextRot = (
            nextMoveRot *
            nextReflectRot *
            m_InitialRotation
        );

        m_Move_Rotation_Curr = nextMoveRot;
        m_Move_Reflect_Curr = nextReflectRot;
        m_Body.MoveRotation(nextRot);
    }

    // -- events --
    void OnCollisionEnter(Collision other) {
        // ignore everything besides first contact (bad idea?
        var contact = other.GetContact(0);

        // get current forward based on accumulated rotation (relevant if multiple
        // collisions in a single frame?)
        var currRotation = m_Move_Reflect_Curr * m_Move_Rotation_Curr;
        var currFwd = currRotation * m_InitialForward;

        // reflect over the normal (bounce)
        var dirNormal = contact.normal;
        var dirCross = Vector3.ProjectOnPlane(currFwd, dirNormal);
        var dirInline = currFwd - dirCross;
        var dirReflect = dirCross - dirInline;

        // accumulate the reflection
        var currReflect = m_Move_Reflect_Curr;
        var nextReflect = currReflect * Quaternion.FromToRotation(currFwd, dirReflect);

        // start the reflection
        m_Move_Reflect_Src = currReflect;
        m_Move_Reflect_Dst = nextReflect;
        m_Move_Reflect.Start();
    }
}