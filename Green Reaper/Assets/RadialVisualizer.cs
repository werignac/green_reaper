using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialVisualizer : ValueVisualizerBehaviour<float>
{
    [SerializeField]
    private Transform circle;

    [SerializeField]
    private float startAngle;

    [SerializeField]
    private float endAngle;

    private void Start()
    {
        SetToAngle(startAngle);
    }

    public override void Visualize(float toVisualize)
    {
        SetToAngle(Mathf.Lerp(startAngle, endAngle, toVisualize));
    }

    private void SetToAngle(float angle)
    {
        circle.rotation = Quaternion.Euler(0, 0, angle);
    }
}
