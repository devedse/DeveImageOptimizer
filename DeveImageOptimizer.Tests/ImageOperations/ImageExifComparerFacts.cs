using DeveImageOptimizer.Helpers;
using DeveImageOptimizer.ImageOperations;
using ExifLibrary;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DeveImageOptimizer.Tests.ImageOperations
{
    public class ImageExifComparerFacts
    {
        [Fact]
        public async Task FindsOutThatExifDataIsTheSame()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "After.JPG");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "AfterWithSameExif.JPG");

            var areEqual = await ImageExifComparer.AreImageExifDatasEqual(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public async Task FindsOutThatExifDataIsDifferent()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "After.JPG");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "AfterWithDifferentExif.JPG");

            var areEqual = await ImageExifComparer.AreImageExifDatasEqual(imageApath, imageBpath);

            Assert.False(areEqual);
        }

        [Fact]
        public async Task FindsOutThatExifDataIsDifferent2()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "After.JPG");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "AfterWithDifferentExif2.JPG");

            var areEqual = await ImageExifComparer.AreImageExifDatasEqual(imageApath, imageBpath);

            Assert.False(areEqual);
        }

        [Fact]
        public async Task DoesntFailWhenThereIsWrongExifButTheImportantExifIsCorrect()
        {
            var imageApath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "EXIFTEST_BeforeOptimization.JPG");
            var imageBpath = Path.Combine(FolderHelperMethods.Internal_AssemblyDirectory.Value, "TestImages", "EXIFTEST_AfterOptimization.JPG");

            var areEqual = await ImageExifComparer.AreImageExifDatasEqual(imageApath, imageBpath);

            Assert.True(areEqual);
        }

        [Fact]
        public void RotationTest_WorksIfWasNormalAndIsNowStillNormal()
        {
            ImageFile imageA = new TestImageFile(new ExifEnumProperty<Orientation>(ExifTag.Orientation, Orientation.Normal));
            ImageFile imageB = new TestImageFile(new ExifEnumProperty<Orientation>(ExifTag.Orientation, Orientation.Normal));

            var areEqual = ImageExifComparer.AreImageExifDatasEqual(imageA, imageB);

            Assert.True(areEqual);
        }

        [Fact]
        public void RotationTest_WorksIfWasNormalAndIsNowRemoved()
        {
            ImageFile imageA = new TestImageFile(new ExifEnumProperty<Orientation>(ExifTag.Orientation, Orientation.Normal));
            ImageFile imageB = new TestImageFile();

            var areEqual = ImageExifComparer.AreImageExifDatasEqual(imageA, imageB);

            Assert.True(areEqual);
        }

        [Fact]
        public void RotationTest_WorksIfWasNormalAndIsNowChanged()
        {
            ImageFile imageA = new TestImageFile(new ExifEnumProperty<Orientation>(ExifTag.Orientation, Orientation.Normal));
            ImageFile imageB = new TestImageFile(new ExifEnumProperty<Orientation>(ExifTag.Orientation, Orientation.Flipped));

            var areEqual = ImageExifComparer.AreImageExifDatasEqual(imageA, imageB);

            Assert.False(areEqual);
        }

        [Fact]
        public void RotationTest_WorksIfWasFlippedAndIsNowReoved()
        {
            ImageFile imageA = new TestImageFile(new ExifEnumProperty<Orientation>(ExifTag.Orientation, Orientation.Flipped));
            ImageFile imageB = new TestImageFile();

            var areEqual = ImageExifComparer.AreImageExifDatasEqual(imageA, imageB);

            Assert.False(areEqual);
        }

        [Fact]
        public void RotationTest_WorksIfWasRemovedBeforeAndAfter()
        {
            ImageFile imageA = new TestImageFile();
            ImageFile imageB = new TestImageFile();

            var areEqual = ImageExifComparer.AreImageExifDatasEqual(imageA, imageB);

            Assert.True(areEqual);
        }
    }

    public class TestImageFile : ImageFile
    {
        public TestImageFile(params ExifProperty[] collection)
        {
            foreach (var prop in collection)
            {
                Properties.Add(prop);
            }
        }

        public override void Crush()
        {

        }

        protected override void SaveInternal(MemoryStream stream)
        {

        }
    }
}
