/*
 This sample code illustrates below mention feature
    1) Diffie Hillman key exchange
    2) HKDF Aes encryption and decryption
    
 Tech used:
 1) BouncyCastle
 2) C# dotnet 
*/
namespace In.ProjectEKA.HipService.EncryptionBit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Digests;
    using Org.BouncyCastle.Crypto.EC;
    using Org.BouncyCastle.Crypto.Engines;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Modes;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Pkcs;
    using Org.BouncyCastle.Security;
    using Encoder = Org.BouncyCastle.Utilities.Encoders;


    public class EncrytorDemo
    {
        private readonly string CURVE = "curve25519";
        private readonly string ALGORITHM = "ECDH";
        private readonly string StrToPerformActionOn = "SomeValue";
        public void SetUpDemo()
        {
            // Generating key pairs 
            var receiverKeyPair = GenerateKey();
            var receiverPublicKey = GetPublicKey(receiverKeyPair);
            var receiverPrivateKey = GetPrivateKey(receiverKeyPair);
            var senderKeyPair = GenerateKey();
            var senderPublicKey = GetPublicKey(senderKeyPair);
            var senderPrivateKey = GetPrivateKey(senderKeyPair);

            // Generating random key 
            var randomKeySender = GenerateRandomKey();
            var randomKeyReceiver = GenerateRandomKey();
            
            // Generating XOR array for getting the salt and IV used for encryption 
            var xorOfRandoms = XorOfRandom(randomKeySender, randomKeyReceiver).ToArray();

            // Encrypting the string
            var encryptString = EncryptString(StrToPerformActionOn, 
                xorOfRandoms,
                senderPrivateKey,
                receiverPublicKey);
            
            Console.WriteLine("ENCRYPTED STRING: " + encryptString);
            Console.WriteLine("----------------------------------->");
            
            // Decrypting the encrypted value
            var decryptedString = DecryptString(encryptString,
                xorOfRandoms,
                receiverPrivateKey,
                senderPublicKey);
            
            Console.WriteLine("DECRYPTED STRING: " + decryptedString);
        }

        // Method for encrypting the string
        private string EncryptString(string stringToEncrypt,
                                    byte[] xorOfRandoms,
                                    string senderPrivateKey,
                                    string  receiverPublicKey)
        {
            // Generating the shared key using the parameters available
            var sharedKey = GetBase64FromByte(GetSharedSecretValue(senderPrivateKey, receiverPublicKey));
            Console.WriteLine("DHE SHARED SECRET: " + sharedKey);

            // Generate the salt and IV
            var salt = xorOfRandoms.Take(20);
            var iv = xorOfRandoms.TakeLast(12);
            var aesKey = GenerateAesKey(sharedKey, salt);
            Console.WriteLine("HKDF AES KEY: " + GetBase64FromByte(aesKey.ToArray()));

            // Encrypt the data
            var encryptedString = string.Empty;
            try
            {
                var dataBytes = Encoding.UTF8.GetBytes(stringToEncrypt);
                var cipher = new GcmBlockCipher(new AesEngine());
                var parameters =
                    new AeadParameters(new KeyParameter(aesKey.ToArray()), 128, iv.ToArray(), null);
                cipher.Init(true, parameters);
                var encryptedBytes = new byte[cipher.GetOutputSize(dataBytes.Length)];
                var returnLengthEncryptedData = cipher.ProcessBytes
                    (dataBytes, 0, dataBytes.Length, encryptedBytes, 0);
                cipher.DoFinal(encryptedBytes, returnLengthEncryptedData);
                encryptedString = Convert.ToBase64String(encryptedBytes, Base64FormattingOptions.None);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            // Return the encrypted string
            return encryptedString;
        }

        // Method for decrypting the string
        private string DecryptString(string stringToDecrypt,
            byte[] xorOfRandoms,
            string receiverPrivateKey,
            string senderPublicKey)
        {
            // Generating the shared key using the parameters available
            var sharedKey = GetBase64FromByte(GetSharedSecretValue(receiverPrivateKey, senderPublicKey));
            Console.WriteLine("DHE SHARED SECRET: " + sharedKey);
            
            // Generate the salt, IV and aes key
            var salt = xorOfRandoms.Take(20);
            var iv = xorOfRandoms.TakeLast(12);
            var aesKey = GenerateAesKey(sharedKey, salt);
            Console.WriteLine("HKDF AES KEY: " + GetBase64FromByte(aesKey.ToArray()));

            // Decrypting the data
            String decryptedData = "";
            try {
                var dataBytes = GetByteFromBase64(stringToDecrypt).ToArray();
                var cipher = new GcmBlockCipher(new AesEngine());
                var parameters =
                    new AeadParameters(new KeyParameter(aesKey.ToArray()), 128, iv.ToArray(), null);
                cipher.Init(false, parameters);
                byte[] plainBytes = new byte[cipher.GetOutputSize(dataBytes.Length)];
                int retLen = cipher.ProcessBytes
                    (dataBytes, 0, dataBytes.Length, plainBytes, 0);
                cipher.DoFinal(plainBytes, retLen);
                decryptedData = Encoding.UTF8.GetString(plainBytes);
            } catch (Exception ex) {
                Console.Write(ex);
            }
            
            // Returning decrypted data
            return decryptedData;
        }

        // Generating DHE Key
        private AsymmetricCipherKeyPair GenerateKey()
        {
            var ecP = CustomNamedCurves.GetByName(CURVE);
            var ecSpec = new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H, ecP.GetSeed());
            var generator = (ECKeyPairGenerator) GeneratorUtilities.GetKeyPairGenerator(ALGORITHM);
            generator.Init(new ECKeyGenerationParameters(ecSpec, new SecureRandom()));
            return generator.GenerateKeyPair();
        }
        
        // Generating shared key
        private byte[]  GetSharedSecretValue(string privKey, string pubKey)
        {
            var privateKey = GetPrivateKeyFrom(privKey);
            var publicKey = GetPublicKeyFrom(pubKey);
            var agreement = AgreementUtilities.GetBasicAgreement(ALGORITHM);
            agreement.Init(privateKey);
            var result = agreement.CalculateAgreement(publicKey);
            return result.ToByteArrayUnsigned();
        }
        
        // XOR of teo random string (sender and receiver)
        private static IEnumerable<byte> XorOfRandom(string randomKeySender, string randomKeyReceiver)
        {
            var randomKeySenderBytes = GetByteFromBase64(randomKeySender).ToArray();
            var randomKeyReceiverBytes = GetByteFromBase64(randomKeyReceiver).ToArray();
            var sb = new byte[randomKeyReceiverBytes.Length];
            for (var i = 0; i < randomKeySenderBytes.Length; i++)
            {
                sb[i] = (byte) (randomKeySenderBytes[i] ^ randomKeyReceiverBytes[i % randomKeyReceiverBytes.Length]);
            }
        
            return sb;
        }
        
        // Generate 32 byte random Key 
        private static string GenerateRandomKey()
        {
            var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[32];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return GetBase64FromByte(randomBytes);
        }
        
        // Method for getting Aes key using HKDF
        private static IEnumerable<byte> GenerateAesKey(string sharedKey, IEnumerable<byte> salt)
        {
            var hkdfBytesGenerator = new HkdfBytesGenerator(new Sha256Digest());
            var hkdfParameters = new HkdfParameters(GetByteFromBase64(sharedKey).ToArray(),
                salt.ToArray(),
                null);
            hkdfBytesGenerator.Init(hkdfParameters);
            var aesKey = new byte[32];
            hkdfBytesGenerator.GenerateBytes(aesKey, 0, 32);
            return aesKey;
        }
        
        // Get base64 from byte array.
        private static string GetBase64FromByte(IEnumerable<byte> value)
        {
            return Encoder.Base64.ToBase64String((byte[]) value);
        }
        
        //Get byte array from string.
        private static IEnumerable<byte> GetByteFromBase64(string value)
        {
            return Encoder.Base64.Decode(value);
        }
        
        // Converting Public key to string
        public static string GetPublicKey(AsymmetricCipherKeyPair keyPair)
        {
            return Convert.ToBase64String(Org.BouncyCastle.X509.SubjectPublicKeyInfoFactory
                .CreateSubjectPublicKeyInfo(keyPair.Public).GetEncoded());
        }

        // Converting Private key to string
        public string GetPrivateKey(AsymmetricCipherKeyPair keyPair)
        {
            var keyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private);
            var encoded = keyInfo.ToAsn1Object().GetDerEncoded();
            return GetBase64FromByte(encoded);
        }

        // Converting string to privateKey
        public AsymmetricKeyParameter GetPrivateKeyFrom(string privateKey)
        {
            return PrivateKeyFactory.CreateKey((byte[]) GetByteFromBase64(privateKey));
        }

        // Converting string to publicKey
        public AsymmetricKeyParameter GetPublicKeyFrom(string publicKey)
        {
            return PublicKeyFactory.CreateKey((byte[]) GetByteFromBase64(publicKey));
        }
    }
}