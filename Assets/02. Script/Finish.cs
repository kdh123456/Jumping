using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            UIManager.Instance.SetMenuPanelActive();
            GameManager.Instance.SetGameStart(false);
        }
    }
}
