using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalSignature;
public static class Calculator
{
    public static bool TryGetRandomPrimeDivisorTo(int dividend, out int result)
    {
        Random randomizer = new();
        result = randomizer.Next(100, 1_000);
        System.Timers.Timer timer = new(2000);
        timer.Elapsed += Timer_Elapsed;
        while (!IsPrime(result) || dividend % result != 0)
        {
            result = randomizer.Next(100, 1_000);
            if (timer.Enabled == false)
            {
                timer.Dispose();
                return false;
            }
        }
        return true;
    }
    private static void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (sender is System.Timers.Timer timer)
        {
            timer.Enabled = false;
        }
    }
    public static int GetRandomPrime()
    {
        Random randomizer = new();
        int result = randomizer.Next(500, 1_000);
        while (!IsPrime(result))
        {
            result = randomizer.Next(500, 1_000);
        }
        return result;
    }
    private static bool IsPrime(int number)
    {
        if (number == 1)
            return true;
        for (int i = 2; i < number; i++)
        {
            if (number % i == 0)
                return false;
        }
        return true;
    }
}
