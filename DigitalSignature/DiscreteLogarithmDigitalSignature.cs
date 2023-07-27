using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalSignature;
public class DiscreteLogarithmDigitalSignature
{
    public static string GetDigitalSignature(string message)
    {
        Random randomizer = new Random();
        int p, q;
        do
        {
            p = Calculator.GetRandomPrime();
        }
        while (!Calculator.TryGetRandomPrimeDivisorTo(p - 1, out q));
        int gamma = randomizer.Next(2, p - 2);
        var g = Math.Pow(gamma, (p - 1) / q) % p;
        int secretKey = randomizer.Next(3, q - 2);
        var openKey = Math.Pow(g, secretKey) % p;
        int k = randomizer.Next(1, q - 1);
        var hash = System.Security.Cryptography.SHA1.Create()
                                                    .ComputeHash(
                                                        Encoding.ASCII.GetBytes(message)
                                                    );
        var r = Math.Pow(g, k) % p;
        int h = BitConverter.ToInt32(hash);
        var roh = r % q;
        var s = (1 + roh * h * secretKey) / h / secretKey % q;
        if (Math.Pow(r, s * h) == g * Math.Pow(openKey, roh * h))
        {
            StringBuilder builder = new();
            builder.Append(r.ToString())
                   .Append(s.ToString());
            byte[] bytes = Encoding.ASCII.GetBytes(builder.ToString());
            return BitConverter.ToString(bytes)
                               .Replace("-", string.Empty);
        }
        else return GetDigitalSignature(message);
    }
}