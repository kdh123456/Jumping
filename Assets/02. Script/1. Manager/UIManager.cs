using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using static Define;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField]
    private GameObject menuPanel = null;
    [SerializeField]
    private GameObject settingMenu = null;
    [SerializeField]
    private GameObject settingScrollRect = null;
    [SerializeField]
    private Text timerText = null;
    [SerializeField]
    private Text goalPercentText = null;
    [SerializeField]
    private Slider valueSlider = null;
    [SerializeField]
    private Toggle toggle = null;
    [SerializeReference]
    private GameObject backPanel = null;
    [SerializeField]
    private Text continueText = null;
    [SerializeField]
    private GameObject skipButton = null;
    [SerializeField]
    private PlayableDirector playableDirector;

    public Text[] txt;
    void Start()
    {
        UpdateKey();
    }

    private void Update()
    {
        Vector2 pos = MainCam.WorldToScreenPoint(GameManager.Instance.Player.transform.position + Vector3.up * 1.5f);
        valueSlider.transform.position = pos;

        if (GameManager.Instance.IsGameStart)
        {
            if (settingMenu.activeSelf || GetSettingPanelActive())
            {
                backPanel.SetActive(true);
            }
            else
            {
                backPanel.SetActive(false);
            }
        }

        if(GameManager.Instance.Save.isFirst == true)
        {
            continueText.gameObject.SetActive(false);
        }
        else
        {
            continueText.gameObject.SetActive(true);
        }
    }

    public void UpdateKey()
    {
        for (int i = 0; i < txt.Length; i++)
        {
            txt[i].text = KeySetting.keys[(KeyAction)i].ToString();
        }
    }

    public void UpdateGoalPercentText(float distance)
    {
        if (distance <= 0)
            goalPercentText.text = "0%";
        else if(distance >= 100)
            goalPercentText.text = "100%";
        else
            goalPercentText.text = string.Format("{0:F0}%", distance);
    }

    public void SetMenuPanelActive()
    {
        menuPanel.SetActive(!menuPanel.activeSelf);
        if (Time.timeScale == 0)
            Time.timeScale = 1;

        settingScrollRect.SetActive(false);
    }

    public bool GetMenuPanelActive()
    {
        return menuPanel.activeSelf;
    }

    public bool GetSettingPanelActive()
    {
        return settingScrollRect.activeSelf;
    }

    public void SetSettingMenuActive()
    {
        settingMenu.SetActive(!settingMenu.activeSelf);
        Time.timeScale = settingMenu.activeSelf ? 0 : 1;
    }

    public void TimerText()
    {
        timerText.text = string.Format("{0:N2}", GameManager.Instance.Timer);
    }

    public void SetTimerActive(bool active)
    {
        timerText.gameObject.SetActive(active);
    }

    public void ValueSliderActive(bool isAcvtive)
    {
        valueSlider.gameObject.SetActive(isAcvtive);
    }

    public void ValueSliderValue(float value)
    {
        valueSlider.value = value;
    }

    public void TimerTextActive()
    {
        timerText.gameObject.SetActive(toggle.isOn);
    }

    public void ActuveSkip(bool skip)
    {
        skipButton.SetActive(skip);
    }

    public void Skip()
    {
        playableDirector.Stop();
    }
}
