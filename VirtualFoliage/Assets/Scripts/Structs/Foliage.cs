using UnityEngine;
using System.Collections;
using System;
/// <summary>
/// Matches a foliage prefab with its preferred anchoring
/// </summary>
[Serializable]
public struct Foliage
{
    public enum FoliageType
    {
        Ground, Wall
    }
    public GameObject template;
    public FoliageType type;
}
