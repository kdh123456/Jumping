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
    void Start()
    {
        EventManager.StartListening("First",PlayFirst);
    }

    private void PlayFirst()
    {
        playableDirector.Play();
    }
}
