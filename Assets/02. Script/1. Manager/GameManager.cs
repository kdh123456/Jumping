using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;
using System.Reflection;

public class GameManager : MonoSingleton<GameManager>
{
    [Header("?????????")]
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
    private bool isCutscene = false;
    public bool IsCutscene
    {
        get => isCutscene;
        set => isCutscene = value;
    }

    public bool IsGameStart { get { return isGameStart; } }

    private readonly Vector2 resetPosition = new Vector2(-9f, 4f);

    #region Save
    internal string SAVE_PATH = "";
    internal readonly string SAVE_FILENAME = "/SaveFile.txt";
    #endregion

    private SAVE save;
    public CUTSCENE cUTSCENE;
    public SAVE Save { get => save; set => save = value; }

    private float timer;
    public float Timer { get { return timer; } }

    private Transform finish = null;
    private float goalDistance = 0f;

    private static WaitForSeconds waitForSeconds10 = new WaitForSeconds(1);

    void Awake()
    {
        playerTr = GameObject.FindWithTag("Player").transform;

        SAVE_PATH = Application.dataPath + "/Save";
        if (!Directory.Exists(SAVE_PATH))
            Directory.CreateDirectory(SAVE_PATH);

        DOTween.Init().SetCapacity(500, 50);
    }

    void Start()
    {
        finish = GameObject.FindWithTag("Finish").transform;

        //StartCoroutine(SaveCoroutine());
        save = LoadJsonFile<SAVE>(SAVE_PATH, SAVE_FILENAME);
        //SoundManager.Instance.SetBackGroundSoundClip(BackGroundSoundState.Basic);
        //SoundManager.Instance.SetBackGroundSoundClip(BackGroundSoundState.SpringBgm);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (!UIManager.Instance.GetMenuPanelActive() && !UIManager.Instance.GetSettingPanelActive())
            {
                UIManager.Instance.SetSettingMenuActive();
                SoundManager.Instance.StopBgmSound();
            }

        if (isGameStart == true && isCutscene == false) // ??????????? ??????????????????????????????
        {
            timer += Time.deltaTime;
            //UIManager.Instance.SetTimerActive(true);
            UIManager.Instance.TimerText();
        }
        else
        {
            //UIManager.Instance.SetTimerActive(false);
        }

        goalDistance = playerTr.position.y / (finish.position.y) * 100;
        UIManager.Instance.UpdateGoalPercentText(goalDistance);
    }

    public void ContinueGame()
    {
        SetGameStart(true);
        SoundManager.Instance.SetBackGroundSoundClip(BackGroundSoundState.Basic);
        //SoundManager.Instance.SetBackGroundSoundClip(BackGroundSoundState.SpringBgm);
        UIManager.Instance.SetMenuPanelActive();
        ItemSpawnManager.Instance.RockAndRopeRespawn();
        player.GetComponent<PlayerMove>().Reset();
        save = LoadJsonFile<SAVE>(SAVE_PATH, SAVE_FILENAME);
        playerTr.position = save.position;
        timer = save.timer;

        //StartCoroutine(CameraMove());
        MethodInfo mInfo = typeof(CameraMove).GetMethod("Movemenet", BindingFlags.Public);
        mInfo?.Invoke(typeof(CameraMove), null);
    }

    public void NewGameCutSceen()
    {
        Reset();
        player.GetComponent<PlayerMove>().Reset();
        save = LoadJsonFile<SAVE>(SAVE_PATH, SAVE_FILENAME);
        playerTr.position = save.position;
        UIManager.Instance.SetMenuPanelActive();
        //isCutscene = false;
    }
    public void NewGame()
    {
        if(save.isFirst == true)
        {
            save.isFirst = false;
            SaveJson<SAVE>(SAVE_PATH, SAVE_FILENAME, save);
        }
        SoundManager.Instance.SetBackGroundSoundClip(BackGroundSoundState.Basic);
        //SoundManager.Instance.SetBackGroundSoundClip(BackGroundSoundState.SpringBgm);
        SetGameStart(true);
        ItemSpawnManager.Instance.RockAndRopeRespawn();
        timer = save.timer;

        StartCoroutine(CameraMove());
    }

    private IEnumerator CameraMove()
    {
        yield return new WaitForSeconds(.5f);
        MethodInfo mInfo = typeof(CameraMove).GetMethod("Movemenet", BindingFlags.Public);
        mInfo?.Invoke(typeof(CameraMove), null);
        Debug.Log("Success MethodInfo");
    }

    public void QuitGame()
    {
        //SavePositionAndTimer();

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

    IEnumerator SaveCoroutine()
    {
        save = LoadJsonFile<SAVE>(SAVE_PATH, SAVE_FILENAME);
        while (true)
        {
            yield return waitForSeconds10;
            if (isGameStart)
                SavePositionAndTimer();
        }
    }

    public void SavePositionAndTimer()
    {
        save.position = playerTr.position;
        save.timer = timer;
        SaveJson(SAVE_PATH, SAVE_FILENAME, save);
        Debug.Log("Save!");
    }

    public void Reset()
    {
        save.position = resetPosition;
        save.timer = 0f;

        isGameStart = true;
        timer = 0;
        UIManager.Instance.TimerText();
        goalDistance = playerTr.position.y / (finish.position.y) * 100;
        UIManager.Instance.UpdateGoalPercentText(goalDistance);
        player.transform.rotation = Quaternion.identity;
        PlayerStateManager.Instance.UpdateState(PlayerState.BASIC);
        player.GetComponent<PlayerMove>().UpdateAnimator();
        SaveJson<SAVE>(SAVE_PATH, SAVE_FILENAME, save);
        EventManager.TriggerEvent("RESET");
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
