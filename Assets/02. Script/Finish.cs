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

    private void Start()
    {
        GameManager.Instance.Save = GameManager.Instance.LoadJsonFile<SAVE>(GameManager.Instance.SAVE_PATH, GameManager.Instance.SAVE_FILENAME);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            EventManager.TriggerEvent("Stop");
            GameManager.Instance.SaveJson<SAVE>(GameManager.Instance.SAVE_PATH, GameManager.Instance.SAVE_FILENAME, GameManager.Instance.Save);
            image.DOFade(1,time).OnComplete(()=>{
                playable.Play();
                image.DOFade(0,time);
            });
        }
    }

    private void Add()
    {
        if (IsEmpty())
        {
            for (int i = 0; i < GameManager.Instance.Save.timeList.Length; i++)
            {
                if (GameManager.Instance.Save.timeList[i] != 0)
                {
                    GameManager.Instance.Save.timeList[i] = GameManager.Instance.Timer;
                    return;
                }
            }
        }
        else
        {
            GameManager.Instance.Save.timeList[9] = GameManager.Instance.Timer;
        }

        Sort();

        GameManager.Instance.SaveJson<SAVE>(GameManager.Instance.SAVE_PATH, GameManager.Instance.SAVE_FILENAME, GameManager.Instance.Save);
    }

    bool IsEmpty()
    {
        for(int i = 0; i < GameManager.Instance.Save.timeList.Length; i++)
        {
            if (GameManager.Instance.Save.timeList[i] == 0)
                return true;
            else
                return false;
        }

        return false;
    }

    void Sort()
    {
        Array.Sort(GameManager.Instance.Save.timeList);
    }

    public void end()
    {
        UIManager.Instance.SetMenuPanelActive();
        GameManager.Instance.SetGameStart(false);
        EventManager.TriggerEvent("RESET");
    }
}
