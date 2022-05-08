using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPoolData
{
    public List<GameObject> prefabs = new List<GameObject>();

    public List<int> prefabCreateCounts = new List<int>();
}