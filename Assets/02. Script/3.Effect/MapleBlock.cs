using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MapleBlock : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            this.GetComponent<SpriteRenderer>().DOFade(0, .3f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            this.GetComponent<SpriteRenderer>().DOFade(1, .3f);
        }
    }
}
