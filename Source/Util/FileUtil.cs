using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Windows;

namespace SealFisher.Util
{
    public static class FileUtil
    {
        public static string StrFromResource(string fileName)
        {
            var uri = new Uri($"pack://application:,,,/Resources/Shader/{fileName}");
            var streamInfo = Application.GetResourceStream(uri);

            using var reader = new StreamReader(streamInfo.Stream);
            return reader.ReadToEnd();
        }

        public static Image<Rgba32> ImgFromResource(string fileName)
        {
            var uri = new Uri($"pack://application:,,,/Resources/{fileName}");
            var resource = Application.GetResourceStream(uri);

            return Image.Load<Rgba32>(resource.Stream);
        }
    }
}
