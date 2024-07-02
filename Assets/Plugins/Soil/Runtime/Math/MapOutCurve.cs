using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Soil {

/// a normalized curve with a min & max value
[Serializable]
public struct MapOutCurve: FloatTransform {
    // -- fields --
    [FormerlySerializedAs("m_Curve")]
    [Tooltip("the curve")]
    public AnimationCurve Curve;

    [FormerlySerializedAs("m_Dst")]
    [Tooltip("the destination range")]
    public FloatRange Dst;

    // -- FloatTransform --
    public float Evaluate(float input) {
        return MapCurve.Evaluate(Curve, Dst, input);
    }

    // -- debug --
    public override string ToString() {
        return $"<MapOutCurve dst={Dst}>";
    }
}

}