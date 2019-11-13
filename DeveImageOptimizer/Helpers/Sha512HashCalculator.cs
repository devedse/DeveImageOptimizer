using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DeveImageOptimizer.Helpers
{
    public static class Sha512HashCalculator
    {
        public static readonly int ExpectedHashLength = CalculateHash(string.Empty).Length;

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

        public static string CalculateHash(string inputString)
        {
            using (var hashAlgorithm = SHA512.Create())
            {
                var hash = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
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
