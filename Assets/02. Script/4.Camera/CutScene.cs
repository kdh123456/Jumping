using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutScene : MonoBehaviour
{
    [SerializeField]
    private Vector3[] objvector;
    private void Start()
    {
        EventManager.StartListening("RESET", Reset);
        if (objvector.Length > CutSceneManager.Instance.twoCount)
        {
            this.gameObject.transform.position = objvector[CutSceneManager.Instance.twoCount];
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.TriggerEvent("MoveStop");
            EventManager.TriggerEvent("UPDOWNCUTSCENE");
            EventManager.TriggerEvent("ChangeCameraVector");
            EventManager.TriggerEvent("CUTSCENEPLUS");
            if (objvector.Length > CutSceneManager.Instance.twoCount)
            {
                this.gameObject.transform.position = objvector[CutSceneManager.Instance.twoCount];
            }
            else
            {
                this.gameObject.transform.position = Vector2.zero;
            }
        }
    }

    private void Reset()
    {
        this.gameObject.transform.position = objvector[0];
    }
}
