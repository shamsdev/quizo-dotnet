using System.Security.Cryptography;
using System.Text;

namespace QuizoDotnet.Application.Utils;

public static class CryptUtils
{
    public static string Encrypt(string input, string key,
        CipherMode cipherMode = CipherMode.ECB,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        try
        {
            using var aesAlg = Aes.Create();

            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.Mode = cipherMode;
            aesAlg.Padding = paddingMode;

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(input);
                }

                byte[] encrypted = msEncrypt.ToArray();

                return Convert.ToBase64String(encrypted);
            }
        }
        catch (Exception e)
        {
            throw new ArgumentException(e.Message);
        }
    }

    public static string Decrypt(string cipherText, string key,
        CipherMode cipherMode = CipherMode.ECB,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        try
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.Mode = cipherMode;
                aesAlg.Padding = paddingMode;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                var cipherBytes = Convert.FromBase64String(cipherText);

                using (var msDecrypt = new MemoryStream(cipherBytes))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
        catch (Exception e)
        {
            throw new ArgumentException(e.Message);
        }
    }
}