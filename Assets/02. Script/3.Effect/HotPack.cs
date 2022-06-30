using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPack : MonoBehaviour
{
    public float time = 9f;
    private float selectCountdown = 0f;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Mathf.Floor(selectCountdown) >= time)
        {
            EventManager.TriggerEvent("Hotpack");
            selectCountdown = 0f;
        }
        else
        {
            selectCountdown += Time.deltaTime;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        selectCountdown = 0f;
    }
}
