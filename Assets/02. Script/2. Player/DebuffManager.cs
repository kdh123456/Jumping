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

    private PlayerMove playerMove;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GameManager.Instance.Player.GetComponent<SpriteRenderer>();
        playerMove = GetComponent<PlayerMove>();
    }

    void Update()
    {
        if (GameManager.Instance.IsGameStart)
        {
            if (!isDebuff)
            {
                if (!playerMove.IsFaint)
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
                    // cc?轅붽틓??????????ル뭸????????????
                    break;
                case SeasonState.FALL:
                    Debug.Log("???????袁ⓦ걤???ш낄猷???");
                    //EventManager.TriggerEvent("") ??轅붽틓??????????????????ル늉????轅붽틓????筌뤾쑴裕?棺堉?뙴???
                    // ?????붺몭???????????ル뒌????
                    break;
                case SeasonState.WINTER:
                    Debug.Log("?????袁⑸즴筌??");
                    // cc?饔낅떽????????????ル?????????????
                    break;
            }
            EventManager.TriggerEvent("Faint");
            value = 0;
            spriteRenderer.color = Color.white;
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

    //    // ?轅붽틓???壤굿?怨쀫ぁ??????轅붽틓????????????붺몭?⑸쨨???醫롮첂??貫??????????ル∥堉?-> ??
    //    // ?轅붽틓???壤굿?怨쀫ぁ????????轅붽틓????????????붺몭?⑸쨨???醫롮첂??貫??????????ル∥堉?-> ????
    //    // ?轅붽틓???壤굿?怨쀫ぁ????ル봿???????轅붽틓????????????붺몭?⑸쨨???醫롮첂??貫??????????ル∥堉?-> ???ル봿????
    //    // ?轅붽틓???壤굿?怨쀫ぁ???β뼯援?硫⑸궔?????轅붽틓????????????붺몭?⑸쨨???醫롮첂??貫??????????ル∥堉?-> ??β뼯援?硫⑸궔??
    //}
}
