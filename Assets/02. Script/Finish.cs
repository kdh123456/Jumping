using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem fireWorks;

    private void Start()
    {
        fireWorks.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            UIManager.Instance.SetMenuPanelActive();
            GameManager.Instance.SetGameStart(false);
            fireWorks.gameObject.SetActive(true);
            fireWorks.Play();
        }
    }
}
