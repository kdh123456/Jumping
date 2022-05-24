using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffManager : MonoSingleton<DebuffManager>
{
    private float value = 0f;
    public float Value
    {
        get => value;
    }

    private SeasonState state;
    public SeasonState State
    {
        get => state;
    }

    private readonly int MIN_VALUE = 0;
    private readonly int MAX_VALUE = 20;
    public int MAXVALUE
    {
        get => MAX_VALUE;
    }

    private bool isDebuff = false;
    public bool IsDebuff
    {
        get
        {
            return isDebuff;
        }
        set
        {
            isDebuff = value;
        }
    }
    private bool isDown = false;
    public bool IsDown
    {
        get => isDown;
        private set
        {
            isDown = value;
            EventManager.TriggerEvent("ColorChange");
        }
    }

    void Update()
    {
        if (GameManager.Instance.IsGameStart)
        {
            if (!isDebuff)
            {
                switch (state)
                {
                    case SeasonState.SUMMER_0:
                    case SeasonState.SUMMER_1:
                    case SeasonState.WINTER:
                        value = Mathf.Clamp(isDown ? value - Time.deltaTime : value + Time.deltaTime, MIN_VALUE, MAX_VALUE);
                        break;
                    case SeasonState.SPRING:
                        value = 0;
                        break;
                    case SeasonState.FALL:
                        value = Mathf.Clamp(isDown ? value + Time.deltaTime : 0, MIN_VALUE, MAX_VALUE);
                        break;
                }
            }

            Debuff();
        }
    }

    public void UpdateDown(bool b)
    {
        IsDown = b;
    }

    void Debuff()
    {
        if (isDebuff) return;
        if (value >= MAX_VALUE)
        {
            isDebuff = true;
            switch (state)
            {
                case SeasonState.SUMMER_0:
                    Debug.Log("????????");
                    EventManager.TriggerEvent("Faint");
                    value = 0;
                    GameManager.Instance.Player.GetComponent<SpriteRenderer>().color = Color.white;
                    // cc?²ãƒ«?????????«ëŠ…?????¥Â€?¬íˆ“??
                    break;
                case SeasonState.FALL:
                    Debug.Log("???Î¼???„ëª­??è¢ã‰???");
                    //EventManager.TriggerEvent("") ??²ãƒ«??º‰????????æ½?????«ë”†????²ãƒ«????ëª„ì’œ?¥â–²ë£???
                    // ??äº?»‹ê¼?¶­????????«ë´¿????
                    break;
                case SeasonState.WINTER:
                    Debug.Log("?????„ì…ì±??");
                    EventManager.TriggerEvent("Faint");
                    value = 0;
                    // cc?ê¿”êº‚??????????«ë’‡????????????
                    break;
            }
            isDebuff = false;
        }
    }

    public void UpdateSeason(SeasonState state)
    {
        this.state = state;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Spring":
                UpdateSeason(SeasonState.SPRING);
                break;
            case "Summer_0":
                UpdateSeason(SeasonState.SUMMER_0);
                break;
            case "Summer_1":
                UpdateSeason(SeasonState.SUMMER_1);
                break;
            case "Fall":
                UpdateSeason(SeasonState.FALL);
                break;
            case "Winter":
                UpdateSeason(SeasonState.WINTER);
                break;
        }
    }

    //private void Test()
    //{
    //    Transform playerTr = GameManager.Instance.Player.transform;

    //    // ?²ãƒ«???ç¯€?°ì‘??????²ãƒ«?????????äº?»‹ê¼¨æ´??? ì¥‰??¥ë‰©????????ì¢Žë–µ?-> ??
    //    // ?²ãƒ«???ç¯€?°ì‘????????²ãƒ«?????????äº?»‹ê¼¨æ´??? ì¥‰??¥ë‰©????????ì¢Žë–µ?-> ????
    //    // ?²ãƒ«???ç¯€?°ì‘???ì¢Šìˆ??????²ãƒ«?????????äº?»‹ê¼¨æ´??? ì¥‰??¥ë‰©????????ì¢Žë–µ?-> ??ì¢Šìˆ???
    //    // ?²ãƒ«???ç¯€?°ì‘??æ¿¡ã‚‹?¸æ³³?????²ãƒ«?????????äº?»‹ê¼¨æ´??? ì¥‰??¥ë‰©????????ì¢Žë–µ?-> ?æ¿¡ã‚‹?¸æ³³??
    //}
}
