namespace In.ProjectEKA.HipService.DataFlow.CryptoHelper
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class AesBase64Wrapper
    {
        private static readonly string Iv = "IV_VALUE_16_BYTE";
        private static readonly string Salt = "SALT_VALUE";
 
        public static string EncryptAndEncode(string raw, string password)
        {
            using var csp = new AesCryptoServiceProvider();
            var e = GetCryptoTransform(csp, true, password);
            var inputBuffer = Encoding.UTF8.GetBytes(raw);
            var output = e.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            var encrypted = Convert.ToBase64String(output);
            return encrypted;
        }

        private static ICryptoTransform GetCryptoTransform(AesCryptoServiceProvider csp,
            bool encrypting,  string password)
        {
            csp.Mode = CipherMode.CBC;
            csp.Padding = PaddingMode.PKCS7;
            var spec = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(Salt), 65536);
            byte[] key = spec.GetBytes(16);
            csp.IV = Encoding.UTF8.GetBytes(Iv);
            csp.Key = key;
            return encrypting ? csp.CreateEncryptor() : csp.CreateDecryptor();
        }
    }
}