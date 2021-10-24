using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeVisualizer : ValueVisualizerBehaviour<int>
{
    [SerializeField]
    private Toggle[] toggles;

    public override void Visualize(int toVisualize)
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].isOn = i < toVisualize;
        }
    }
}
