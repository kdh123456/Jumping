using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HotPack : MonoBehaviour
{
    public float time = 9f;
    private float selectCountdown = 0f;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            spriteRenderer.DOColor(Color.red, time);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Mathf.Floor(selectCountdown) >= time)
            {
                EventManager.TriggerEvent("Hotpack");
                spriteRenderer.DOColor(Color.white, .2F);
                selectCountdown = 0f;
            }
            else
            {
                selectCountdown += Time.deltaTime;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            selectCountdown = 0f;
        }
    }
}
