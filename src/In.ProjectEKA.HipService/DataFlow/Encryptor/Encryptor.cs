namespace In.ProjectEKA.HipService.DataFlow.Encryptor
{
    using System;
    using System.Linq;
    using System.Text;
    using HipLibrary.Patient.Model;
    using Logger;
    using Optional;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Engines;
    using Org.BouncyCastle.Crypto.Modes;
    using Org.BouncyCastle.Crypto.Parameters;
    using Encoder = Org.BouncyCastle.Utilities.Encoders;

    public class Encryptor : IEncryptor
    {
        public virtual Option<string> EncryptData(KeyMaterial receivedKeyMaterial,
            AsymmetricCipherKeyPair senderKeyPair,
            string content, string randomKeySender)
        {
            var receiverKeyBytes = EncryptorHelper.GetByteFromBase64(receivedKeyMaterial.DhPublicKey.KeyValue);
            var sharedKey = EncryptorHelper.GetBase64FromByte(EncryptorHelper.GetDeriveKey((byte[]) receiverKeyBytes,
                senderKeyPair,
                receivedKeyMaterial.Curve,
                receivedKeyMaterial.CryptoAlg));
            var encryptedContent = Encrypt(sharedKey, content, randomKeySender,
                receivedKeyMaterial.Nonce);
            return encryptedContent == string.Empty ? Option.None<string>() : Option.Some(encryptedContent);
        }

        private static string Encrypt(string sharedKey, string dataToEncrypt,
            string randomKeySender, string randomKeyReceiver)
        {
            var xorOfRandoms = EncryptorHelper.XorOfRandom(randomKeySender, randomKeyReceiver).ToArray();
            var salt = xorOfRandoms.Take(20);
            var iv = xorOfRandoms.TakeLast(12);
            var aesKey = EncryptorHelper.GenerateAesKey(sharedKey, salt);
            try
            {
                return EncryptDataUseAesGcm(dataToEncrypt, aesKey.ToArray(), iv.ToArray());
            }
            catch (Exception e)
            {
                Log.Error("Error Occured while encryption {exception}: ", e);
                return "";
            }
        }

        private static string EncryptDataUseAesGcm(string data, byte[] key, byte[] iv)
        {
            var encryptedString = string.Empty;
            try
            {
                var dataBytes = Encoding.UTF8.GetBytes(data);
                var cipher = new GcmBlockCipher(new AesEngine());
                var parameters =
                    new AeadParameters(new KeyParameter(key), 128, iv, null);
                cipher.Init(true, parameters);
                var encryptedBytes = new byte[cipher.GetOutputSize(dataBytes.Length)];
                var returnLengthEncryptedData = cipher.ProcessBytes
                    (dataBytes, 0, dataBytes.Length, encryptedBytes, 0);
                cipher.DoFinal(encryptedBytes, returnLengthEncryptedData);
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