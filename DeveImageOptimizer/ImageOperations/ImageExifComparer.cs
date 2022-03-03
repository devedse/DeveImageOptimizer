using DeveImageOptimizer.Helpers;
using ExifLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DeveImageOptimizer.ImageOperations
{
    public static class ImageExifComparer
    {
        public static async Task<bool> AreImageExifDatasEqual(string image1Path, string image2Path)
        {
            if (!File.Exists(image1Path))
            {
                throw new FileNotFoundException("Could not find Image1", image1Path);
            }

            if (!File.Exists(image2Path))
            {
                throw new FileNotFoundException("Could not find Image2", image2Path);
            }

            if (!FileTypeHelper.IsJpgFile(image1Path) || !FileTypeHelper.IsJpgFile(image2Path))
            {
                //For now only handle Jpg images
                return true;
            }

            var image1 = await ImageFile.FromFileAsync(image1Path);
            var image2 = await ImageFile.FromFileAsync(image2Path);

            foreach (var prop1 in image1.Properties)
            {
                //We check if all properties exist in the other image, but not the other way around.
                //For some reason the optimizers add some exif data in some cases
                if (!image2.Properties.Any(prop2 =>
                    prop1.Name == prop2.Name &&
                    prop1.Tag == prop2.Tag &&
                    prop1.IFD == prop2.IFD &&
                    prop1.Interoperability.TagID == prop2.Interoperability.TagID &&
                    prop1.Interoperability.TypeID == prop2.Interoperability.TypeID &&
                    prop1.Interoperability.Count == prop2.Interoperability.Count &&
                    Enumerable.SequenceEqual(prop1.Interoperability.Data, prop2.Interoperability.Data)
                    ))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
