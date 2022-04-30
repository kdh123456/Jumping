using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFloorCheck : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Floor"))
        {
            SoundManager.Instance.SetEffectSoundClip(EffectSoundState.Land);
            EventManager.TriggerEvent("FloorCheck");
        }
    }
}
