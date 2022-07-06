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
        Movemenet();
    }

    public void Movemenet()
    {
        targetPos = playerTr.position;
        count = (int)targetPos.y / _height;
        this.transform.position = new Vector3(0, (_height * count) + 10, -10);
        Debug.Log("Movement!");
    }
}
