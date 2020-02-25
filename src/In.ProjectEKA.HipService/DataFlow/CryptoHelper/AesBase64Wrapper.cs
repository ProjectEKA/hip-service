namespace In.ProjectEKA.HipService.DataFlow.CryptoHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public static class AesBase64Wrapper
    {
        public static string EncryptAndEncode(string raw, string password)
        {
            using var csp = new AesCryptoServiceProvider();
            var iv = GenerateRandomKey(16);
            var salt = GenerateRandomKey(8);
            var e = GetCryptoTransform(csp, true, password, iv, salt);
            var inputBuffer = Encoding.UTF8.GetBytes(raw);
            var output = e.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            var encryptedData = output.Concat((byte [])iv).Concat((byte [])salt);
            var encrypted = Convert.ToBase64String(encryptedData.ToArray());
            return encrypted;
        }

        private static ICryptoTransform GetCryptoTransform(AesCryptoServiceProvider csp,
            bool encrypting,  string password, IEnumerable<byte> iv, IEnumerable<byte> salt)
        {
            csp.Mode = CipherMode.CBC;
            csp.Padding = PaddingMode.PKCS7;
            var spec = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), salt.ToArray(), 65536);
            var key = spec.GetBytes(16);
            csp.IV = iv.ToArray();
            csp.Key = key;
            return encrypting ? csp.CreateEncryptor() : csp.CreateDecryptor();
        }
        
        private static IEnumerable<byte> GenerateRandomKey(int size)
        {
            var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[size];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return randomBytes;   
        }
    }
}