﻿using SkiaSharp;
using System;
using System.Runtime.InteropServices;

namespace IronSoftware.Drawing
{
    /// <summary>
    /// For internal usage
    /// </summary>
    public static class IronSkiasharpBitmap
    {
        /// <summary>
        /// Resize an image with scaling.
        /// </summary>
        /// <param name="bitmap">Original bitmap to resize.</param>
        /// <param name="scale">Scale of new image 0 - 1.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static SKBitmap Resize(this SKBitmap bitmap, float scale)
        {
            if (bitmap != null)
            {
                SKBitmap toBitmap = new SKBitmap((int)(bitmap.Width * scale), (int)(bitmap.Height * scale), bitmap.ColorType, bitmap.AlphaType);

                using (SKCanvas canvas = new SKCanvas(toBitmap))
                {
                    canvas.SetMatrix(SKMatrix.CreateScale(scale, scale));
                    canvas.DrawBitmap(bitmap, 0, 0, CreateHighQualityPaint());
                    canvas.ResetMatrix();
                    canvas.Flush();
                }

                return toBitmap;
            }
            else
            {
                throw new Exception("Please provide a bitmap to process.");
            }
        }

        /// <summary>
        /// Resize an image with width and height.
        /// </summary>
        /// <param name="bitmap">Original bitmap to resize.</param>
        /// <param name="width">Width of the new resized image.</param>
        /// <param name="height">Height of the new resized image.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static SKBitmap Resize(this SKBitmap bitmap, int width, int height)
        {
            if (bitmap != null)
            {
                SKBitmap toBitmap = new SKBitmap(width, height, bitmap.ColorType, bitmap.AlphaType);

                using (SKCanvas canvas = new SKCanvas(toBitmap))
                {
                    canvas.SetMatrix(SKMatrix.CreateScale(CalculateScaleOfWidth(bitmap, width), CalculateScaleOfHeight(bitmap, height)));
                    canvas.DrawBitmap(bitmap, 0, 0, CreateHighQualityPaint());
                    canvas.ResetMatrix();
                    canvas.Flush();
                }
                return toBitmap;
            }
            else
            {
                throw new Exception("Please provide a bitmap to process.");
            }
        }

        /// <summary>
        /// Resize an image with width and height.
        /// </summary>
        /// <param name="bitmap">Original bitmap to crop.</param>
        /// <param name="cropArea">Crop area for the image.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static SKBitmap CropImage(this SKBitmap bitmap, CropRectangle cropArea)
        {
            if (cropArea != null && bitmap != null)
            {
                SKRect cropRect = ValidateCropArea(bitmap, cropArea);
                SKBitmap croppedBitmap = new SKBitmap((int)cropRect.Width, (int)cropRect.Height);

                SKRect dest = new SKRect(0, 0, cropRect.Width, cropRect.Height);
                SKRect source = new SKRect(cropRect.Left, cropRect.Top, cropRect.Right, cropRect.Bottom);

                try
                {
                    using (SKCanvas canvas = new SKCanvas(croppedBitmap))
                    {
                        canvas.DrawBitmap(bitmap, source, dest, CreateHighQualityPaint());
                    }

                    return croppedBitmap;
                }
                catch (OutOfMemoryException ex)
                {
                    try { croppedBitmap.Dispose(); } catch { }
                    throw new Exception("Crop Rectangle is larger than the input image.", ex);
                }
            }
            else
            {
                throw new Exception("Please provide a bitmap and crop area to process.");
            }
        }

        /// <summary>
        /// Crop an image with width and height.
        /// </summary>
        /// <param name="bitmap">Original bitmap to crop.</param>
        /// <param name="width">Width of the new cropped image.</param>
        /// <param name="height">Height of the new cropped image.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static SKBitmap CropImage(this SKBitmap bitmap, int width, int height)
        {

            if (bitmap != null)
            {
                SKBitmap toBitmap = new SKBitmap(width, height, bitmap.ColorType, bitmap.AlphaType);
                bitmap.ExtractSubset(toBitmap, new CropRectangle(0, 0, width, height));

                return toBitmap;
            }
            else
            {
                throw new Exception("Please provide a bitmap to process.");
            }
        }

        /// <summary>
        /// Rotate an image. 
        /// </summary>
        /// <param name="bitmap">Original bitmap to rotate.</param>
        /// <param name="angle">Angle to rotate the image.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static SKBitmap RotateImage(this SKBitmap bitmap, double angle)
        {
            if (bitmap != null)
            {
                double radians = Math.PI * angle / 180;
                float sine = (float)Math.Abs(Math.Sin(radians));
                float cosine = (float)Math.Abs(Math.Cos(radians));

                int originalWidth = (bitmap).Width;
                int originalHeight = (bitmap).Height;
                int rotatedWidth = (int)(cosine * originalWidth + sine * originalHeight);
                int rotatedHeight = (int)(cosine * originalHeight + sine * originalWidth);

                SKBitmap rotatedBitmap = new SKBitmap(rotatedWidth, rotatedHeight);

                using (SKCanvas canvas = new SKCanvas(rotatedBitmap))
                {
                    canvas.Clear();
                    canvas.Translate(rotatedWidth / 2, rotatedHeight / 2);
                    canvas.RotateDegrees((float)angle);
                    canvas.Translate(-originalWidth / 2, -originalHeight / 2);
                    canvas.DrawBitmap(bitmap, new SKPoint(), CreateHighQualityPaint());
                }

                return rotatedBitmap;
            }
            else
            {
                throw new Exception("Please provide a bitmap to process.");
            }
        }

        /// <summary>
        /// Trim white space of the image.
        /// </summary>
        /// <param name="bitmap">Original bitmap to trim.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static SKBitmap Trim(this SKBitmap bitmap)
        {
            if (bitmap != null)
            {
                int[] rgbValues = new int[bitmap.Height * bitmap.Width];
                Marshal.Copy(bitmap.GetPixels(), rgbValues, 0, rgbValues.Length);

                int left = bitmap.Width;
                int top = bitmap.Height;
                int right = 0;
                int bottom = 0;

                DetermineTop(bitmap, rgbValues, ref left, ref top, ref right, ref bottom);
                DetermineBottom(bitmap, rgbValues, ref left, ref right, ref bottom);

                if (bottom > top)
                {
                    DetermineLeftAndRight(bitmap, rgbValues, ref left, top, ref right, bottom);
                }

                return bitmap.CropImage(new SKRect(left, top, right, bottom));
            }
            else
            {
                throw new Exception("Please provide a bitmap to process.");
            }
        }

        /// <summary>
        /// Add a colored border around the image.
        /// </summary>
        /// <param name="bitmap">Original bitmap to add a border to.</param>
        /// <param name="color">Color of the border.</param>
        /// <param name="width">Width of the border in pixel.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static SKBitmap AddBorder(this SKBitmap bitmap, IronSoftware.Drawing.Color color, int width)
        {
            if (bitmap != null)
            {
                int maxWidth = bitmap.Width + width * 2;
                int maxHeight = bitmap.Height + width * 2;
                SKBitmap toBitmap = new SKBitmap(maxWidth, maxHeight);

                using (SKCanvas canvas = new SKCanvas(toBitmap))
                {
                    canvas.Clear(color);
                    SKRect dest = new SKRect(width, width, width + bitmap.Width, width + bitmap.Height);
                    canvas.DrawBitmap(bitmap, dest, CreateHighQualityPaint());
                    canvas.Flush();
                }

                return toBitmap;
            }
            else
            {
                throw new Exception("Please provide a bitmap to process.");
            }
        }

        #region Private Method

        private static CropRectangle ValidateCropArea(SKBitmap img, CropRectangle CropArea)
        {
            int maxWidth = img.Width;
            int maxHeight = img.Height;

            int cropAreaX = CropArea.X > 0 ? CropArea.X : 0;
            int cropAreaY = CropArea.Y > 0 ? CropArea.Y : 0;
            int cropAreaWidth = CropArea.Width > 0 ? CropArea.Width : img.Width;
            int cropAreaHeight = CropArea.Height > 0 ? CropArea.Height : img.Height;

            int croppedWidth = cropAreaX + cropAreaWidth;
            int croppedHeight = cropAreaY + cropAreaHeight;

            int newWidth = cropAreaWidth;
            int newHeight = cropAreaHeight;
            if (croppedWidth > maxWidth)
            {
                newWidth = maxWidth - cropAreaX;
            }
            if (croppedHeight > maxHeight)
            {
                newHeight = maxHeight - cropAreaY;
            }
            return new CropRectangle(cropAreaX, cropAreaY, newWidth, newHeight);
        }

        private static void DetermineLeftAndRight(SKBitmap originalBitmap, int[] rgbValues, ref int left, int top, ref int right, int bottom)
        {
            for (int r = top + 1; r < bottom; r++)
            {
                DetermineLeft(originalBitmap, rgbValues, ref left, r);
                DetermineRight(originalBitmap, rgbValues, ref right, r);
            }
        }

        private static void DetermineRight(SKBitmap originalBitmap, int[] rgbValues, ref int right, int r)
        {
            for (int c = originalBitmap.Width - 1; c > right; c--)
            {
                int color = rgbValues[r * originalBitmap.Width + c] & 0xffffff;
                if (color != 0xffffff)
                {
                    if (right < c)
                    {
                        right = c;
                        break;
                    }
                }
            }
        }

        private static void DetermineLeft(SKBitmap originalBitmap, int[] rgbValues, ref int left, int r)
        {
            for (int c = 0; c < left; c++)
            {
                int color = rgbValues[r * originalBitmap.Width + c] & 0xffffff;
                if (color != 0xffffff)
                {
                    if (left > c)
                    {
                        left = c;
                        break;
                    }
                }
            }
        }

        private static void DetermineBottom(SKBitmap originalBitmap, int[] rgbValues, ref int left, ref int right, ref int bottom)
        {
            for (int i = rgbValues.Length - 1; i >= 0; i--)
            {
                int color = rgbValues[i] & 0xffffff;
                if (color != 0xffffff)
                {
                    int r = i / originalBitmap.Width;
                    int c = i % originalBitmap.Width;

                    if (left > c)
                    {
                        left = c;
                    }
                    if (right < c)
                    {
                        right = c;
                    }
                    bottom = r;
                    break;
                }
            }
        }

        private static void DetermineTop(SKBitmap originalBitmap, int[] rgbValues, ref int left, ref int top, ref int right, ref int bottom)
        {
            for (int i = 0; i < rgbValues.Length; i++)
            {
                int color = rgbValues[i] & 0xffffff;
                if (color != 0xffffff)
                {
                    int r = i / originalBitmap.Width;
                    int c = i % originalBitmap.Width;

                    if (left > c)
                    {
                        left = c;
                    }
                    if (right < c)
                    {
                        right = c;
                    }
                    bottom = r;
                    top = r;
                    break;
                }
            }
        }

        private static float CalculateScaleOfWidth(SKBitmap originalBitmap, int width)
        {
            return (float)width / originalBitmap.Width;
        }

        private static float CalculateScaleOfHeight(SKBitmap originalBitmap, int height)
        {
            return (float)height / originalBitmap.Height;
        }

        private static SKPaint CreateHighQualityPaint()
        {
            return new SKPaint()
            {
                FilterQuality = SKFilterQuality.High
            };
        }

        #endregion
    }
}
