namespace In.ProjectEKA.HipService.DataFlow.Encryptor
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.EC;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Security;
    using Encoder = Org.BouncyCastle.Utilities.Encoders;

    public static class EncryptorHelper
    {
        public static string GenerateRandomKey()
        {
            var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[32];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return GetBase64FromByte(randomBytes);   
        }
        
        public static string GetPublicKey(AsymmetricCipherKeyPair senderKeyPair)
        {
            return Convert.ToBase64String(Org.BouncyCastle.X509.SubjectPublicKeyInfoFactory
                .CreateSubjectPublicKeyInfo(senderKeyPair.Public).GetEncoded());
        }
        
        public static AsymmetricCipherKeyPair GenerateKeyPair(string curveName, string algorithm)
        {
            var ecP = CustomNamedCurves.GetByName(curveName);
            var ecSpec = new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H, ecP.GetSeed());
            var generator = (ECKeyPairGenerator)GeneratorUtilities.GetKeyPairGenerator(algorithm);
            generator.Init(new ECKeyGenerationParameters(ecSpec, new SecureRandom()));
            return generator.GenerateKeyPair();
        }

        public static string GetBase64FromByte(IEnumerable<byte> value)
        {
            return Encoder.Base64.ToBase64String((byte [])value);
        }

        public static IEnumerable<byte> GetByteFromBase64(string value)
        {
            return Encoder.Base64.Decode(value);
        }
    }
}