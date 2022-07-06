using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteraction
{
    public string Name
    {
        get;
        set;
    }
    public string Info
    {
        get;
        set;
    }

    public Transform Transform
    {
        get;
    }

}
