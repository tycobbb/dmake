using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Soil {

/// a normalized curve with a source range
[Serializable]
public struct MapInCurve: FloatTransform {
    // -- fields --
    [FormerlySerializedAs("m_Curve")]
    [Tooltip("the curve")]
    public AnimationCurve Curve;

    [FormerlySerializedAs("m_Src")]
    [Tooltip("the source range")]
    public FloatRange Src;

    // -- FloatTransform --
    public float Evaluate(float input) {
        return MapCurve.Evaluate(Curve, Src, input);
    }

    // -- debug --
    public override string ToString() {
        return $"<MapInCurve src={Src}>";
    }
}

}