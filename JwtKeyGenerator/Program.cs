public class Program
{
    public static void Main()
    {
        Console.WriteLine("Generating RSA Key Pair for JWT...\n");

        var (privateKey, publicKey) = RSAKeyGenerator.GenerateKeyPair();

        Console.WriteLine("PRIVATE KEY (Keep this secret - only on auth service):");
        Console.WriteLine(privateKey);
        Console.WriteLine("\n" + "=".PadLeft(80, '=') + "\n");

        Console.WriteLine("PUBLIC KEY (Can be shared with all services):");
        Console.WriteLine(publicKey);
        Console.WriteLine("\n" + "=".PadLeft(80, '=') + "\n");


        Console.WriteLine("Environment Variables:");
        Console.WriteLine($"JWT_PRIVATE_KEY=\"{privateKey.Replace("\r\n", "\n").Replace("\n", "\\n")}\"");
        Console.WriteLine($"JWT_PUBLIC_KEY=\"{publicKey.Replace("\r\n", "\n").Replace("\n", "\\n")}\"");
    }
}
