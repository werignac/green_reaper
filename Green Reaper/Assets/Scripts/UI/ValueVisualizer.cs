using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ValueVisualizer<T>
{
    void Visualize(T toVisualize);
}

public abstract class ValueVisualizerBehaviour<T> : MonoBehaviour, ValueVisualizer<T>
{
    public abstract void Visualize(T toVisualize);
}
