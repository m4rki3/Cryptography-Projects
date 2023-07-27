using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Hashing;
public class SHA1
{
    private BitArray? sourceMessageBits;
    private int sourceMessageLength;
    private bool[]? fullMessageBits;
    private int fullMessageLength;
    private int zerosCount;
    private int blocksCount;
    private List<List<BitArray>> bitBlocks = new();
    private List<BitArray> wValues = new();
    private BitArray? m;
    private BitArray[] hashValues;
    private BitArray[] aValues;
    private BitArray[] constants;
    public SHA1()
    {
        hashValues = new BitArray[5]
        {
             new(Encoding.ASCII.GetBytes(0x67452301.ToString())),
             new(Encoding.ASCII.GetBytes(0xEFCDAB89.ToString())),
             new(Encoding.ASCII.GetBytes(0x98BADCFE.ToString())),
             new(Encoding.ASCII.GetBytes(0x10325476.ToString())),
             new(Encoding.ASCII.GetBytes(0xC3D2E1F0.ToString()))
        };
        constants = new BitArray[4]
        {
             new(Encoding.ASCII.GetBytes(0x5A827999.ToString())),
             new(Encoding.ASCII.GetBytes(0x6ED9EBA1.ToString())),
             new(Encoding.ASCII.GetBytes(0x8F1BBCDC.ToString())),
             new(Encoding.ASCII.GetBytes(0xCA62C1D6.ToString()))
        };
        aValues = new BitArray[5]
        {
            new(0),
            new(0),
            new(0),
            new(0),
            new(0)
        };
    }
    public string GetHashCode(string message)
    {
        sourceMessageBits = new(Encoding.ASCII.GetBytes(message));
        sourceMessageLength = sourceMessageBits.Count;
        m = new(
            Encoding.ASCII.GetBytes(sourceMessageLength.ToString())
        );
        m.Length = 64;
        int rating = sourceMessageLength % 512;
        if (rating <= 447)
        {
            zerosCount = 447 - rating;
        }
        else
        {
            zerosCount = 959 - rating;
        }
        CompleteTheFullMessage();
        for (int i = 0; i < blocksCount; i++)
        {
            hashValues.CopyTo(aValues);
            for (int j = 0; j < 16; j++)
            {
                wValues.Add(bitBlocks[i][j]);
            }
            for (int j = 16; j < 80; j++)
            {
                wValues.Add(
                    wValues[j - 3].Xor(wValues[j - 8])
                                  .Xor(wValues[j - 14])
                                  .Xor(wValues[j - 16])
                                  .LeftShift(count: 1)
                );
            }
            for (int j = 0; j < 80; j++)
            {
                EquateLengths(aValues[0], aValues[1], aValues[2], aValues[3], aValues[4], wValues[j]);
                aValues[4] = aValues[0].LeftShift(count: 5)
                                       .Or(Function(aValues[1], aValues[2], aValues[3], j))
                                       .Or(aValues[4])
                                       .Or(wValues[j])
                                       .Or(GetConstant(j));
                SwapAValues();
            }
            for (int j = 0; j < hashValues.Length; j++)
            {
                EquateLengths(hashValues[j], aValues[j]);
                hashValues[j] = hashValues[j].Or(aValues[j]);
            }
        }
        List<byte[]> hashBytes = new();
        StringBuilder hashSB = new();
        for (int i = 0; i < hashValues.Length; i++)
        {
            hashBytes.Add(
                new byte[(int)Math.Round((double)(hashValues[i].Length / 8), MidpointRounding.ToPositiveInfinity)]
            );
            hashValues[i].CopyTo(hashBytes[i], index: 0);
            hashSB.Append(
                BitConverter.ToString(hashBytes[i])
                            .Replace("-", string.Empty)
            );
        }
        return hashSB.ToString();
    }
    private void EquateLengths(params BitArray[] arrays)
    {
        int maxLength = 0;
        foreach (var item in arrays)
        {
            maxLength = Math.Max(maxLength, item.Length);
        }
        foreach (var item in arrays)
        {
            item.Length = maxLength;
        }
    }
    private void SwapAValues()
    {
        BitArray[] aValuesCopy = new BitArray[aValues.Length];
        Array.Copy(aValues, aValuesCopy, aValues.Length);
        aValues[0] = aValuesCopy[4];
        aValues[1] = aValuesCopy[4];
        aValues[2] = aValuesCopy[1].LeftShift(count: 30);
        aValues[3] = aValuesCopy[2];
        aValues[4] = aValuesCopy[3];
    }
    private BitArray GetConstant(int index) => index switch
    {
        >= 0 and < 20 => constants[0],
        >= 20 and < 40 => constants[1],
        >= 40 and < 60 => constants[2],
        >= 60 and < 80 => constants[3],
        _ => throw new ArgumentException("Index must be at 0..79 range")
    };
    private BitArray Function(BitArray x, BitArray y, BitArray z, int index) => index switch
    {
        >= 0 and < 20 => x.And(y).Or(z.Not().And(z)),
        >= 20 and < 40 or >= 60 and < 80 => x.Xor(y).Xor(z),
        >= 40 and < 60 => x.And(y).Or(x.And(z)).Or(y.And(z)),
        _ => throw new ArgumentException("Index must be at 0..79 range")
    };
    private void CompleteTheFullMessage()
    {
        fullMessageLength = sourceMessageLength + 1 + zerosCount + m.Length;
        fullMessageBits = new bool[fullMessageLength];
        sourceMessageBits?.CopyTo(fullMessageBits, 0);
        int index = sourceMessageLength;
        fullMessageBits[index] = true;
        for (index = sourceMessageLength + 1; index < sourceMessageLength + 1 + zerosCount; index++)
        {
            fullMessageBits[index] = false;
        }
        for (int j = 0; j < m.Length; index++, j++)
        {
            fullMessageBits[index] = m[j];
        }
        blocksCount = (sourceMessageLength + zerosCount + 65) / 512;
        for (int i = 0, bitsCounter = 0; i < blocksCount; i++)
        {
            bitBlocks.Add(new List<BitArray>());
            for (int j = 0; j < 16; j++)
            {
                bitBlocks[i].Add(new(32));
                for (int k = 0; k < 32; k++, bitsCounter++)
                {
                    bitBlocks[i][j][k] = fullMessageBits[bitsCounter];
                }
            }
        }
    }
}