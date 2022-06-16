using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    [SerializeField]
    private Transform createPoint;
    [SerializeField]
    private Text rankIsEmptyText;

    private TIMELIST timeList;
    private readonly string SAVE_FILENAME = "TimeList.txt";

    private void Start()
    {
        rankIsEmptyText.gameObject.SetActive(false);

        timeList = GameManager.Instance.LoadJsonFile<TIMELIST>(GameManager.Instance.SAVE_PATH, SAVE_FILENAME);

        CreateBoard();
    }

    public void CreateBoard()
    {
        RemoveBoard();

        timeList = GameManager.Instance.LoadJsonFile<TIMELIST>(GameManager.Instance.SAVE_PATH, SAVE_FILENAME);

        if (timeList.timeList[0] == 0) // Rank is Empty!
        {
            rankIsEmptyText.gameObject.SetActive(true);
            return;
        }
        else
        {
            rankIsEmptyText.gameObject.SetActive(false);
        }

        for (int i = 1; i <= timeList.timeList.Length; i++)
        {
            if (timeList.timeList[i - 1] != 0)
            {
                GameObject board = ObjectPool.Instance.GetObject(PoolObjectType.BOARD);
                board.transform.SetParent(createPoint);
                board.transform.position = Vector3.zero;

                board.transform.GetChild(0).GetComponent<Text>().text = i.ToString();
                board.transform.GetChild(1).GetComponent<Text>().text = string.Format("{0:F0}", timeList.timeList[i - 1]);
            }
        }
    }

    public void RemoveBoard()
    {
        int childCnt = createPoint.childCount;
        if (childCnt == 0) return;

        for(int i = 0; i < childCnt; i++)
        {
            ObjectPool.Instance.ReturnObject(PoolObjectType.BOARD, createPoint.GetChild(0).gameObject);
        }
    }
}
