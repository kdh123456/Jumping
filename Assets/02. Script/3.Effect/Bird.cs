using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bird : MonoBehaviour
{
    [SerializeField]
    private Vector2 latePosition;

    private Vector2 originPosition;

    private void Start()
    {
        originPosition = transform.position;
        MoveLatePos();
    }

    private void MoveLatePos()
    {
        float time = Vector2.Distance(latePosition, originPosition) / 2;
        transform.DOMove(latePosition, time).SetEase(Ease.Linear).SetDelay(3).OnComplete(() => MoveOriginPos());
    }

    private void MoveOriginPos()
    {
        float time = Vector2.Distance(latePosition, originPosition) / 2;
        transform.DOMove(originPosition, time).SetEase(Ease.Linear).SetDelay(3).OnComplete(() => MoveOriginPos());
    }


}
