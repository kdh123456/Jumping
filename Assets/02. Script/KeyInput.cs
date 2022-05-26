using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInput : MonoBehaviour
{
    private PlayerState state;

    private PlayerMove playerMove => GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
    void Update()
    {
        if (GameManager.Instance.IsGameStart && (Time.timeScale == 1) && playerMove.IsMove)
        {
            if (Input.GetKeyDown(KeySetting.keys[KeyAction.JUMP]))
            {
                EventManager.TriggerEvent("START");
            }
            if (Input.GetKey(KeySetting.keys[KeyAction.JUMP]))
            {
                EventManager.TriggerEvent("STARTING");
            }
            if (Input.GetKeyUp(KeySetting.keys[KeyAction.JUMP]))
            {
                EventManager.TriggerEvent("STOP");
            }

            if (Input.GetKeyDown(KeySetting.keys[KeyAction.SWALLOW]))
            {
                EventManager.TriggerEvent("Swallow");
            }

            if (Input.GetKeyDown(KeySetting.keys[KeyAction.SKILL]))
            {
                state = PlayerStateManager.Instance.PlayerState;

                switch (state)
                {
                    case PlayerState.FIREBALL:
                        EventManager.TriggerEvent("Fire");
                        break;
                    case PlayerState.LADYBUG:
                        EventManager.TriggerEvent("Umbrella");
                        break;
                    case PlayerState.SMALL:
                        EventManager.TriggerEvent("Small");
                        break;
                    case PlayerState.FLY:
                        EventManager.TriggerEvent("Fly");
                        break;
                    case PlayerState.WATER:
                    EventManager.TriggerEvent("EatWell");
                    break;   
                }
            }
        }
    }
}
