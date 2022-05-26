using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MapleBlock : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Material material;

    private static WaitForSeconds waitForSeconds01 = new WaitForSeconds(0.1f);

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        

        if (collision.gameObject.CompareTag("Player"))
        {
            Sequence seq = DOTween.Sequence();
            seq.AppendCallback(() => StartCoroutine(ShakeCoroutine()));
            seq.SetDelay(.1f);
            seq.Append(spriteRenderer.DOFade(0, .2f));
            //this.GetComponent<SpriteRenderer>().DOFade(0, .3f);
            //StartCoroutine(ShakeCoroutine());
            seq.Play().SetAutoKill();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        

        if (collision.gameObject.CompareTag("Player"))
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(spriteRenderer.DOFade(1, .2f));
            seq.SetDelay(.2f);
            seq.AppendCallback(() => StartCoroutine(ShakeCoroutine()));
            //StartCoroutine(ShakeCoroutine());
            //this.GetComponent<SpriteRenderer>().DOFade(1, .3f);
            seq.Play().SetAutoKill();
        }
    }

    private IEnumerator ShakeCoroutine()
    {
        material.SetFloat("_WindSpeed", 2f);
        material.SetFloat("_WindIntensity", .5f);
        yield return waitForSeconds01;
        material.SetFloat("_WindSpeed", 1f);
        material.SetFloat("_WindIntensity", 0.1f);
    }
}
