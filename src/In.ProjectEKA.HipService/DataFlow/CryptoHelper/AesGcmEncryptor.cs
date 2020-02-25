namespace In.ProjectEKA.HipService.DataFlow.CryptoHelper
{
    using System;
    using System.Text;
    using Org.BouncyCastle.Crypto.Engines;
    using Org.BouncyCastle.Crypto.Modes;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Security;

    public class AesGcmEncryptor
    {
        private static readonly SecureRandom Random = new SecureRandom();

        // Pre-configured Encryption Parameters
        private const int NonceBitSize = 128;
        private const int KeyBitSize = 256;
        
        public static byte[] NewKey()
        {
            var key = new byte[KeyBitSize / 8];
            Random.NextBytes(key);
            return key;
        }

        public static byte[] NewIv()
        {
            var iv = new byte[NonceBitSize / 8];
            Random.NextBytes(iv);
            return iv;
        }
        
        public static string EncryptAesGcm(string data, byte[] key, byte[] iv)
        {
            var encryptedString = string.Empty;
            try
            {
                var plainBytes = Encoding.UTF8.GetBytes(data);
                var cipher = new GcmBlockCipher(new AesFastEngine());
                var parameters = 
                             new AeadParameters(new KeyParameter(key), 128, iv, null);
                cipher.Init(true, parameters);
                var encryptedBytes = new byte[cipher.GetOutputSize(plainBytes.Length)];
                var retLen = cipher.ProcessBytes
                               (plainBytes, 0, plainBytes.Length, encryptedBytes, 0);
                cipher.DoFinal(encryptedBytes, retLen);
                encryptedString = Convert.ToBase64String(encryptedBytes, Base64FormattingOptions.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            return encryptedString;
        }
    }
}
