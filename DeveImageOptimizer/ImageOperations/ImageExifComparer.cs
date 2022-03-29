using DeveImageOptimizer.Helpers;
using ExifLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DeveImageOptimizer.ImageOperations
{
    public static class ImageExifComparer
    {
        private static Dictionary<ExifTag, Func<ExifProperty, ExifPropertyCollection<ExifProperty>, bool>> _validExifTags = new()
        {
            { ExifTag.Make, ExifPropertyEqualInDestination },
            { ExifTag.Model, ExifPropertyEqualInDestination },
            { ExifTag.Orientation, ExifPropertyEqualInDestinationOrCompletelyMissingIfProp1ConformsToValue(prop1 => (prop1 as ExifEnumProperty<Orientation>)?.Tag == ExifTag.Orientation && (prop1 as ExifEnumProperty<Orientation>)?.Value == Orientation.Normal)},
            { ExifTag.Software, ExifPropertyEqualInDestination },
            { ExifTag.DateTime, ExifPropertyEqualInDestination },
            { ExifTag.ExposureTime, ExifPropertyEqualInDestination },
            { ExifTag.FNumber, ExifPropertyEqualInDestination },
            { ExifTag.ExposureProgram, ExifPropertyEqualInDestination },
            { ExifTag.ISOSpeedRatings, ExifPropertyEqualInDestination },
            { ExifTag.DateTimeOriginal, ExifPropertyEqualInDestination },
            { ExifTag.DateTimeDigitized, ExifPropertyEqualInDestination },
            { ExifTag.BrightnessValue, ExifPropertyEqualInDestination },
            { ExifTag.ExposureBiasValue, ExifPropertyEqualInDestination },
            { ExifTag.MaxApertureValue, ExifPropertyEqualInDestination },
            { ExifTag.MeteringMode, ExifPropertyEqualInDestination },
            { ExifTag.LightSource, ExifPropertyEqualInDestination },
            { ExifTag.Flash, ExifPropertyEqualInDestination },
            { ExifTag.FocalLength, ExifPropertyEqualInDestination },
            { ExifTag.MakerNote, ExifPropertyEqualInDestination },
            { ExifTag.UserComment, ExifPropertyEqualInDestination },
            { ExifTag.PixelXDimension, ExifPropertyEqualInDestination },
            { ExifTag.PixelYDimension, ExifPropertyEqualInDestination },
            { ExifTag.ExposureMode, ExifPropertyEqualInDestination },
            { ExifTag.WhiteBalance, ExifPropertyEqualInDestination },
            { ExifTag.DigitalZoomRatio, ExifPropertyEqualInDestination },
            { ExifTag.FocalLengthIn35mmFilm, ExifPropertyEqualInDestination },
            { ExifTag.SceneCaptureType, ExifPropertyEqualInDestination },
            { ExifTag.Contrast, ExifPropertyEqualInDestination },
            { ExifTag.Saturation, ExifPropertyEqualInDestination },
            { ExifTag.Sharpness, ExifPropertyEqualInDestination },
            { ExifTag.LensSpecification, ExifPropertyEqualInDestination },
            { ExifTag.LensModel, ExifPropertyEqualInDestination },

            { ExifTag.ApertureValue, ExifPropertyEqualInDestination }
        };

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

            return AreImageExifDatasEqual(image1, image2);
        }

        public static bool AreImageExifDatasEqual(ImageFile image1, ImageFile image2)
        {
            foreach (var prop1 in image1.Properties)
            {
                if (_validExifTags.TryGetValue(prop1.Tag, out var checkerFunc))
                {
                    if (!checkerFunc(prop1, image2.Properties))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool ExifPropertyEqualInDestination(ExifProperty prop1, ExifPropertyCollection<ExifProperty> collection)
        {
            //We check if all properties exist in the other image, but not the other way around.
            //For some reason the optimizers add some exif data in some cases
            return collection.Any(prop2 =>
                prop1.Name == prop2.Name &&
                prop1.Tag == prop2.Tag &&
                prop1.IFD == prop2.IFD &&
                prop1.Interoperability.TagID == prop2.Interoperability.TagID &&
                prop1.Interoperability.TypeID == prop2.Interoperability.TypeID &&
                prop1.Interoperability.Count == prop2.Interoperability.Count &&
                Enumerable.SequenceEqual(prop1.Interoperability.Data, prop2.Interoperability.Data)
                );
        }

        private static bool ExifPropertyEqualInDestinationOrCompletelyMissing(ExifProperty prop1, ExifPropertyCollection<ExifProperty> collection)
        {
            //First check if it is exactly equal, if not make sure it is completely missing
            var result = ExifPropertyEqualInDestination(prop1, collection);
            if (result)
            {
                return true;
            }
            if (collection.All(prop2 => prop1.Tag != prop2.Tag))
            {
                return true;
            }
            return false;
        }

        private static Func<ExifProperty, ExifPropertyCollection<ExifProperty>, bool> ExifPropertyEqualInDestinationOrCompletelyMissingIfProp1ConformsToValue(Func<ExifProperty, bool> conformsToValue)
        {
            return new Func<ExifProperty, ExifPropertyCollection<ExifProperty>, bool>((prop1, collection) =>
            {
                //First check if it is exactly equal, if not make sure it is completely missing
                var result = ExifPropertyEqualInDestination(prop1, collection);
                if (result)
                {
                    return true;
                }
                if (conformsToValue(prop1) && collection.All(prop2 => prop1.Tag != prop2.Tag))
                {
                    return true;
                }
                return false;
            });
        }
    }
}
