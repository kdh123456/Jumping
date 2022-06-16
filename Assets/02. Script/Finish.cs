using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.Playables;

public class Finish : MonoBehaviour
{
    [SerializeField]
    Image image;
    [SerializeField]
    float time;
    [SerializeField]
    PlayableDirector playable;
    private TIMELIST timeList;

    private readonly string SAVE_FILENAME = "TimeList.txt";

    private void Start()
    {
        timeList = GameManager.Instance.LoadJsonFile<TIMELIST>(GameManager.Instance.SAVE_PATH, SAVE_FILENAME);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && GameManager.Instance.IsGameStart)
        {
            EventManager.TriggerEvent("Stop");
            GameManager.Instance.SaveJson<SAVE>(GameManager.Instance.SAVE_PATH, GameManager.Instance.SAVE_FILENAME, GameManager.Instance.Save);
            image.DOFade(1, time).OnComplete(() =>
            {
                playable.Play();
                image.DOFade(0, time);
            });
        }
    }

    private void Add()
    {
        for (int i = 0; i < timeList.timeList.Length; i++)
        {
            if (timeList.timeList[i] == 0)
            {
                timeList.timeList[i] = GameManager.Instance.Timer;

                Sort();

                GameManager.Instance.SaveJson<TIMELIST>(GameManager.Instance.SAVE_PATH, SAVE_FILENAME, timeList);
                return;
            }
        }

        timeList.timeList[0] = GameManager.Instance.Timer;

        Sort();

        GameManager.Instance.SaveJson<TIMELIST>(GameManager.Instance.SAVE_PATH, SAVE_FILENAME, timeList);
    }

    void Sort()
    {
        Array.Sort(timeList.timeList);
        Array.Reverse(timeList.timeList);
    }

    public void end()
    {
        GameManager.Instance.Save.isFirst = true;
        UIManager.Instance.SetMenuPanelActive();
        GameManager.Instance.SetGameStart(false);
        GameManager.Instance.Player.GetComponent<PlayerMove>().Reset();
        timeList = GameManager.Instance.LoadJsonFile<TIMELIST>(GameManager.Instance.SAVE_PATH, SAVE_FILENAME);
        Add();
        GameManager.Instance.SaveJson<TIMELIST>(GameManager.Instance.SAVE_PATH, SAVE_FILENAME, timeList);
        GameManager.Instance.SaveJson<SAVE>(GameManager.Instance.SAVE_PATH, GameManager.Instance.SAVE_FILENAME, GameManager.Instance.Save);
    }
}
