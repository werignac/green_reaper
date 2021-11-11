using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

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
        Vector2 difference = points[points.Length - 1].position - points[0].position;
        float angle = -Mathf.Atan2(difference.y, difference.x);

        Matrix<float> rotationMatrix = Matrix<float>.Build.DenseOfArray(new float[,]
        { {Mathf.Cos(angle) , -Mathf.Sin(angle)},
        { Mathf.Sin(angle) , Mathf.Cos(angle)} });

        Polynomial polynomial = PolynomialGenerator.GeneratePolynomial(3, GetConstraints(rotationMatrix), GetDerivateConstrains(rotationMatrix));

        float start = RotateVector2(points[0].position, angle).x;
        float end = RotateVector2(points[points.Length - 1].position, angle).x;

        List<Vector3> linePoints = new List<Vector3>();

        for (float i = start; (start < end && i < end) || (start > end && i > end); i += (end - start)/100)
        {
            Vector2 rotatedBack = RotateVector2(new Vector2(i, (float)polynomial.Evaluate(i)), -angle);
            linePoints.Add(new Vector3(rotatedBack.x, rotatedBack.y, 0));
        }

        lineRenderer.positionCount = linePoints.Count;
        lineRenderer.SetPositions(linePoints.ToArray());
    }

    private List<Vector2> GetConstraints(Matrix<float> rotationMatrix)
    { 
        List<Vector2> constraints = new List<Vector2>();
        foreach (Transform point in points)
            constraints.Add(RotateVector2(point.position, rotationMatrix));
        return constraints;
    }

    private List<Vector2> GetDerivateConstrains(Matrix<float> rotationMatrix)
    {
        List<Vector2> constraints = new List<Vector2>();

        Vector2 start = RotateVector2(points[0].position, rotationMatrix);
        Vector2 startSlopeRot = RotateVector2(startSlope.position, rotationMatrix);
        Vector2 end = RotateVector2(points[points.Length - 1].position, rotationMatrix);
        Vector2 endSlopeRot = RotateVector2(endSlope.position, rotationMatrix);

        constraints.Add(new Vector2(start.x, CalculateSlope(start, startSlopeRot)));
        constraints.Add(new Vector2(end.x, CalculateSlope(end, endSlopeRot)));
        return constraints;
    }

    private static Vector2 RotateVector2(Vector2 toRotate, float angle)
    {
        Matrix<float> rotationMatrix = Matrix<float>.Build.DenseOfArray(new float[,]
        { {Mathf.Cos(angle) , -Mathf.Sin(angle)},
        { Mathf.Sin(angle) , Mathf.Cos(angle)} });
        return RotateVector2(toRotate, rotationMatrix);
    }


    private static Vector2 RotateVector2(Vector2 toRotate, Matrix<float> rotationMatrix)
    {
        Vector<float> conversion = Vector<float>.Build.DenseOfArray(new float[] { toRotate.x, toRotate.y });
        Vector<float> rotated = rotationMatrix.Multiply(conversion);
        return new Vector2(rotated[0], rotated[1]);
    }

    private static float CalculateSlope(Vector2 v1, Vector2 v2)
    {
        return (v2.y - v1.y) / (v2.x - v1.x);
    }
}
