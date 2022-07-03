using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutScene : MonoBehaviour
{
    [SerializeField]
    private Vector3[] objvector;
    private void Start()
    {
        EventManager.StartListening("CUTSCENE", CUT);
        EventManager.StartListening("RESET", Reset);
        if (objvector.Length > CutSceneManager.Instance.twoCount)
        {
            this.gameObject.transform.position = objvector[CutSceneManager.Instance.twoCount];
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && GameManager.Instance.IsGameStart)
        {
            CUT();
        }
    }

    private void Reset()
    {
        this.gameObject.transform.position = objvector[0];
    }

    private void CUT()
    {
        EventManager.TriggerEvent("Stop");
        EventManager.TriggerEvent("UPDOWNCUTSCENE");
        EventManager.TriggerEvent("ChangeCameraVector");
        EventManager.TriggerEvent("CUTSCENEPLUS");
        UIManager.Instance.ActuveSkip(true);
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
