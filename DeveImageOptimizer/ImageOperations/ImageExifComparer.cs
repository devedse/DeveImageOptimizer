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

            var image1 = await ImageFile.FromFileAsync(image1Path);
            var image2 = await ImageFile.FromFileAsync(image2Path);

            if (image1.Properties.Count != image2.Properties.Count)
            {
                return false;
            }

            var image1Properties = image1.Properties.OrderBy(t => t.Name).ToList();
            var image2Properties = image2.Properties.OrderBy(t => t.Name).ToList();

            for (int i = 0; i < image1Properties.Count; i++)
            {
                var prop1 = image1Properties[i];
                var prop2 = image2Properties[i];
                var prop1type = prop1.Value.GetType();
                var prop2type = prop2.Value.GetType();

                if (prop1.Name != prop2.Name || prop1.Tag != prop2.Tag || prop1.IFD != prop2.IFD)
                {
                    return false;
                }

                if (prop1.Interoperability.TagID != prop2.Interoperability.TagID ||
                    prop1.Interoperability.TypeID != prop2.Interoperability.TypeID ||
                    prop1.Interoperability.Count != prop2.Interoperability.Count ||
                    !Enumerable.SequenceEqual(prop1.Interoperability.Data, prop2.Interoperability.Data))
                {
                    return false;
                }


                //if (prop1type == prop2type)
                //{
                //    bool result;
                //    if (prop1.Value is IEnumerable prop1enumerable && prop2.Value is IEnumerable prop2enumerable)
                //    {
                //        result = Enumerable.SequenceEqual(prop1enumerable.OfType<object>(), prop2enumerable.OfType<object>());
                //        //result = !prop1enumerable.OfType<object>().Except(prop2enumerable.OfType<object>()).Any();                        
                //    }
                //    else
                //    {
                //        result = prop1.Value.Equals(prop2.Value);
                //    }
                //    if (!result)
                //    {
                //        result = prop1.Value.ToString().Equals(prop2.Value.ToString());
                //        if (!result)
                //        {
                //            return false;
                //        }
                //    }
                //}
                //else
                //{
                //    return false;
                //}
            }

            return true;
        }
    }
}
