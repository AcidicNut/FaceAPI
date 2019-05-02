using SkiaSharp;
using System;
using System.IO;
using System.Reflection;

namespace FaceAPIHF.ViewModels
{
    static class BitmapExtensions
    {
        // ResourceID alapján betölti az erőforrást és skiasharp-os bitmapot ad vissza.
        public static SKBitmap LoadBitmapResource(Type type, string resourceID)
        {
            Assembly assembly = type.GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                return SKBitmap.Decode(stream);
            }
        }

        // SkiaSharp-os Canvas-re Kirajzol egy képet. "ApectFit"
        public static void DrawBitmap(this SKCanvas canvas, SKBitmap bitmap, SKRect dest,
                                      BitmapStretch stretch)
        {
            // Scaling beállítása Aspect fit-hez hasonlóra
            float scale = Math.Min(dest.Width / bitmap.Width, dest.Height / bitmap.Height);
            SKRect display = CalculateDisplayRect(dest, scale * bitmap.Width, scale * bitmap.Height);

            canvas.DrawBitmap(bitmap, display);
        }

        private static SKRect CalculateDisplayRect(SKRect dest, float bmpWidth, float bmpHeight)
        {
            // Relatív koordináta számolása, majd eltolás a képernyő helyzetével.
            float x = (dest.Width - bmpWidth) / 2 + dest.Left;
            float y = (dest.Height - bmpHeight) / 2 + dest.Top;

            return new SKRect(x, y, x + bmpWidth, y + bmpHeight);
        }
    }

    public enum BitmapStretch
    {
        Uniform,
        AspectFit = Uniform
    }
}
