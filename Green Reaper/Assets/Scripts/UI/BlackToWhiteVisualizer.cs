using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BlackToWhiteVisualizer : ValueVisualizerBehaviour<float>
{
    private SpriteRenderer spriteRenderer;

    public override void Visualize(float toVisualize)
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(toVisualize,toVisualize,toVisualize, spriteRenderer.color.a);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    
}
