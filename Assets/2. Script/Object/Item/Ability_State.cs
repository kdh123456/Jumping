using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_State : MonoBehaviour
{
    [SerializeField]
    private AbilityState state;

    public AbilityState abilityState { get { return state; } }
}
