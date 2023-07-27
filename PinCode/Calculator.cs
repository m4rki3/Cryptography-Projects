using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace PinCode;
public class Calculator : ICalculator
{
    private IRandomByteDetector randomByteDetector;
    private ISubstitutionFactory substitutionFactory;
    private double[,] diagonalMatrix;
    private double[,] lowerTriangularMatrix, inverseLowerTriangularMatrix;
    private double[,] upperTriangularMatrix, inverseUpperTriangularMatrix;
    private Queue<double> diagonalElements;
    private List<byte[]> substitutions;
    public Calculator(IRandomByteDetector detector, ISubstitutionFactory factory)
    {
        randomByteDetector = detector;
        substitutionFactory = factory;
        diagonalElements = new();
        substitutions = new();
        diagonalMatrix = new double[8, 8];
        lowerTriangularMatrix = new double[8, 8];
        inverseLowerTriangularMatrix = new double[8, 8];
        upperTriangularMatrix = new double[8, 8];
        inverseUpperTriangularMatrix = new double[8, 8];
        InitializeSubstitutions();
        InitializeLowerTriangularMatrix(ref lowerTriangularMatrix);
        InitializeUpperTriangularMatrix(ref upperTriangularMatrix);
        InitializeInverseOfMatrix(lowerTriangularMatrix, ref inverseLowerTriangularMatrix);
        InitializeInverseOfMatrix(upperTriangularMatrix, ref inverseUpperTriangularMatrix);
    }
    public double[,] CreateNewInvolutiveMatrix(byte setSize)
    {
        EnqueueDiagonalElements(setSize);
        InitializeDiagonalMatrix(ref diagonalMatrix);
        return lowerTriangularMatrix.MultiplyOn(upperTriangularMatrix)
                                    .MultiplyOn(diagonalMatrix)
                                    .MultiplyOn(inverseUpperTriangularMatrix)
                                    .MultiplyOn(inverseLowerTriangularMatrix);
    }
    public void FeistelTranformation(byte firstElement, byte secondElement, params byte[] keys)
    {
        if (keys.Length < 4)
            throw new ArgumentException("Keys array must include at least 4 elements.");
        for (int i = 0; i < 4; i++)
        {
            firstElement = (byte)((firstElement + substitutions[i][(secondElement + keys[i]) % 100]) % 100);
            if (i != 3)
            {
                var temp = firstElement;
                firstElement = secondElement;
                secondElement = temp;
            }
        }
    }
    public void InverseOfFeistelTransformation(byte firstElement, byte secondElement, params byte[] keys)
    {
        if (keys.Length < 4)
            throw new ArgumentException("Keys array must include at least 4 elements.");
        for (int i = 3; i >= 0; i--)
        {
            firstElement = (byte)((firstElement + (100 - substitutions[i][(secondElement + keys[i]) % 100])) % 100);
            if (i != 0)
            {
                var temp = firstElement;
                firstElement = secondElement;
                secondElement = temp;
            }
        }
    }
    public void ShiftLeftBytes(byte[] bytes)
    {
        var temp = bytes[0];
        for (int i = 0; i < bytes.Length - 1; i++)
        {
            bytes[i] = bytes[i + 1];
        }
        bytes[bytes.Length - 1] = temp;
    }
    public void ShiftRightBytes(byte[] bytes)
    {
        var temp = bytes[bytes.Length - 1];
        for (int i = bytes.Length - 1; i > 0; i--)
        {
            bytes[i] = bytes[i - 1];
        }
        bytes[0] = temp;
    }
    private void EnqueueDiagonalElements(byte setSize)
    {
        for (int i = 0; i < 4; i++)
        {
            byte a = randomByteDetector.GetRandomByte(setSize);
            byte b = randomByteDetector.GetRandomByte(setSize);
            while (GetGCD(b, setSize) != 1)
            {
                b = randomByteDetector.GetRandomByte(setSize);
            }
            diagonalElements.Enqueue(a);
            diagonalElements.Enqueue(b);
            diagonalElements.Enqueue((1 - a * a) / b);
            diagonalElements.Enqueue(-a);
        }
    }
    private void InitializeSubstitutions()
    {
        for (int i = 0; i < 3; i++)
            substitutions.Add(substitutionFactory.MakeNewSubstitutionOnSet(100, 100));
        substitutions.Add(substitutionFactory.MakeNewSubstitutionBasedOnDiscreteLogarithm());
    }
    private int GetGCD(int first, int second)
    {
        if (first == 0)
            return second;
        else
        {
            int min = Math.Min(first, second);
            int max = Math.Max(first, second);
            return GetGCD(max - min, min);
        }
    }
    private void InitializeDiagonalMatrix(ref double[,] matrix)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (i == j || (j % 2 == 1 && j == i + 1) || (j % 2 == 0 && i == j + 1))
                {
                    matrix[i, j] = diagonalElements.Dequeue();
                }
                else matrix[i, j] = 0;
            }
        }
    }
    private void InitializeLowerTriangularMatrix(ref double[,] matrix)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (j > i)
                    matrix[i, j] = 0;
                else if (i == j)
                {
                    var temp = randomByteDetector.GetRandomByte(100);
                    while (GetGCD(temp, 100) != 1)
                        temp = randomByteDetector.GetRandomByte(100);
                    matrix[i, j] = temp;
                }
                else matrix[i, j] = randomByteDetector.GetRandomByte(100);
            }
        }
    }
    private void InitializeUpperTriangularMatrix(ref double[,] matrix)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (j < i)
                    matrix[i, j] = 0;
                else if (i == j)
                {
                    var temp = randomByteDetector.GetRandomByte(100);
                    while (GetGCD(temp, 100) != 1)
                        temp = randomByteDetector.GetRandomByte(100);
                    matrix[i, j] = temp;
                }
                else matrix[i, j] = randomByteDetector.GetRandomByte(100);
            }
        }
    }
    private void InitializeInverseOfMatrix(double[,] source, ref double[,] matrix)
    {
        matrix = Accord.Math.Matrix.Inverse(source);
    }
}