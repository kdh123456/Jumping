using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Transform playerTr = null;
    private Vector3 targetPos = Vector3.zero;

    private const int _height = 20;
    private int count = 0;
    void Start()
    {
        playerTr = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        #region 플레이어에 딱 붙어서 가기
        //targetPos = playerTr.position;
        //targetPos.x = 0;
        //targetPos.y = Mathf.Clamp(targetPos.y, 0, 10000000/*이 숫자는 후에 조정*/);
        //targetPos.z = -10;
        //transform.position = targetPos;
        #endregion

        #region 약간 느리게 따라가기
        //targetPos = playerTr.position;
        //targetPos.y = Mathf.Clamp(targetPos.y, 0, 10000000/*이 숫자는 후에 조정*/);
        //transform.position = Vector3.Lerp(transform.position, new Vector3(0, targetPos.y, -10), .5f/*이 숫자 조정해서 따라오는 속도 조절 가능*/);
        #endregion

        #region 점프킹 방식
        Movememet();
        #endregion
    }

    public void Movememet()
    {
        targetPos = playerTr.position;
        count = (int)targetPos.y / _height;
        this.transform.position = new Vector3(0, (_height * count) + 10, -10);
    }
}
