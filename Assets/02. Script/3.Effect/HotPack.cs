using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPack : MonoBehaviour
{
    public float time = 9f;
    private float selectCountdown;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Mathf.Floor(selectCountdown) == time)
        {

        }
        else
        {
            selectCountdown += Time.deltaTime;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }


}