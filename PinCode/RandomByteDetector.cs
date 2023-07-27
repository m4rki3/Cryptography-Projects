using System;
using System.Numerics;
namespace PinCode;
public class RandomByteDetector : IRandomByteDetector
{
    private readonly byte[] randomMemory;
    private readonly byte[] keyRandomMemory;
    private readonly byte[] mainKey;
    private readonly byte mainKeyLength;
    private readonly Random randomizer;
    private int iRC4, jRC4;
    public RandomByteDetector(byte mainKeyLength)
    {
        randomMemory = new byte[256];
        keyRandomMemory = new byte[256];
        mainKey = new byte[mainKeyLength];
        randomizer = new Random();
        this.mainKeyLength = mainKeyLength;
        for (byte i = 0; i < mainKeyLength; i++)
        {
            mainKey[i] = (byte)randomizer.Next(0, 256);
        }
        Initialize();
    }
    private void Initialize()
    {
        for (int i = 0; i < 256; i++)
        {
            randomMemory[i] = (byte)i;
            keyRandomMemory[i] = mainKey[i % mainKeyLength];
        }
        iRC4 = 0;
        jRC4 = 0;
        int randomIndex = 0;
        for (int i = 0; i < 256; i++)
        {
            randomIndex = (randomIndex + randomMemory[i] + keyRandomMemory[i]) % 256;
            var temp = randomMemory[i];
            randomMemory[i] = keyRandomMemory[randomIndex];
            keyRandomMemory[randomIndex] = temp;
        }
    }
    private byte GetRC4RandomByte()
    {
        iRC4 = (iRC4 + 1) % 256;
        jRC4 = (jRC4 + randomMemory[iRC4]) % 256;
        var temp = randomMemory[iRC4];
        randomMemory[iRC4] = randomMemory[jRC4];
        randomMemory[jRC4] = temp;
        int randomIndex = (randomMemory[iRC4] + randomMemory[jRC4]) % 256;
        return randomMemory[randomIndex];
    }
    public byte GetRandomByte(byte maxValue)
    {
        int number = 256 - (256 % maxValue);
        while (true)
        {
            byte randomByte = GetRC4RandomByte();
            if (randomByte < number)
            {
                randomByte = (byte)(randomByte % maxValue);
                return randomByte;
            }
        }
    }
}