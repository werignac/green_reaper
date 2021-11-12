using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;

public class Path
{
    private float angle;
    private Polynomial polynomial;
    private float start;
    private float end;
    public float radius;

    public Path(Vector2 startPoint, Vector2 startDirection, Vector2 endPoint, Vector2 endDirection, float _radius)
    {
        radius = _radius;

        Vector2 difference = endPoint - startPoint;
        angle = -Mathf.Atan2(difference.y, difference.x);

        Matrix<float> rotationMatrix = Matrix<float>.Build.DenseOfArray(new float[,]
        { {Mathf.Cos(angle) , -Mathf.Sin(angle)},
        { Mathf.Sin(angle) , Mathf.Cos(angle)} });

        polynomial = PolynomialGenerator.GeneratePolynomial(3, GetConstraints(new Vector2[] {startPoint, endPoint}, rotationMatrix), 
            GetDerivateConstrains(startPoint, startDirection, endPoint, endDirection, rotationMatrix));

        start = RotateVector2(startPoint, angle).x;
        end = RotateVector2(endPoint, angle).x;
    }

    public IEnumerator<Vector2> Iterate(float resolution)
    {
        float intervals = (end - start) * (1 - resolution);

        for (float i = start; (start <= end && i < end) || (start >= end && i > end); i += intervals)
        {
            yield return RotateVector2(new Vector2(i, (float)polynomial.Evaluate(i)), -angle);
        }
    }

    public float Length()
    {
        return (float)(polynomial.Integrate().Evaluate(end) - polynomial.Integrate().Evaluate(start));
    }

    private static List<Vector2> GetConstraints(Vector2[] points, Matrix<float> rotationMatrix)
    {
        List<Vector2> constraints = new List<Vector2>();
        foreach (Vector2 point in points)
            constraints.Add(RotateVector2(point, rotationMatrix));
        return constraints;
    }

    private static List<Vector2> GetDerivateConstrains(Vector2 startPoint, Vector2 startDir, Vector2 endPoint, Vector2 endDir, Matrix<float> rotationMatrix)
    {
        List<Vector2> constraints = new List<Vector2>();

        Vector2 start = RotateVector2(startPoint, rotationMatrix);
        Vector2 startSlopeRot = RotateVector2(startDir, rotationMatrix);
        Vector2 end = RotateVector2(endPoint, rotationMatrix);
        Vector2 endSlopeRot = RotateVector2(endDir, rotationMatrix);

        constraints.Add(new Vector2(start.x, CalculateSlope(start, startSlopeRot)));
        constraints.Add(new Vector2(end.x, CalculateSlope(end, endSlopeRot)));
        return constraints;
    }

    public static Vector2 RotateVector2(Vector2 toRotate, float angle)
    {
        Matrix<float> rotationMatrix = Matrix<float>.Build.DenseOfArray(new float[,]
        { {Mathf.Cos(angle) , -Mathf.Sin(angle)},
        { Mathf.Sin(angle) , Mathf.Cos(angle)} });
        return RotateVector2(toRotate, rotationMatrix);
    }


    public static Vector2 RotateVector2(Vector2 toRotate, Matrix<float> rotationMatrix)
    {
        Vector<float> conversion = Vector<float>.Build.DenseOfArray(new float[] { toRotate.x, toRotate.y });
        Vector<float> rotated = rotationMatrix.Multiply(conversion);
        return new Vector2(rotated[0], rotated[1]);
    }

    private static float CalculateSlope(Vector2 v1, Vector2 v2)
    {
        return (v2.y - v1.y) / (v2.x - v1.x);
    }

    public bool InRange(Vector2 point, int numberOfRecurses)
    {
        Vector2 rotatedPoint = RotateVector2(point, angle);

        float min = rotatedPoint.x - radius;
        float max = rotatedPoint.x + radius;

        max = Mathf.Min(max, end);
        min = Mathf.Max(min, start);

        float distanceAtMin = Vector2.Distance(rotatedPoint, new Vector2(min, (float)polynomial.Evaluate(min)));
        float distanceAtMax = Vector2.Distance(rotatedPoint, new Vector2(max, (float)polynomial.Evaluate(max)));

        float minDistance = ApproximateMinDistanceRec(rotatedPoint, numberOfRecurses, min, max);
        minDistance = Mathf.Min(minDistance, distanceAtMin);
        minDistance = Mathf.Min(minDistance, distanceAtMax);

        return minDistance < radius;
    }

    private float ApproximateMinDistanceRec(Vector2 rotatedPoint, int numberOfRecurses, float min, float max)
    {
        float xValue = (max + min) / 2;

        float distance = Vector2.Distance(rotatedPoint, new Vector2(xValue, (float) polynomial.Evaluate(xValue)));

        if (numberOfRecurses > 1)
        {
            distance = Mathf.Min(distance, ApproximateMinDistanceRec(rotatedPoint, numberOfRecurses - 1, min, xValue));
            distance = Mathf.Min(distance, ApproximateMinDistanceRec(rotatedPoint, numberOfRecurses - 1, xValue, max));
        }

        return distance;
    }
}
