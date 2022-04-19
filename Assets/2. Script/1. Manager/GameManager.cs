using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [Header("맵 범위")]
    [SerializeField]
    private float maxY = 0f;
    [SerializeField]
    private float minY = 0f;
    [SerializeField]
    private float maxX = 0f;
    [SerializeField]
    private float minX = 0f;

    public float mxY { get { return maxY; } }
    public float mxX { get { return maxX; } }
    public float mnY { get { return minY; } }
    public float mnX { get { return minX; } }

    [SerializeField]
    private Transform playerslider = null;
    private Transform playerTr = null;
    public Transform Pool { get { return playerslider; } }

    [SerializeField]
    private Player player;
    public Player Player { get { return player; } }

    private bool isGameStart = false;

    public bool IsGameStart { get { return isGameStart; } }

    private readonly Vector2 resetPosition = new Vector2(-9f, 4f);

    #region 세이브에 쓰는 것들
    internal string SAVE_PATH = "";

    private readonly string SAVE_FILENAME = "/SaveFile.txt";
    #endregion

    private SAVE save;

    private float timer;
    public float Timer { get { return timer; } }

    private Transform finish = null;
    private float goalDistance = 0f;

    void Awake()
    {
        playerTr = GameObject.FindWithTag("Player").transform;

        SAVE_PATH = Application.dataPath + "/Save";
        if (!Directory.Exists(SAVE_PATH))
            Directory.CreateDirectory(SAVE_PATH);
    }

    void Start()
    {
        finish = GameObject.FindWithTag("Finish").transform;

        StartCoroutine(Save());
        SoundManager.Instance.SetBackGroundSoundClip(BackGroundSoundState.Basic);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
            if (!UIManager.Instance.GetMenuPanelActive())
                    UIManager.Instance.SetSettingMenuActive();

        if (isGameStart == true) //이게 타이머 기능
        {
            timer += Time.deltaTime;
            UIManager.Instance.SetTimerActive(true);
            UIManager.Instance.TimerText();
        }
        else
        {
            UIManager.Instance.SetTimerActive(false);
        }

        goalDistance = playerTr.position.y / (finish.position.y) * 100;
        UIManager.Instance.UpdateGoalPercentText(goalDistance);
    }

    public void ContinueGame()
    {
        SetGameStart(true);
        UIManager.Instance.SetMenuPanelActive();
        ItemSpawnManager.Instance.RockAndRopeRespawn();
        save = LoadJsonFile<SAVE>(SAVE_PATH, SAVE_FILENAME);
        playerTr.position = save.position;
        timer = save.timer;
    }

    public void NewGameCutSceen()
    {
        Reset();
        EventManager.TriggerEvent("First");
        UIManager.Instance.SetMenuPanelActive();
    }
    public void NewGame()
    {
        SetGameStart(true);
        ItemSpawnManager.Instance.RockAndRopeRespawn();
        save = LoadJsonFile<SAVE>(SAVE_PATH, SAVE_FILENAME);
        playerTr.position = save.position;
        timer = save.timer;
    }

    public void QuitGame()
    {
        SavePositionAndTimer();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void SetGameStart(bool isStart)
    {
        isGameStart = isStart;
    }

    public void SetTimeScale(int num)
    {
        Time.timeScale = num;
    }

    IEnumerator Save()
    {
        save = LoadJsonFile<SAVE>(SAVE_PATH, SAVE_FILENAME);
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (isGameStart)
                SavePositionAndTimer();
        }
    }

    public void SavePositionAndTimer()
    {
        save.position = playerTr.position;
        save.timer = timer;
        SaveJson(SAVE_PATH, SAVE_FILENAME, save);
    }

    void Reset()
    {
        save.position = resetPosition;
        save.timer = 0f;
        SaveJson<SAVE>(SAVE_PATH, SAVE_FILENAME, save);

        player.transform.rotation = Quaternion.identity;
    }

    #region Save&Load
    public void SaveJson<T>(string createPath, string fileName, T value)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", createPath, fileName), FileMode.Create);
        string json = JsonUtility.ToJson(value, true);
        byte[] data = Encoding.UTF8.GetBytes(json);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    public T LoadJsonFile<T>(string loadPath, string fileName) where T : new()
    {
        if (File.Exists(string.Format("{0}/{1}.json", loadPath, fileName)))
        {
            FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", loadPath, fileName), FileMode.Open);
            byte[] data = new byte[fileStream.Length];
            fileStream.Read(data, 0, data.Length);
            fileStream.Close();
            string jsonData = Encoding.UTF8.GetString(data);
            return JsonUtility.FromJson<T>(jsonData);
        }
        SaveJson<T>(loadPath, fileName, new T());
        return LoadJsonFile<T>(loadPath, fileName);
    }
    #endregion
}