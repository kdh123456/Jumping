using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoSingleton<PlayerStateManager>
{
    private PlayerState STATE;
    public PlayerState PlayerState { get { return STATE; } }

    void Awake()
    {
        UpdateState(PlayerState.BASIC);
    }

    public void UpdateState(PlayerState state)
    {
        STATE = state;
    }
}
