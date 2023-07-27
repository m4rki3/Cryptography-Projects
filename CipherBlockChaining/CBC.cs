using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CipherBlockChaining;
public static class AesCBC
{
    public static string Encrypt(Aes aes, string message, byte[] iv, byte[] key)
    {
        ICryptoTransform transform = aes.CreateEncryptor();
        byte[] encrypted = transform.TransformFinalBlock(Encoding.ASCII.GetBytes(message), 0, message.Length);
        return Convert.ToBase64String(encrypted);
    }
    public static string Decrypt(Aes aes, string cipherText, byte[] iv, byte[] key)
    {
        ICryptoTransform transform = aes.CreateDecryptor();
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        byte[] decrypted = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        return Encoding.ASCII.GetString(decrypted);
    }
}
