using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashing;
public static class Extensions
{
    public static void CopyTo(this BitArray[] source, BitArray[] destination)
    {
        if (source.Length != destination.Length)
            throw new ArgumentException("Lengths must be equal");
        for (int i = 0; i < source.Length; i++)
        {
            destination[i].Length = source[i].Length;
            for (int j = 0; j < source[i].Length; j++)
                destination[i][j] = source[i][j];
        }
    }
}