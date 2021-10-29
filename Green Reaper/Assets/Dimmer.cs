using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Dimmer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private bool isDimmingOrDim = false;

    private float progress;

    private Coroutine process;

    [SerializeField]
    private float dimPeriod = 0.25f;

    [SerializeField]
    private float maxDim = 0.75f;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        progress = dimPeriod;
    }

    public void SetDim(bool isDim)
    {
        if (isDimmingOrDim != isDim)
        {
            if (process != null)
            {
                StopCoroutine(process);
                process = null;
            }

            progress = dimPeriod - progress;
            isDimmingOrDim = isDim;

            process = StartCoroutine(Dim());
        }
    }

    private IEnumerator Dim()
    {
        while (progress < dimPeriod)
        {
            progress += Time.deltaTime;

            float alpha = isDimmingOrDim ? Mathf.Lerp(1, maxDim, progress / dimPeriod) : Mathf.Lerp(maxDim, 1, progress / dimPeriod);

            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.b, spriteRenderer.color.g, alpha);
            yield return new WaitForEndOfFrame();
        }

        progress = dimPeriod;
        process = null;
    }
}
