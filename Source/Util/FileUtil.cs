using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Windows;

namespace SealFisher.Util
{
    public static class FileUtil
    {
        public static string StrFromResource(string resourcePath)
        {
            var uri = new Uri($"pack://application:,,,/Resources/{resourcePath}");
            var streamInfo = Application.GetResourceStream(uri);

            using var reader = new StreamReader(streamInfo.Stream);
            return reader.ReadToEnd();
        }

        public static Image<Rgba32> ImgFromResource(string resourcePath)
        {
            var uri = new Uri($"pack://application:,,,/Resources/{resourcePath}");
            var resource = Application.GetResourceStream(uri);

            return Image.Load<Rgba32>(resource.Stream);
        }
    }
}
