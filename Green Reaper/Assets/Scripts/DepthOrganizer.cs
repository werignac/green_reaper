using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthOrganizer : MonoBehaviour
{
    [SerializeField]
    private bool isDynamic;

    [SerializeField]
    private bool useSpriteRenderer;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private float offset;

    [SerializeField]
    private float finalOffset;

    private void Start()
    {
        if (useSpriteRenderer && spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        SetDepth();
    }

    private void Update()
    {
        if (isDynamic)
            SetDepth();
    }

    private void SetDepth()
    {
        finalOffset = offset;

        if (useSpriteRenderer)
            finalOffset += -ApproxHeightFromRenderer(spriteRenderer, transform);

        transform.position = new Vector3(transform.position.x, transform.position.y, DepthFunction(transform.position.y + offset));
    }

    private static float DepthFunction(float yValue)
    {
        return Mathf.Atan(yValue*0.001f)*5f;
    }


    private static float ApproxHeightFromRenderer(SpriteRenderer renderer, Transform transform)
    {
        return renderer.bounds.size.y * transform.lossyScale.y / 2;
    }
}
