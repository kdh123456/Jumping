using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Leaf : ItemEffect
{
    private SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        CreateEffect();
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void CreateEffect()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer.color.a == 1)
            spriteRenderer.color = new Color(1, 1, 1, 0);
        spriteRenderer.DOFade(1, .2f);
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
