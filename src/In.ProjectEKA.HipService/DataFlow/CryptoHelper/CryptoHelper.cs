namespace In.ProjectEKA.HipService.DataFlow.CryptoHelper
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Org.BouncyCastle.Asn1.X9;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Security;
    using Encoder = Org.BouncyCastle.Utilities.Encoders;

    public static class CryptoHelper
    {
        public static string EncryptData(string receiverPublicKey, string content)
        {
            var keyGenerationParameters = new KeyGenerationParameters(new SecureRandom(), 192);
            var generator = (ECKeyPairGenerator)GeneratorUtilities.GetKeyPairGenerator("ECDH");
            generator.Init(keyGenerationParameters);

            var senderKeyPair = generator.GenerateKeyPair();
            
            var senderPublicKey = Convert.ToBase64String(Org.BouncyCastle.X509.SubjectPublicKeyInfoFactory
                .CreateSubjectPublicKeyInfo(senderKeyPair.Public).GetEncoded());
            
            var receiverKeyBytes= GetByteFromBase64(receiverPublicKey);
            var sharedKey = GetBase64FromByte(GetDeriveKey((byte[])receiverKeyBytes,senderKeyPair));
            
            return Encrypt(sharedKey, content);
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
            return AesBase64Wrapper.EncryptAndEncode(json, sharedKey);;
        }
        
        private static string GetBase64FromByte(IEnumerable<byte> value)
        {
            return Encoder.Base64.ToBase64String((byte [])value);
        }

        private static IEnumerable<byte> GetByteFromBase64(string value)
        {
            return Encoder.Base64.Decode(value);
        }
    }
}