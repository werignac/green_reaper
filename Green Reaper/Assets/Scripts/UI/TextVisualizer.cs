using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextVisualizer<T> : ValueVisualizerBehaviour<T>
{
    private Text textComponent;

    private void Start()
    {
        if (textComponent == null)
            textComponent = GetComponent<Text>();
    }

    public override void Visualize(T toVisualize)
    {
        if (textComponent == null)
            textComponent = GetComponent<Text>();

        textComponent.text = toVisualize.ToString();
    }
}
