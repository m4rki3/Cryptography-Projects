using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PinCode;
public class SubstitutionFactory : ISubstitutionFactory
{
    private IRandomByteDetector randomByteDetector;
    public SubstitutionFactory(IRandomByteDetector randomByteDetector)
    {
        this.randomByteDetector = randomByteDetector;
    }
    public byte[] MakeNewSubstitutionOnSet(byte setSize, byte maxValue)
    {
        byte[] bytes = new byte[setSize];
        for (byte i = 0; i < setSize; i++)
            bytes[i] = i;
        byte kRandomIndex, mRandomIndex;
        while (true)
        {
            for (int i = 0; i < setSize; i++)
            {
                kRandomIndex = randomByteDetector.GetRandomByte(setSize);
                mRandomIndex = randomByteDetector.GetRandomByte(setSize);
                var temp = bytes[kRandomIndex];
                bytes[kRandomIndex] = bytes[mRandomIndex];
                bytes[mRandomIndex] = temp;
            }
            if (setSize <= maxValue)
                return bytes.Where(item => item <= maxValue)
                            .Select(item => item)
                            .ToArray();
            return bytes;
        }
    }
    public byte[] MakeNewSubstitutionBasedOnDiscreteLogarithm()
    {
        byte[] xSet = MakeNewSubstitutionOnSet(100, 100);
        byte kRandomItem = randomByteDetector.GetRandomByte(100);
        byte mRandomItem = randomByteDetector.GetRandomByte(100);
        byte[] ySet = new byte[100];
        for (int i = 0; i < 100; i++)
            ySet[i] = (byte)(kRandomItem * xSet[i] + mRandomItem);
        return ySet;
    }
    private bool IsBytesFullySwapped(byte[] values)
    {
        for (byte i = 0; i < 256; i++)
            if (values[i] == i) return false; 
        return true;
    }
}