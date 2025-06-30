using System.Security.Cryptography;
using System.Text;

public static class RSAKeyGenerator
{
    public static (string privateKey, string publicKey) GenerateKeyPair()
    {
        using var rsa = RSA.Create(2048); // 2048-bit key

        var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
        var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());

        // Format as PEM for easier usage
        var privateKeyPem = $"-----BEGIN PRIVATE KEY-----\n{FormatKeyString(privateKey)}\n-----END PRIVATE KEY-----";
        var publicKeyPem = $"-----BEGIN PUBLIC KEY-----\n{FormatKeyString(publicKey)}\n-----END PUBLIC KEY-----";

        return (privateKeyPem, publicKeyPem);
    }

    private static string FormatKeyString(string key)
    {
        const int lineLength = 64;
        var formattedKey = new StringBuilder();

        for (int i = 0; i < key.Length; i += lineLength)
        {
            formattedKey.AppendLine(key.Substring(i, Math.Min(lineLength, key.Length - i)));
        }

        return formattedKey.ToString().TrimEnd();
    }
}