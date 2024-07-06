using System;
using UnityEngine;

namespace Soil {

/// marks a field as the beginning of a foldout
[AttributeUsage(AttributeTargets.Field)]
public sealed class FoldoutAttribute: PropertyAttribute {
    // -- props --
    /// .
    public readonly string Name;

    // -- lifetime --
    public FoldoutAttribute(string name) {
        Name = name;
    }
}

}