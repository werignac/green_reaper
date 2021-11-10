using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;

public class PolynomialGenerator
{
    public static Polynomial GeneratePolynomial(uint polynomialPower, List<Vector2> normalConstrains, List<Vector2> derivativeConstraints)
    {
        Matrix<float> equations = CreateSystemOfEquations(polynomialPower, normalConstrains,derivativeConstraints, out Vector<float> solutions);
        Vector<float> constants = equations.Solve(solutions);
        double[] doubledConstants = new double[constants.Count];

        for (int i = 0; i < constants.Count; i++)
            doubledConstants[constants.Count - (i + 1)] = constants[i];

        return new Polynomial(doubledConstants);
    }

    private static Matrix<float> CreateSystemOfEquations(uint polynomialPower, List<Vector2> normalConstrains, List<Vector2> derivativeConstraints, out Vector<float> solutions)
    {
        float[,] equations = new float[polynomialPower + 1, normalConstrains.Count + derivativeConstraints.Count];

        uint equationIndex = 0;

        List<float> solutionsList = new List<float>();

        foreach (Vector2 constraint in normalConstrains)
        {
            float[] xValues = CalculateXValues(constraint.x, polynomialPower);
            FillEquation(equations, equationIndex, xValues);
            solutionsList.Add(constraint.y);
            equationIndex++;
        }

        foreach(Vector2 constraint in derivativeConstraints)
        {
            float[] xValues = CalculateXValuesDerivative(constraint.x,polynomialPower);
            FillEquation(equations, equationIndex, xValues);
            solutionsList.Add(constraint.y);
            equationIndex++;
        }

        solutions = Vector<float>.Build.DenseOfEnumerable(solutionsList);

        return Matrix<float>.Build.DenseOfArray(equations);
    }
    

    private static float[] CalculateXValues(float xValue, uint polynomialPower)
    {
        float[] values = new float[polynomialPower + 1];

        for (int i = (int)polynomialPower; i >= 0; i--)
            values[polynomialPower - i] = Mathf.Pow(xValue, i);

        return values;
    }

    private static float[] CalculateXValuesDerivative(float xValue, uint polynomialPower)
    {
        float[] values = new float[polynomialPower + 1];

        for (int i = (int) polynomialPower; i >= 0; i--)
            values[polynomialPower - i] = i * Mathf.Pow(xValue, i - 1);

        return values;
    }

    private static void FillEquation(float[,] equations, uint equationIndex, float[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            float value = values[i];
            equations[equationIndex, i] = value;
        }
    }
}
