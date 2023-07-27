using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinCode;
public static class Extensions
{
    public static double[,] MultiplyOn(this double[,] first, double[,] second)
    {
        return Accord.Math.Matrix.Dot(first, second);
    }
    public static byte[] ToByte(this double[] vector, int size, int maxValue)
    {
        byte[] bytes = new byte[size];
        for (int i = 0; i < size; i++)
        {
            if (vector[i] < 0) bytes[i] = (byte)(vector[i] * -1 % maxValue);
            else bytes[i] = (byte)(vector[i] % maxValue);
        }
        return bytes;
    }
}
