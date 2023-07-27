using Accord;
using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace PinCode;
public static class CodeGenerator
{
    private static IRandomByteDetector randomByteDetector;
    private static ICalculator calculator;
    public static string GenerateCode(string code, IRandomByteDetector detector, ICalculator computingCalculator)
    {
        randomByteDetector = detector;
        calculator = computingCalculator;
        byte[] vector = ParseToTwoDigitBytes(code);
        byte[] keys = GenerateKeys(count: 8);
        byte[] feiselKeys = GenerateKeys(count: 16);
        MixingBytesBeginning(vector, keys);
        for (int i = 0; i < 3; i++)
        {
            calculator.FeistelTranformation(
                vector[0], vector[1],
                vector[7], feiselKeys[2 * i], vector[2], (byte)(feiselKeys[2 * i + 1] * feiselKeys[2 * i + 1])
            );
            calculator.ShiftRightBytes(vector);
            calculator.ShiftRightBytes(vector);
        }
        calculator.FeistelTranformation(vector[0], vector[1],
            vector[7], feiselKeys[6], vector[2], feiselKeys[7]);
        var invM = calculator.CreateNewInvolutiveMatrix(100);
        CheckIfInvolutive(invM);
        vector = vector.Dot(calculator.CreateNewInvolutiveMatrix(setSize: 100))
                       .ToByte(size: 8, maxValue: 100);
        for (int i = 4; i < 7; i++)
        {
            calculator.InverseOfFeistelTransformation(vector[0], vector[1],
                vector[7], feiselKeys[2 * i], vector[2], feiselKeys[2 * i + 1]);
            calculator.ShiftLeftBytes(vector);
            calculator.ShiftLeftBytes(vector);
        }
        MixingBytesEnding(vector, keys);
        return GenerateCodeFromBytes(vector);
    }
    private static bool CheckIfInvolutive(double[,] matrix)
    {
        if (matrix.MultiplyOn(matrix) == Accord.Math.Matrix.Identity(8))
            return true;
        return false;
    }
    private static string GenerateCodeFromBytes(byte[] bytes)
    {
        string code = string.Empty;
        for (int i = 0; i < 8; i++)
        {
            code += bytes[i] % 10;
            code += bytes[i] / 10;
        }
        return code;
    }
    private static byte[] ParseToTwoDigitBytes(string code)
    {
        byte[] bytes = new byte[8];
        for (int i = 0; i < 8; i++)
        {
            if (!byte.TryParse(code[2 * i].ToString() + code[2 * i + 1].ToString(), out bytes[i]))
                throw new ArgumentException("Can't parse the input code. Code should consist of 16 digits.");
        }
        return bytes;
    }
    private static byte[] GenerateKeys(int count)
    {
        byte[] keys = new byte[count];
        for (int i = 0; i < count; i++)
        {
            keys[i] = randomByteDetector.GetRandomByte(100);
        }
        return keys;
    }
    private static void MixingBytesEnding(byte[] bytes, byte[] keys)
    {
        for (int i = 0; i < bytes.Length; i++)
            bytes[i] = (byte)(Math.Abs(bytes[i] - keys[i]) % 100);
    }
    private static void MixingBytesBeginning(byte[] bytes, byte[] keys)
    {
        for (int i = 0; i < bytes.Length; i++)
            bytes[i] = (byte)((bytes[i] + keys[i]) % 100);
    }
}