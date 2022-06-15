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

    private void Start()
    {
        rankIsEmptyText.gameObject.SetActive(false);

        CreateBoard();
    }

    public void CreateBoard()
    {
        GameManager.Instance.Save = GameManager.Instance.LoadJsonFile<SAVE>(GameManager.Instance.SAVE_PATH, GameManager.Instance.SAVE_FILENAME);

        if (GameManager.Instance.Save.timeList[0] == 0) // Rank is Empty!
        {
            rankIsEmptyText.gameObject.SetActive(true);
            return;
        }
        else
        {
            rankIsEmptyText.gameObject.SetActive(false);
        }

        for (int i = 1; i <= GameManager.Instance.Save.timeList.Length; i++)
        {
            

            GameObject board = ObjectPool.Instance.GetObject(PoolObjectType.BOARD);
            board.transform.SetParent(createPoint);
            board.transform.position = Vector3.zero;

            board.transform.GetChild(0).GetComponent<Text>().text = i.ToString();
            board.transform.GetChild(1).GetComponent<Text>().text = string.Format("{0:F0}", GameManager.Instance.Save.timeList[i - 1]);
        }
    }

    public void RemoveBoard()
    {
        for(int i = 0; i < createPoint.childCount; i++)
        {
            ObjectPool.Instance.ReturnObject(PoolObjectType.BOARD, createPoint.GetChild(i).gameObject);
        }
    }
}
