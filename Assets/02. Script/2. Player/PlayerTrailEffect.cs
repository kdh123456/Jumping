using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrailEffect : MonoBehaviour
{
    private TrailRenderer trailRenderer;

    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        InActiveTrail();
    }

    public void ActiveTrail()
    {
        trailRenderer.time = 0.3f;
    }
    public void InActiveTrail()
    {
        trailRenderer.time = 0f;
    }
}
