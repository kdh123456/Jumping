using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField]
    private GameObject menuPanel = null;
    [SerializeField]
    private GameObject settingMenu = null;
    [SerializeField]
    private Text timerText = null;
    [SerializeField]
    private Text goalPercentText = null;
    [SerializeField]
    private ScrollRect settingScrollView = null;

    public Text[] txt;
    void Start()
    {
        UpdateKey();
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
    }

    public bool GetMenuPanelActive()
    {
        return menuPanel.activeSelf;
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

    public void SetScrollView()
    {
        //settingScrollView.
    }
}
