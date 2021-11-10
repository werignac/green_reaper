using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics;

[RequireComponent(typeof(LineRenderer))]
public class PolynomialVisualizer : MonoBehaviour
{
    [SerializeField]
    private Transform[] points;

    [SerializeField]
    private Transform startSlope;

    [SerializeField]
    private Transform endSlope;

    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        Polynomial polynomial = PolynomialGenerator.GeneratePolynomial(3, GetConstraints(), GetDerivateConstrains());

        float start = points[0].position.x;
        float end = points[points.Length - 1].position.x;

        List<Vector3> linePoints = new List<Vector3>();

        for (float i = start; (start < end && i < end) || (start > end && i > end); i += (end - start)/100)
        {
            linePoints.Add(new Vector3(i, (float) polynomial.Evaluate(i), 0));
        }

        lineRenderer.positionCount = linePoints.Count;
        lineRenderer.SetPositions(linePoints.ToArray());
    }

    private List<Vector2> GetConstraints()
    { 
        List<Vector2> constraints = new List<Vector2>();
        foreach (Transform point in points)
            constraints.Add(point.position);
        return constraints;
    }

    private List<Vector2> GetDerivateConstrains()
    {
        List<Vector2> constraints = new List<Vector2>();
        constraints.Add(new Vector2(points[0].position.x, CalculateSlope(points[0].position, startSlope.transform.position)));
        constraints.Add(new Vector2(points[points.Length - 1].position.x, CalculateSlope(points[points.Length - 1].position, endSlope.transform.position)));
        return constraints;
    }


    private static float CalculateSlope(Vector2 v1, Vector2 v2)
    {
        return (v2.y - v1.y) / (v2.x - v1.x);
    }
}
