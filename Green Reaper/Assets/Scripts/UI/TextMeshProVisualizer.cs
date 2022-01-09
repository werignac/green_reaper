using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class TextMeshProVisualizer<T> : ValueVisualizerBehaviour<T>
{
    private TextMeshPro textComponent;

    private void Start()
    {
        if (textComponent == null)
            textComponent = GetComponent<TextMeshPro>();
    }

    public override void Visualize(T toVisualize)
    {
        if (textComponent == null)
            textComponent = GetComponent<TextMeshPro>();

        textComponent.text = toVisualize.ToString();
    }
}
