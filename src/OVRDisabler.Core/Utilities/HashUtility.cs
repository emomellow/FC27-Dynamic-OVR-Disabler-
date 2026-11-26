using System.Security.Cryptography;
using System.Text;

namespace OVRDisabler.Core.Utilities;

public static class HashUtility
{
    public static string ComputeSha256(byte[] data)
    {
        byte[] hash = SHA256.HashData(data);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public static string ComputeSha256(string text, Encoding encoding)
    {
        return ComputeSha256(encoding.GetBytes(text));
    }

    public static string ComputeSha256(string filePath)
    {
        if (!File.Exists(filePath)) return string.Empty;
        using var stream = File.OpenRead(filePath);
        byte[] hash = SHA256.HashData(stream);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
