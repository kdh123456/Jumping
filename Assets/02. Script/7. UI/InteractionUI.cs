using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    private RectTransform RectTransform
    {
        get
        {
            rectTransform = GetComponent<RectTransform>();
            return rectTransform;
        }
    }
    private RectTransform rectTransform;

    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI infoText;

    /// <summary>
    /// 상호작용 UI 키기
    /// </summary>
    /// <param name="interaction"></param>
    public void ActiveUI(IInteraction interaction)
    {
        nameText.text = interaction.Name;
        infoText.text = interaction.Info;
        PositionSetting(interaction);

        gameObject.SetActive(true);

    }

    /// <summary>
    /// 상호작용 UI 끄기
    /// </summary>
    public void InActiveUI()
    {
        gameObject.SetActive(false);
    }

    private void PositionSetting(IInteraction interaction)
    {
        Vector3 point = Camera.main.WorldToScreenPoint(interaction.Transform.position);
        point.y -= 100;
        if(Screen.width - 300 < point.x)
        {
            point.x += point.x - Screen.width - 300;
        }
        else if(300 > point.x)
        {
            point.x += 300 - point.x;
        }
        RectTransform.position = point;
    }
}
