using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DeveImageOptimizer.Helpers
{
    public static class FileHashCalculator
    {
        public static string CalculateFileHash(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BufferedStream bs = new BufferedStream(fs))
                {
                    using (var hashAlgorithm = SHA512.Create())
                    {
                        var hash = hashAlgorithm.ComputeHash(bs);
                        StringBuilder formatted = new StringBuilder(2 * hash.Length);
                        foreach (byte b in hash)
                        {
                            formatted.AppendFormat("{0:X2}", b);
                        }
                        return formatted.ToString();
                    }
                }
            }
        }
    }
}
