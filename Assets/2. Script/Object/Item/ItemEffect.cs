using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemEffect : MonoBehaviour
{
    public abstract void CreateEffect();

    public abstract void DestroyEffect();
}
