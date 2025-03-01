using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class FlickerLight : TimedBehaviour
{
    [SerializeField]
    private float baseIntensity = 4.75f;

    [SerializeField]
    private float intensityChange = 0.5f;

    protected override void OnTimerReached()
    {
        float newIntensity = baseIntensity + (Random.value * intensityChange);
        Tween.LightIntensity(GetComponent<Light>(), newIntensity, 0.2f, 0f, Tween.EaseInOut);
    }
}
