using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Soil {

/// a normalized curve with a max value
[Serializable]
public struct MapInMaxCurve: FloatTransform {
    // -- fields --
    [Tooltip("the curve")]
    public AnimationCurve Curve;

    [Tooltip("the source maximum")]
    public float Src;

    // -- FloatTransform --
    public float Evaluate(float input) {
        return MapCurve.Evaluate(Curve, input / Src);
    }

    // -- queries --
    /// the maximum value of the input range
    public float Max {
        get => Src;
    }

    // -- debug --
    public override string ToString() {
        return $"<MapInMaxCurve Src={Src}>";
    }
}

}