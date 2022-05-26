using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    public void Reset()
    {
        value = 0f;
        //spriteRenderer.DOColor(Color.white, .1f);
        spriteRenderer.color = Color.white;
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
                    // cc?耀붾굝???????????????????????????
                    break;
                case SeasonState.FALL:
                    Debug.Log("????????ш끽踰椰?????袁ㅻ쇀???");
                    //EventManager.TriggerEvent("") ??耀붾굝??????????????????????ル뭽????耀붾굝??????癲ル슢???븐쭍?黎앸럽??????
                    // ??????븍툖??????????????ル?????
                    break;
                case SeasonState.WINTER:
                    Debug.Log("??????ш끽維뽳쭩?뱀땡??");
                    // cc??遺얘턁???????????????????????????
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
                IsDown = false;
                break;
            case "Summer_1":
                UpdateSeason(SeasonState.SUMMER_1);
                IsDown = false;
                break;
            case "Fall":
                UpdateSeason(SeasonState.FALL);
                break;
            case "Winter":
                UpdateSeason(SeasonState.WINTER);
                break;
        }
    }
}
