using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CloudBlock : ItemEffect
{
    private SpriteRenderer spriteRenderer = null;
    private Collider2D col = null;

    [SerializeField]
    private float time = 2f;
    void Awake()
    {
        spriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
        col = transform.parent.GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        CreateEffect();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(DestroyBlock());
        }
    }

    IEnumerator DestroyBlock()
    {
        yield return new WaitForSeconds(time);
        DestroyEffect();

        yield return new WaitForSeconds(5f);
        CreateEffect();
    }

    public override void CreateEffect()
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() => spriteRenderer.color = new Color(1, 1, 1, 0));
        seq.AppendCallback(() => col.enabled = true);
        seq.Join(spriteRenderer.DOFade(1, 1f));
    }

    public override void DestroyEffect()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => col.enabled = false);
        seq.Join(spriteRenderer.DOFade(0, 1f));
        //seq.AppendCallback(() => ObjectPool.Instance.ReturnObject(PoolObjectType.CLOUD, this.transform.parent.gameObject));
    }
}
