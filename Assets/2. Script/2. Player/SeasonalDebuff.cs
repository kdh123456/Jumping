using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonalDebuff : MonoBehaviour
{
    private float value = 0f;

    private SeasonState STATE;

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
    void Start()
    {
        //UpdateSeason(SeasonState.SUMMER);
    }

    void Update()
    {
        if (GameManager.Instance.IsGameStart)
        {
            if (!isDebuff)
            {
                switch (STATE)
                {
                    case SeasonState.SUMMER:
                    case SeasonState.WINTER:
                        value = isDown ? value - Time.deltaTime : value + Time.deltaTime;
                        //value += Time.deltaTime;
                        break;
                    case SeasonState.SPRING:
                    case SeasonState.FALL:
                        value = 0;
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
            switch (STATE)
            {
                case SeasonState.SUMMER:
                    Debug.Log("기절");
                    // 여기서 cc에 맞는 애니메이션 출력
                    break;
                case SeasonState.WINTER:
                    Debug.Log("빙결");
                    // 여기서 cc에 맞는 애니메이션 출력
                    break;
            }
            EventManager.TriggerEvent("Faint");
            value = 0;
        }
    }

    public void UpdateSeason(SeasonState state)
    {
        STATE = state;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Spring":
                UpdateSeason(SeasonState.SPRING);
                break;
            case "Summer":
                UpdateSeason(SeasonState.SUMMER);
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
