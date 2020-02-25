namespace In.ProjectEKA.HipService.DataFlow.CryptoHelper
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using Logger;
    using Newtonsoft.Json;
    using Org.BouncyCastle.Asn1.X9;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Security;
    using Encoder = Org.BouncyCastle.Utilities.Encoders;

    public static class CryptoHelper
    {
        public static AsymmetricCipherKeyPair GenerateKeyPair()
        {
            var keyGenerationParameters = new KeyGenerationParameters(new SecureRandom(), 192);
            var generator = (ECKeyPairGenerator)GeneratorUtilities.GetKeyPairGenerator("ECDH");
            generator.Init(keyGenerationParameters);
            return generator.GenerateKeyPair();
        }

        public static string GetPublicKey(AsymmetricCipherKeyPair senderKeyPair)
        {
            return Convert.ToBase64String(Org.BouncyCastle.X509.SubjectPublicKeyInfoFactory
                .CreateSubjectPublicKeyInfo(senderKeyPair.Public).GetEncoded());
        }
        
        public static string EncryptData(string receiverPublicKey,
            AsymmetricCipherKeyPair senderKeyPair,  string content, string randomKeySender, string randomKeyReceiver)
        {
            var receiverKeyBytes= GetByteFromBase64(receiverPublicKey);
            var sharedKey = GetBase64FromByte(GetDeriveKey((byte[])receiverKeyBytes,senderKeyPair));
            var sharedSecret = GetSha256Hash(sharedKey + randomKeySender + randomKeyReceiver);
            return Encrypt(sharedSecret, content);
        }
        
        private static IEnumerable<byte> GetDeriveKey(byte[] key1, AsymmetricCipherKeyPair senderKeyPair)
        {
            var ecP = ECNamedCurveTable.GetByName("prime192v1");
            var ecSpec = new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H, ecP.GetSeed());
            var publicKey = new ECPublicKeyParameters(ecSpec.Curve.DecodePoint(key1), ecSpec);

            var agreement = AgreementUtilities.GetBasicAgreement("ECDH");
            agreement.Init(senderKeyPair.Private);
            var result = agreement.CalculateAgreement(publicKey);
            return result.ToByteArrayUnsigned();
        }

        private static string Encrypt(string sharedKey, string dataToEncrypt)
        {
            var json = JsonConvert.SerializeObject(dataToEncrypt);
            try
            {
                return AesBase64Wrapper.EncryptAndEncode(json, sharedKey);
            }
            catch (Exception e)
            {
                Log.Error("Error Occured while encryption {exception}: ",e);
                throw;
            }
        }
        
        public static string GetBase64FromByte(IEnumerable<byte> value)
        {
            return Encoder.Base64.ToBase64String((byte [])value);
        }

        public static IEnumerable<byte> GetByteFromBase64(string value)
        {
            return Encoder.Base64.Decode(value);
        }

        public static string GenerateRandomKey()
        {
            var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[32];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return GetBase64FromByte(randomBytes);   
        }
        
        private static string GetSha256Hash(string input)
        {
            using var sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }  
    }
}