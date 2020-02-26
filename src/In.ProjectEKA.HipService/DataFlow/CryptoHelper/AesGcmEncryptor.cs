namespace In.ProjectEKA.HipService.DataFlow.CryptoHelper
{
    using System;
    using System.Text;
    using Logger;
    using Org.BouncyCastle.Crypto.Engines;
    using Org.BouncyCastle.Crypto.Modes;
    using Org.BouncyCastle.Crypto.Parameters;
    
    public static class AesGcmEncryptor
    {
        public static string EncryptDataUseAesGcm(string data, byte[] key, byte[] iv)
        {
            var encryptedString = string.Empty;
            try
            {
                var plainBytes = Encoding.UTF8.GetBytes(data);
                var cipher = new GcmBlockCipher(new AesEngine());
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
                Log.Error(ex.Message);
                Log.Fatal(ex);
            }
            return encryptedString;
        }
    }
}
