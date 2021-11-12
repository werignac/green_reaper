using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffVisualizersManager : MonoBehaviour
{
    private Dictionary<PlantBuffType, BuffProgressVisualizer> visualizers;

    private void Start()
    {
        visualizers = new Dictionary<PlantBuffType, BuffProgressVisualizer>();

        foreach(Transform child in transform)
        {
            BuffProgressVisualizer visualizer = child.GetComponent<BuffProgressVisualizer>();
            visualizers.Add(visualizer.GetBuffType(), visualizer);
            visualizer.gameObject.SetActive(false);
        }
    }

    public void ActivateVisualizer(PlantBuffType type)
    {
        visualizers[type].gameObject.SetActive(true);
        visualizers[type].Visualize(1f);
    }

    public void SetVisualizer(PlantBuffType type, float amount)
    {
        visualizers[type].Visualize(amount);
    }

    public void DeactiveVisualizer(PlantBuffType type)
    {
        visualizers[type].gameObject.SetActive(false);
    }
}
