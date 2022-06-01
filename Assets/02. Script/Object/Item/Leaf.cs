using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Leaf : ItemEffect
{
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.color = new Color(1, 1, 1, 0);
    }

    public override void CreateEffect()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(spriteRenderer.DOFade(1, .2f));

        seq.Play();
    }

    public override void DestroyEffect()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(spriteRenderer.DOFade(0, .2f));
        seq.AppendCallback(() => ObjectPool.Instance.ReturnObject(PoolObjectType.LEAF, gameObject));

        seq.Play();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Floor"))
        {
            DestroyEffect();
        }
    }
}
