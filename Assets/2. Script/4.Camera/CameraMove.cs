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
        #region �÷��̾ �� �پ ����
        //targetPos = playerTr.position;
        //targetPos.x = 0;
        //targetPos.y = Mathf.Clamp(targetPos.y, 0, 10000000/*�� ���ڴ� �Ŀ� ����*/);
        //targetPos.z = -10;
        //transform.position = targetPos;
        #endregion

        #region �ణ ������ ���󰡱�
        //targetPos = playerTr.position;
        //targetPos.y = Mathf.Clamp(targetPos.y, 0, 10000000/*�� ���ڴ� �Ŀ� ����*/);
        //transform.position = Vector3.Lerp(transform.position, new Vector3(0, targetPos.y, -10), .5f/*�� ���� �����ؼ� ������� �ӵ� ���� ����*/);
        #endregion

        #region ����ŷ ���
        targetPos = playerTr.position;
        count = (int)targetPos.y / _height;
        this.transform.position = new Vector3(0, (_height * count) + 10, -10);
        #endregion
    }
}
