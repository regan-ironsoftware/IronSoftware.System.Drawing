using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;
using Xunit.Abstractions;

namespace IronSoftware.Drawing.Common.Tests.UnitTests
{
    public class AnyBitmapFunctionality : Compare
    {
        public AnyBitmapFunctionality(ITestOutputHelper output) : base(output)
        {
        }

        [FactWithAutomaticDisplayName]
        public void Create_AnyBitmap_by_Filename()
        {
            string imagePath = GetRelativeFilePath("Mona-Lisa-oil-wood-panel-Leonardo-da.webp");

            AnyBitmap bitmap = AnyBitmap.FromFile(imagePath);
            bitmap.SaveAs("result.bmp");
            Assert.Equal(671, bitmap.Width);
            Assert.Equal(1000, bitmap.Height);
            Assert.Equal(74684, bitmap.Length);
            AssertImageAreEqual(imagePath, "result.bmp");

            bitmap = new AnyBitmap(imagePath);
            bitmap.SaveAs("result.bmp");
            Assert.Equal(671, bitmap.Width);
            Assert.Equal(1000, bitmap.Height);
            Assert.Equal(74684, bitmap.Length);
            AssertImageAreEqual(imagePath, "result.bmp");
        }

        [FactWithAutomaticDisplayName]
        public void Create_AnyBitmap_by_Byte()
        {            
            string imagePath = GetRelativeFilePath("Mona-Lisa-oil-wood-panel-Leonardo-da.webp");
            byte[] bytes = File.ReadAllBytes(imagePath);

            AnyBitmap bitmap = AnyBitmap.FromBytes(bytes);
            bitmap.TrySaveAs("result.bmp");
            AssertImageAreEqual(imagePath, "result.bmp");

            bitmap = new AnyBitmap(bytes);
            bitmap.TrySaveAs("result.bmp");
            AssertImageAreEqual(imagePath, "result.bmp");
        }

        [FactWithAutomaticDisplayName]
        public void Create_AnyBitmap_by_Stream()
        {
            string imagePath = GetRelativeFilePath("Mona-Lisa-oil-wood-panel-Leonardo-da.webp");
            byte[] bytes = File.ReadAllBytes(imagePath);
            MemoryStream ms = new MemoryStream(bytes);

            AnyBitmap bitmap = AnyBitmap.FromStream(ms);
            bitmap.TrySaveAs("result.bmp");
            AssertImageAreEqual(imagePath, "result.bmp");

            bitmap = new AnyBitmap(ms);
            bitmap.SaveAs("result.bmp");
            AssertImageAreEqual(imagePath, "result.bmp");
        }

        [FactWithAutomaticDisplayName]
        public void Create_SVG_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("Example_barcode.svg");
            AnyBitmap bitmap = AnyBitmap.FromFile(imagePath);
            bitmap.SaveAs("result.bmp");
            AssertImageAreEqual(imagePath, "result.bmp");
        }

        [FactWithAutomaticDisplayName]
        public void Try_Save_Bitmap_with_Format()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            AnyBitmap anyBitmap = new AnyBitmap(imagePath);

            anyBitmap.SaveAs("result-png.png", AnyBitmap.ImageFormat.Png);
            Assert.True(File.Exists("result-png.png"));
            CleanResultFile("result-png.png");

            anyBitmap.SaveAs("result-png-loss.png", AnyBitmap.ImageFormat.Png, 50);
            Assert.True(File.Exists("result-png-loss.png"));
            CleanResultFile("result-png-loss.png");

            anyBitmap.TrySaveAs("result-try-png.png", AnyBitmap.ImageFormat.Png);
            Assert.True(File.Exists("result-try-png.png"));
            CleanResultFile("result-try-png.png");

            anyBitmap.TrySaveAs("result-try-png-loss.png", AnyBitmap.ImageFormat.Png, 50);
            Assert.True(File.Exists("result-try-png-loss.png"));
            CleanResultFile("result-try-png-loss.png");
        }

        [FactWithAutomaticDisplayName]
        public void Export_file()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            AnyBitmap anyBitmap = new AnyBitmap(imagePath);

            anyBitmap.ExportFile("result.png");
            Assert.True(File.Exists("result.png"));
            CleanResultFile("result.png");

            anyBitmap.ExportFile("result-png.png", AnyBitmap.ImageFormat.Png);
            Assert.True(File.Exists("result-png.png"));
            CleanResultFile("result-png.png");

            anyBitmap.ExportFile("result-png-loss.png", AnyBitmap.ImageFormat.Png, 50);
            Assert.True(File.Exists("result-png-loss.png"));
            CleanResultFile("result-png-loss.png");
        }

        [FactWithAutomaticDisplayName]
        public void CastBitmap_to_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(imagePath);
            AnyBitmap anyBitmap = bitmap;

            bitmap.Save("expected.bmp");
            anyBitmap.SaveAs("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastBitmap_from_AnyBitmap()
        {
            AnyBitmap anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
            System.Drawing.Bitmap bitmap = anyBitmap;

            anyBitmap.SaveAs("expected.bmp");
            bitmap.Save("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [FactWithAutomaticDisplayName]
        public void AnyBitmap_should_get_Bytes()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            AnyBitmap anyBitmap = AnyBitmap.FromFile(imagePath);
            byte[] expected = File.ReadAllBytes(imagePath);

            byte[] result = anyBitmap.GetBytes();
            Assert.Equal(expected, result);

            byte[] resultExport = anyBitmap.ExportBytes();
            Assert.Equal(expected, result);
        }

        [FactWithAutomaticDisplayName]
        public void AnyBitmap_should_get_Stream()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            AnyBitmap anyBitmap = AnyBitmap.FromFile(imagePath);
            MemoryStream expected = new MemoryStream(File.ReadAllBytes(imagePath));

            MemoryStream result = anyBitmap.GetStream();
            AssertStreamAreEqual(expected, result);

            result = anyBitmap.ToStream();
            AssertStreamAreEqual(expected, result);

            Func<Stream> funcStream = anyBitmap.ToStreamFn();
            AssertStreamAreEqual(expected, funcStream);

            using var resultExport = new System.IO.MemoryStream();
            anyBitmap.ExportStream(resultExport);
            AssertStreamAreEqual(expected, (MemoryStream)resultExport);
        }

        [FactWithAutomaticDisplayName]
        public void AnyBitmap_should_get_Hashcode()
        {
            string imagePath = GetRelativeFilePath("Mona-Lisa-oil-wood-panel-Leonardo-da.webp");
            AnyBitmap anyBitmap = AnyBitmap.FromFile(imagePath);

            byte[] bytes = System.IO.File.ReadAllBytes(imagePath);
            Assert.Equal(bytes, anyBitmap.GetBytes());

            int expected = anyBitmap.GetBytes().GetHashCode();
            int result = anyBitmap.GetHashCode();
            Assert.Equal(expected, result);
        }

        [FactWithAutomaticDisplayName]
        public void AnyBitmap_should_ToString()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            AnyBitmap anyBitmap = AnyBitmap.FromFile(imagePath);

            byte[] bytes = File.ReadAllBytes(imagePath);
            string expected = Convert.ToBase64String(bytes, 0, bytes.Length);

            string result = anyBitmap.ToString();
            Assert.Equal(expected, result);
        }

        [FactWithAutomaticDisplayName]
        public void Clone_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            AnyBitmap anyBitmap = AnyBitmap.FromFile(imagePath);
            AnyBitmap clonedAnyBitmap = anyBitmap.Clone();

            anyBitmap.SaveAs("expected.png");
            clonedAnyBitmap.SaveAs("result.png");

            AssertImageAreEqual("expected.png", "result.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKBitmap_to_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            SkiaSharp.SKBitmap skBitmap = SkiaSharp.SKBitmap.Decode(imagePath);
            AnyBitmap anyBitmap = skBitmap;

            SaveSkiaBitmap(skBitmap, "expected.png");
            anyBitmap.SaveAs("result.png");

            AssertImageAreEqual("expected.png", "result.png", true);

            imagePath = GetRelativeFilePath("Sample-Tiff-File-download-for-Testing.tiff");
            skBitmap = IronSkiasharpBitmap.OpenTiffToSKBitmap(imagePath);
            anyBitmap = skBitmap;

            SaveSkiaBitmap(skBitmap, "expected.png");
            anyBitmap.SaveAs("result.png");

            AssertImageAreEqual("expected.png", "result.png", true);

            byte[] bytes = File.ReadAllBytes(imagePath);
            skBitmap = IronSkiasharpBitmap.OpenTiffToSKBitmap(bytes);
            anyBitmap = skBitmap;

            SaveSkiaBitmap(skBitmap, "expected.png");
            anyBitmap.SaveAs("result.png");

            AssertImageAreEqual("expected.png", "result.png", true);

            MemoryStream memStream = new MemoryStream();
            using (FileStream fs = File.OpenRead(imagePath))
            {
                fs.CopyTo(memStream);
                memStream.Position = 0;
            }
            skBitmap = IronSkiasharpBitmap.OpenTiffToSKBitmap(memStream);
            anyBitmap = skBitmap;

            SaveSkiaBitmap(skBitmap, "expected.png");
            anyBitmap.SaveAs("result.png");

            AssertImageAreEqual("expected.png", "result.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKBitmap_from_AnyBitmap()
        {
            AnyBitmap anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
            SkiaSharp.SKBitmap skBitmap = anyBitmap;

            anyBitmap.SaveAs("expected.png");
            SaveSkiaBitmap(skBitmap, "result.png");

            AssertImageAreEqual("expected.png", "result.png", true);

            anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("Sample-Tiff-File-download-for-Testing.tiff"));
            skBitmap = anyBitmap;

            anyBitmap.SaveAs("expected.png");
            SaveSkiaBitmap(skBitmap, "result.png");

            AssertImageAreEqual("expected.png", "result.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKImage_to_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            SkiaSharp.SKImage skImage = SkiaSharp.SKImage.FromBitmap(SkiaSharp.SKBitmap.Decode(imagePath));
            AnyBitmap anyBitmap = skImage;

            SaveSkiaImage(skImage, "expected.png");
            anyBitmap.SaveAs("result.png");

            AssertImageAreEqual("expected.png", "result.png", true);

            imagePath = GetRelativeFilePath("Sample-Tiff-File-download-for-Testing.tiff");
            skImage = SkiaSharp.SKImage.FromBitmap(IronSkiasharpBitmap.OpenTiffToSKBitmap(imagePath));
            anyBitmap = skImage;

            SaveSkiaImage(skImage, "expected.png");
            anyBitmap.SaveAs("result.png");

            AssertImageAreEqual("expected.png", "result.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKImage_from_AnyBitmap()
        {
            AnyBitmap anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
            SkiaSharp.SKImage skImage = anyBitmap;

            anyBitmap.SaveAs("expected.png");
            SaveSkiaImage(skImage, "result.png");

            AssertImageAreEqual("expected.png", "result.png", true);

            anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("Sample-Tiff-File-download-for-Testing.tiff"));
            skImage = anyBitmap;

            anyBitmap.SaveAs("expected.png");
            SaveSkiaImage(skImage, "result.png");

            AssertImageAreEqual("expected.png", "result.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastSixLabors_to_AnyBitmap()
        {
#if NET7_0
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                string imagePath = GetRelativeFilePath("mountainclimbers.jpg");
                SixLabors.ImageSharp.Image imgSharp;
                var exception = Assert.Throws<System.PlatformNotSupportedException>(() => imgSharp = SixLabors.ImageSharp.Image.Load(imagePath));
                Assert.Equal("Operation is not supported on this platform.", exception.Message);
            }
            else
#endif
            {
                string imagePath = GetRelativeFilePath("mountainclimbers.jpg");
                SixLabors.ImageSharp.Image imgSharp = SixLabors.ImageSharp.Image.Load(imagePath);
                AnyBitmap anyBitmap = imgSharp;

                imgSharp.Save("expected.bmp");
                anyBitmap.SaveAs("result.bmp");

                AssertImageAreEqual("expected.bmp", "result.bmp", true);
            }
        }

        [FactWithAutomaticDisplayName]
        public void CastSixLabors_from_AnyBitmap()
        {
#if NET7_0
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                AnyBitmap anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("mountainclimbers.jpg"));
                SixLabors.ImageSharp.Image imgSharp;
                var exception = Assert.Throws<System.PlatformNotSupportedException>(() => imgSharp = anyBitmap);
                Assert.Equal("Operation is not supported on this platform.", exception.Message);
            }
            else
#endif
            { 
                AnyBitmap anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("mountainclimbers.jpg"));
                SixLabors.ImageSharp.Image imgSharp = anyBitmap;

                anyBitmap.SaveAs("expected.bmp");
                imgSharp.Save("result.bmp");

                AssertImageAreEqual("expected.bmp", "result.bmp", true);            
            }
        }


        [FactWithAutomaticDisplayName]
        public void CastMaui_to_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            byte[] bytes = File.ReadAllBytes(imagePath);
            Microsoft.Maui.Graphics.Platform.PlatformImage image = (Microsoft.Maui.Graphics.Platform.PlatformImage)Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(new MemoryStream(bytes));
            AnyBitmap anyBitmap = image;

            SaveMauiImages(image, "expected.bmp");
            anyBitmap.SaveAs("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastMaui_from_AnyBitmap()
        {
            AnyBitmap anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
            Microsoft.Maui.Graphics.Platform.PlatformImage image = anyBitmap;

            anyBitmap.SaveAs("expected.bmp");
            SaveMauiImages(image, "result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

    }
}
