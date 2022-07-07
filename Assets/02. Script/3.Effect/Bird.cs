using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bird : MonoBehaviour
{
    [SerializeField]
    private Vector2 offset;

    [SerializeField]
    private float speed;

    private Vector2 originPosition;
    private Vector2 latePosition;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        originPosition = transform.position;
        latePosition = originPosition + offset;
        MoveLatePos();
    }

    private void MoveLatePos()
    {
        spriteRenderer.flipX = false;
        float time = Vector2.Distance(latePosition, originPosition) / speed;
        transform.DOMove(latePosition, time).SetEase(Ease.Linear).SetDelay(3).OnComplete(() => MoveOriginPos());
    }

    private void MoveOriginPos()
    {
        spriteRenderer.flipX = true;
        float time = Vector2.Distance(latePosition, originPosition) / 2;
        transform.DOMove(originPosition, time).SetEase(Ease.Linear).SetDelay(3).OnComplete(() => MoveOriginPos());
    }

    private void OnDrawGizmos()
    {
        latePosition = originPosition + offset;
        Debug.DrawRay(transform.position, (Vector3)latePosition - transform.position);
    }
}
