using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightVisualizer : ValueVisualizerBehaviour<float>
{
    [SerializeField]
    private List<float> intensitiesOverTime;

    [SerializeField]
    private List<Color> colorsOverTime;

    private Light2D lightToAnimate;

    // Start is called before the first frame update
    void Start()
    {
        lightToAnimate = GetComponent<Light2D>();
    }

    public override void Visualize(float toVisualize)
    {
        float intentistyProgress = GetProgressBetween(toVisualize, intensitiesOverTime, out float leftIntensity, out float rightIntensity);
        lightToAnimate.intensity = Mathf.Lerp(leftIntensity, rightIntensity, intentistyProgress);

        float colorProgress = GetProgressBetween(toVisualize, colorsOverTime, out Color leftColor, out Color rightColor);
        lightToAnimate.color = Color.Lerp(leftColor, rightColor, colorProgress);
    }

    private float GetProgressBetween<T>(float globalProgress, List<T> toInterpolate, out T start, out T end)
    {
        globalProgress = Mathf.Clamp(globalProgress, 0, 1);

        int numberOfElements = toInterpolate.Count - 1;

        if (globalProgress == 1)
        {
            start = toInterpolate[toInterpolate.Count - 2];
            end = toInterpolate[toInterpolate.Count - 1];
            return 1;
        }

        float progressInList = numberOfElements * globalProgress;

        int startIndex = (int) progressInList;
        int endIndex = startIndex + 1;

        start = toInterpolate[startIndex];
        end = toInterpolate[endIndex];

        return progressInList - startIndex;
    }
}
