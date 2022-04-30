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
    private readonly int MAX_VALUE = 5;

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
                        value = Mathf.Clamp(isDown ? 0 : value + Time.deltaTime, MIN_VALUE, MAX_VALUE);
                        break;
                }
            }

            Debuff();
        }
    }

    public void UpdateDown(bool b)
    {
        isDown = b;
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
                    Debug.Log("기절");
                    EventManager.TriggerEvent("Faint");
                    // 여기서 cc에 맞는 애니메이션 출력
                    break;
                case SeasonState.FALL:
                    Debug.Log("독 중독");
                    //EventManager.TriggerEvent("") 정의된 함수 가져오기
                    // 점프력 감소 
                    break;
                case SeasonState.WINTER:
                    Debug.Log("빙결");
                    EventManager.TriggerEvent("Faint");
                    // 여기서 cc에 맞는 애니메이션 출력
                    break;
            }
            value = 0;
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
            case "Summer_1":
                UpdateSeason(SeasonState.SUMMER_0);
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

    //    // 만약 봄 끝 지점의 높이보다 낮으면 -> 봄
    //    // 만약 여름 끝 지점의 높이보다 낮으면 -> 여름
    //    // 만약 가을 끝 지점의 높이보다 낮으면 -> 가을
    //    // 만약 겨울 끝 지점의 높이보다 낮으면 -> 겨울
    //}
}
