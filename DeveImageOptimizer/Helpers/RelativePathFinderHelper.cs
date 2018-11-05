using System;
using System.IO;

namespace DeveImageOptimizer.Helpers
{
    public static class RelativePathFinderHelper
    {
        public static string GetRelativePath(string originDirectory, string fullPathOfFile)
        {
            return GetRelativePath(originDirectory != null ? new DirectoryInfo(originDirectory) : null, fullPathOfFile != null ? new FileInfo(fullPathOfFile) : null);
        }

        public static string GetRelativePath(FileSystemInfo destinationPath, FileSystemInfo originPath)
        {
            if (originPath == null)
            {
                throw new ArgumentNullException(nameof(originPath));
            }

            if (destinationPath == null)
            {
                return originPath.FullName;
            }

            string path1FullName = GetFullName(destinationPath);
            string path2FullName = GetFullName(originPath);

            Uri uri1 = new Uri(path1FullName);
            Uri uri2 = new Uri(path2FullName);
            Uri relativeUri = uri1.MakeRelativeUri(uri2);

            return relativeUri.OriginalString;
        }

        private static string GetFullName(FileSystemInfo path)
        {
            string fullName = path.FullName;

            if (path is DirectoryInfo && fullName[fullName.Length - 1] != Path.DirectorySeparatorChar)
            {
                fullName += Path.DirectorySeparatorChar;
            }
            return fullName;
        }
    }
}
