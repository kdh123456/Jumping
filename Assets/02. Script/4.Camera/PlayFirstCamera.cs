using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PlayFirstCamera : MonoBehaviour
{
    private static bool isend;
    [SerializeField]
    private PlayableDirector playableDirector;

    [SerializeField]
    private Vector3[] upSceneVector;
    [SerializeField]
    private Vector3[] downSceneVector;

    [SerializeField]
    private GameObject firsCam;
    [SerializeField]
    private GameObject secoundCam;

    private void Awake()
    {
        if (downSceneVector.Length > CutSceneManager.Instance.count)
        {
            firsCam.transform.position = downSceneVector[CutSceneManager.Instance.count];
            secoundCam.transform.position = upSceneVector[CutSceneManager.Instance.count];
        }
    }
    void Start()
    {
        EventManager.StartListening("UPDOWNCUTSCENE",UpDownCutScene);
        EventManager.StartListening("ChangeCameraVector",ChangeUDCutSceneVector);
        EventManager.StartListening("RESET", Reset);
    }

    private void UpDownCutScene()
    {
        playableDirector.Play();
    }

    private void ChangeUDCutSceneVector()
    {
        if(upSceneVector.Length == downSceneVector.Length && upSceneVector.Length > CutSceneManager.Instance.count)
        {
            firsCam.transform.position = downSceneVector[CutSceneManager.Instance.count];
            secoundCam.transform.position = upSceneVector[CutSceneManager.Instance.count];
        }
    }

    private void Reset()
    {
        firsCam.transform.position = downSceneVector[0];
        secoundCam.transform.position = upSceneVector[0];
    }
}
