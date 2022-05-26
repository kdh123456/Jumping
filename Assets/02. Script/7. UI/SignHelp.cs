using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class SignHelp : MonoBehaviour
{
    private TextMeshPro text;

    public string value;

    private void Awake()
    {
        text = transform.Find("Text").GetComponent<TextMeshPro>();
    }

    void Start()
    {
        text.SetText(value);
        text.color = new Color(1, 1, 1, 0);
        text.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Sequence seq = DOTween.Sequence();
            seq.AppendCallback(() => text.gameObject.SetActive(true));
            seq.Append(text.DOFade(1, .1f));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(text.DOFade(0, .1f));
            seq.AppendCallback(() => text.gameObject.SetActive(false));
        }
    }
}
