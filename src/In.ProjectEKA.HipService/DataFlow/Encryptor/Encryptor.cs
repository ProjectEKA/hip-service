namespace In.ProjectEKA.HipService.DataFlow.Encryptor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Logger;
    using Optional;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Digests;
    using Org.BouncyCastle.Crypto.EC;
    using Org.BouncyCastle.Crypto.Engines;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Modes;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Security;
    using Encoder = Org.BouncyCastle.Utilities.Encoders;

    public class Encryptor : IEncryptor
    {
        public Option<string> EncryptData(HipLibrary.Patient.Model.KeyMaterial receivedKeyMaterial, AsymmetricCipherKeyPair senderKeyPair,
            string content, string randomKeySender)
        {
            var receiverKeyBytes= EncryptorHelper.GetByteFromBase64(receivedKeyMaterial.DhPublicKey.KeyValue);
            var sharedKey = EncryptorHelper.GetBase64FromByte(GetDeriveKey((byte[])receiverKeyBytes, senderKeyPair,
                receivedKeyMaterial.Curve, receivedKeyMaterial.CryptoAlg));
            var encryptedContent = Encrypt(sharedKey, content, randomKeySender,
                receivedKeyMaterial.Nonce);
            return encryptedContent == string.Empty ? Option.None<string>() : Option.Some(encryptedContent);
        }

        private static IEnumerable<byte> GetDeriveKey(byte[] key1, AsymmetricCipherKeyPair senderKeyPair,
            string curveName, string algorithm)
        { 
            var ecP = CustomNamedCurves.GetByName(curveName);
            var ecSpec = new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H, ecP.GetSeed());
            var publicKey = new ECPublicKeyParameters(ecSpec.Curve.DecodePoint(key1), ecSpec);
            var agreement = AgreementUtilities.GetBasicAgreement(algorithm);
            agreement.Init(senderKeyPair.Private);
            var result = agreement.CalculateAgreement(publicKey);
            return result.ToByteArrayUnsigned();
        }

        private static IEnumerable<byte> GenerateAesKey(string sharedKey, IEnumerable<byte> salt)
        {
            var hkdfBytesGenerator = new HkdfBytesGenerator(new Sha256Digest());
            var hkdfParameters = new HkdfParameters(EncryptorHelper.GetByteFromBase64(sharedKey).ToArray(),
                salt.ToArray(),
                null);
            hkdfBytesGenerator.Init(hkdfParameters);
            var aesKey = new byte[32];
            hkdfBytesGenerator.GenerateBytes(aesKey, 0, 32);
            return aesKey;
        }
        
        private static string Encrypt(string sharedKey, string dataToEncrypt,
            string randomKeySender, string randomKeyReceiver)
        {
            var xorOfRandoms = XorOfRandom(randomKeySender, randomKeyReceiver).ToArray();
            var salt = xorOfRandoms.Take(20);
            var iv = xorOfRandoms.TakeLast(12);
            var aesKey = GenerateAesKey(sharedKey, salt);
            try
            {
                return EncryptDataUseAesGcm(dataToEncrypt, aesKey.ToArray(), iv.ToArray());
            }
            catch (Exception e)
            {
                Log.Error("Error Occured while encryption {exception}: ",e);
                return "";
            }
        }

        private static IEnumerable<byte> XorOfRandom(string randomKeySender, string randomKeyReceiver)
        {
            var randomKeySenderBytes = EncryptorHelper.GetByteFromBase64(randomKeySender).ToArray();
            var randomKeyReceiverBytes = EncryptorHelper.GetByteFromBase64(randomKeyReceiver).ToArray();
            var sb = new byte[randomKeyReceiverBytes.Length];
            for (var i = 0; i < randomKeySenderBytes.Length; i++)
            {
                sb[i] = (byte)(randomKeySenderBytes[i] ^ randomKeyReceiverBytes[i % randomKeyReceiverBytes.Length]);
            }
            return sb;
        }
        
        private static string EncryptDataUseAesGcm(string data, byte[] key, byte[] iv)
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