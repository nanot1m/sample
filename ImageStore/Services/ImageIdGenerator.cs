using System;
using System.Security.Cryptography;

namespace Vostok.Sample.ImageStore.Services
{
    public static class ImageIdGenerator
    {
        public static string Generate(byte[] content)
        {
            var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(content);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}