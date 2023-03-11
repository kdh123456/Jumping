using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;

public class TorchAnimation : MonoBehaviour
{
    private float baseIntensity;
    private float baseTime = .5f;
    private float baseRadius;

    [SerializeField]
    private float intensityRandomness;
    [SerializeField]
    private float timeRandomness;
    [SerializeField]
    private float radiusRandomness;

    private Light2D _light;
    void Start()
    {
        _light = GetComponentInChildren<Light2D>();
        baseIntensity = _light.intensity;
        baseRadius = _light.pointLightOuterRadius;

        ShakeLight();
    }

    public void ShakeLight()
    {
        float targetIntensity = baseIntensity + Random.Range(-baseIntensity, baseIntensity);
        float targetTime = baseTime + Random.Range(-timeRandomness, timeRandomness);
        float targetRadius = baseRadius + Random.Range(-radiusRandomness, radiusRandomness);

        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(
            () => _light.intensity,
            value => _light.intensity = value,
            targetIntensity,
            targetTime
            ));

        seq.Join(DOTween.To(
            () => _light.pointLightOuterRadius,
            value => _light.pointLightOuterRadius = value,
            targetRadius,
            targetTime
            ));

        seq.AppendCallback(() => ShakeLight());
    }
}
