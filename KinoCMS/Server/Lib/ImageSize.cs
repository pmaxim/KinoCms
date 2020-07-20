using System;
using System.IO;
using System.Threading.Tasks;
using KinoCMS.Shared.Models;
using Microsoft.AspNetCore.Http;
using PhotoSauce.MagicScaler;

namespace KinoCMS.Server.Lib
{
    public static class ImageSize
    {
        public static string ResizeImage(string src)
        {
            const int size = Constants.ImageSize;
            const int quality = 100;
            var settings = new ProcessImageSettings()
            {
                Width = size,
                Height = size,
                ResizeMode = CropScaleMode.Max,
                SaveFormat = FileFormat.Jpeg,
                JpegQuality = quality,
                JpegSubsampleMode = ChromaSubsampleMode.Subsample420
            };
            var buffer = FullSizeToByte(src);
            using var imgStream = new MemoryStream(buffer);
            using var output = new MemoryStream();
            MagicImageProcessor.ProcessImage(imgStream, output, settings);
            var t = ReadToEnd(output);
            return ImageToSrc(t);
        }

        public static string ResizeImageFromByte(byte[] buffer, int size)
        {
            const int quality = 100;
            var settings = new ProcessImageSettings()
            {
                Width = size,
                Height = size,
                ResizeMode = CropScaleMode.Max,
                SaveFormat = FileFormat.Jpeg,
                JpegQuality = quality,
                JpegSubsampleMode = ChromaSubsampleMode.Subsample420
            };
            using var imgStream = new MemoryStream(buffer);
            using var output = new MemoryStream();
            MagicImageProcessor.ProcessImage(imgStream, output, settings);
            var t = ReadToEnd(output);
            return ImageToSrc(t);
        }

        public static async Task ResizeImageToFile(string path, IFormFile file)
        {
            const int size = Constants.ImageSize;
            const int quality = 100;
            var settings = new ProcessImageSettings()
            {
                Width = size,
                Height = size,
                ResizeMode = CropScaleMode.Max,
                SaveFormat = FileFormat.Jpeg,
                JpegQuality = quality,
                JpegSubsampleMode = ChromaSubsampleMode.Subsample420
            };
            await using var imgStream = new MemoryStream();
            await using var output = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(imgStream);
            imgStream.Position = 0;
            MagicImageProcessor.ProcessImage(imgStream, output, settings);
        }

        public static bool ResizeImageToFileSrc(string src, string path)
        {
            const int size = Constants.ImageSize;
            const int quality = 100;
            var settings = new ProcessImageSettings()
            {
                Width = size,
                Height = size,
                ResizeMode = CropScaleMode.Max,
                SaveFormat = FileFormat.Jpeg,
                JpegQuality = quality,
                JpegSubsampleMode = ChromaSubsampleMode.Subsample420
            };
            var buffer = FullSizeToByte(src);
            using var imgStream = new MemoryStream(buffer);
            using var output = new MemoryStream();
            MagicImageProcessor.ProcessImage(imgStream, output, settings);
            var t = ReadToEnd(output);
            return ByteArrayToFile(path, t);
        }

        private static bool ByteArrayToFile(string path, byte[] byteArray)
        {
            try
            {
                using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                fs.Write(byteArray, 0, byteArray.Length);
                return true;
            }
            catch 
            {
                return false;
            }
        }

        private static string ImageToSrc(byte[] img)
        {
            return Constants.Base64 + Convert.ToBase64String(img);
        }

        private static byte[] FullSizeToByte(string designPic)
        {
            var index = designPic.IndexOf(";base64,", System.StringComparison.Ordinal);
            designPic = designPic.Substring(index);
            designPic = designPic.Replace(";base64,", string.Empty);
            return Convert.FromBase64String(designPic);
        }

        private static byte[] ReadToEnd(MemoryStream stream)
        {
            var originalPosition = stream.Position;
            stream.Position = 0;
            try
            {
                var readBuffer = new byte[4096];
                var totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;
                    if (totalBytesRead != readBuffer.Length) continue;
                    var nextByte = stream.ReadByte();
                    if (nextByte == -1) continue;
                    var temp = new byte[readBuffer.Length * 2];
                    Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                    Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                    readBuffer = temp;
                    totalBytesRead++;
                }
                var buffer = readBuffer;
                if (readBuffer.Length == totalBytesRead) return buffer;
                buffer = new byte[totalBytesRead];
                Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                return buffer;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }
    }
}
