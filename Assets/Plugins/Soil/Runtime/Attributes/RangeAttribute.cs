using System;
using UnityEngine;

namespace Soil {

/// marks a field as a range (and doesn't complain about data type)
[AttributeUsage(AttributeTargets.Field)]
public sealed class RangeAttribute: PropertyAttribute {
    // -- props --
    /// .
    public readonly float Min;

    /// .
    public readonly float Max;

    // -- lifetime --
    public RangeAttribute(float min, float max) {
        Min = min;
        Max = max;
    }
}

}