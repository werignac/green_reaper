using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlantBuffType { GHOSTPEPPER, FRANKENZINI }

public class BuffProgressVisualizer : ValueVisualizerBehaviour<float>
{
    [SerializeField]
    private Scrollbar scrollBar;

    [SerializeField]
    private PlantBuffType buffType;

    public override void Visualize(float toVisualize)
    {
        scrollBar.size = toVisualize;
    }

    public PlantBuffType GetBuffType()
    {
        return buffType;
    }
}
