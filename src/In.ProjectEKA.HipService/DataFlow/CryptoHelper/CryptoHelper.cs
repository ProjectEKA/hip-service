namespace In.ProjectEKA.HipService.DataFlow.CryptoHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using Logger;
    using Newtonsoft.Json;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Digests;
    using Org.BouncyCastle.Crypto.EC;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Security;
    using Encoder = Org.BouncyCastle.Utilities.Encoders;

    public class CryptoHelper: ICryptoHelper
    {
        public AsymmetricCipherKeyPair GenerateKeyPair(string curveName, string algorithm)
        {
            var ecP = CustomNamedCurves.GetByName(curveName);
            var ecSpec = new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H, ecP.GetSeed());
            var generator = (ECKeyPairGenerator)GeneratorUtilities.GetKeyPairGenerator(algorithm);
            generator.Init(new ECKeyGenerationParameters(ecSpec, new SecureRandom()));
            return generator.GenerateKeyPair();
        }

        public string GetPublicKey(AsymmetricCipherKeyPair senderKeyPair)
        {
            return Convert.ToBase64String(Org.BouncyCastle.X509.SubjectPublicKeyInfoFactory
                .CreateSubjectPublicKeyInfo(senderKeyPair.Public).GetEncoded());
        }
        
        public string EncryptData(string receiverPublicKey, AsymmetricCipherKeyPair senderKeyPair,
            string content, string randomKeySender, string randomKeyReceiver, string curveName, string algorithm)
        {
            try
            {
                var receiverKeyBytes= GetByteFromBase64(receiverPublicKey);
                var sharedKey = GetBase64FromByte(GetDeriveKey((byte[])receiverKeyBytes, senderKeyPair,
                    curveName, algorithm));
                return Encrypt(sharedKey, content, randomKeySender, randomKeyReceiver);
            }
            catch (Exception e)
            {
                Log.Fatal(e);
                return "";
            }
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

        private static string Encrypt(string sharedKey, string dataToEncrypt,
            string randomKeySender, string randomKeyReceiver)
        {
            var json = JsonConvert.SerializeObject(dataToEncrypt);
            var xorOfRandoms = XorOfRandom(randomKeySender, randomKeyReceiver).ToArray();
            var salt = xorOfRandoms.Take(20);
            var iv = xorOfRandoms.TakeLast(12);
            var hkdfBytesGenerator = new HkdfBytesGenerator(new Sha256Digest());
            var hkdfParameters = new HkdfParameters(GetByteFromBase64(sharedKey).ToArray(),
                salt.ToArray(),
                null);
            hkdfBytesGenerator.Init(hkdfParameters);
            var aesKey = new byte[32];
            hkdfBytesGenerator.GenerateBytes(aesKey, 0, 32);
            try
            {
                return AesGcmEncryptor.EncryptDataUseAesGcm(json, aesKey, iv.ToArray());
            }
            catch (Exception e)
            {
                Log.Error("Error Occured while encryption {exception}: ",e);
                return "";
            }
        }
        
        private static string GetBase64FromByte(IEnumerable<byte> value)
        {
            return Encoder.Base64.ToBase64String((byte [])value);
        }

        private static IEnumerable<byte> GetByteFromBase64(string value)
        {
            return Encoder.Base64.Decode(value);
        }

        public string GenerateRandomKey()
        {
            var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[32];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return GetBase64FromByte(randomBytes);   
        }

        private static IEnumerable<byte> XorOfRandom(string randomKeySender, string randomKeyReceiver)
        {
            var randomKeySenderBytes = GetByteFromBase64(randomKeySender).ToArray();
            var randomKeyReceiverBytes = GetByteFromBase64(randomKeyReceiver).ToArray();
            var sb = new byte[randomKeyReceiverBytes.Length];
            for (var i = 0; i < randomKeySenderBytes.Length; i++)
                sb[i] = (byte)(randomKeySenderBytes[i] ^ randomKeyReceiverBytes[i % randomKeyReceiverBytes.Length]);
            return sb;
        }
    }
}