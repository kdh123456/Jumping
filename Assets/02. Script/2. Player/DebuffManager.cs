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
        set => this.value = value;
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
        }
    }

    private PlayerMove playerMove;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid;

    private void Start()
    {
        rigid = GameManager.Instance.Player.GetComponent<Rigidbody2D>();
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

            UIManager.Instance.ValueSliderActive(state == SeasonState.SUMMER_0 || state == SeasonState.SUMMER_1 || state == SeasonState.FALL);
            UIManager.Instance.ValueSliderValue(value);
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
        UIManager.Instance.ValueSliderActive(false);
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
                    // cc????????????????????????????????????
                    break;
                case SeasonState.FALL:
                    Debug.Log("???????????????≤Îê±Îπ???Ô¶??‰∫?ªãÍº????????????????????");
                    float random = Random.Range(0, 2);
                    rigid.AddForce((random == 0 ? Vector2.left : Vector2.right) * 20, ForceMode2D.Impulse);
                    //EventManager.TriggerEvent("") ??????????????????????????????????????????????????????????????????????????Î∏êÎºê?Í≥?∂Ø?Á´?????????ºÎí≠?????????Í≥????????????âÎ®Æ?????????ÍøîÍ∫Ç????∞Í∑•?ñÔßç????????
                    // ????????????????????????????????????
                    break;
                case SeasonState.WINTER:
                    Debug.Log("???????????????Íæ©Î£ÜÔß?ù∞Ï≠???µæ??????????????Ô¶´ÎöÆÏ£??ÅÏ†ò???????????????");
                    // cc????????????????????????????????????
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Spring":
                break;
            case "Summer_0":
                IsDown = false;
                break;
            case "Summer_1":
                IsDown = false;
                break;
            case "Fall":
                break;
            case "Winter":
                break;
        }
    }
}
