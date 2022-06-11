using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneManager : MonoSingleton<CutSceneManager>
{
    private CUTSCENE saveCutScene;

    // Update is called once per frame
    private readonly string SAVE_FILENAME = "/SaveCutScene.txt";

    public int count { get { return saveCutScene.Count; } }
    public int twoCount { get { return saveCutScene.TwoCount; } }


    private void Awake()
    {
        saveCutScene = GameManager.Instance.LoadJsonFile<CUTSCENE>(GameManager.Instance.SAVE_PATH, SAVE_FILENAME);
    }
    private void Start()
    {
        EventManager.StartListening("CUTSCENEPLUS", PlusCutSceneCount);
        EventManager.StartListening("RESET", Reset);
    }
    private void PlusCutSceneCount()
    {
        saveCutScene.TwoCount++;
        saveCutScene.Count++;
        GameManager.Instance.SaveJson<CUTSCENE>(GameManager.Instance.SAVE_PATH, SAVE_FILENAME, saveCutScene);
    }

    private void Reset()
    {
        saveCutScene.TwoCount = 0;
        saveCutScene.Count = 0;
        GameManager.Instance.SaveJson<CUTSCENE>(GameManager.Instance.SAVE_PATH, SAVE_FILENAME, saveCutScene);
    }
}
