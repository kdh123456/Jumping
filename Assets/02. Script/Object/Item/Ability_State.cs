using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_State : MonoBehaviour, IInteraction
{

    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }

    public string Info
    {
        get
        {
            return info;
        }
        set
        {
            info = value;
        }
    }

    public Transform Transform
    {
        get
        {
            return transform;
        }
    }

    [SerializeField]
    private string name = "";

    [SerializeField]
    private string info = "";

    [SerializeField]
    private AbilityState state;

    public AbilityState abilityState { get { return state; } }
}
