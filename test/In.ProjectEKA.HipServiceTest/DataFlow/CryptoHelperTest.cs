namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System.Collections.Generic;
    using System.Linq;
    using Bogus;
    using FluentAssertions;
    using HipService.DataFlow.CryptoHelper;
    using Xunit;
    using Encoder = Org.BouncyCastle.Utilities.Encoders;

    public class CryptoHelperTest
    {
        private readonly CryptoHelper cryptoHelper = new CryptoHelper();
        private readonly string curve = "curve25519";
        private readonly string algorithm = "ECDH";
        
        [Fact]
        private void ShouldGenerateKeyPair()
        {
            var keyPair = cryptoHelper.GenerateKeyPair(curve, algorithm);
            keyPair.Should().NotBeNull();
        }

        [Fact]
        private void ShouldReturnPublicKey()
        {
            var keyPair = cryptoHelper.GenerateKeyPair(curve, algorithm);
            var publicKey = cryptoHelper.GetPublicKey(keyPair);
            publicKey.Should().NotBeNull();
        }

        [Fact]
        private void ShouldGenerate32ByteRandomKey()
        {
            var randomKey = cryptoHelper.GenerateRandomKey();
            var randomKeyByte = GetByteFromBase64(randomKey);

            randomKeyByte.Count().Should().Be(32);
        }

        [Fact]
        private void ShouldEncryptData()
        {
            var randomKeyFirst = cryptoHelper.GenerateRandomKey();
            var randomKeySecond = cryptoHelper.GenerateRandomKey();
            var senderKeyPair = cryptoHelper.GenerateKeyPair(curve, algorithm);
            var receiverPublicKey =
                "BE7rHCAB0xdU/wTh4PeZhnmLfPSS2CQQo3Loi2D3sWiGBwm9lwsxudMgTyjZ0KUzEqUniuW7zmDr3Vy9JeYI7gA=";
            
            var result = cryptoHelper.EncryptData(receiverPublicKey, senderKeyPair, new Faker().Random.Word(), randomKeyFirst,
                randomKeySecond, curve, algorithm);

            result.Length.Should().BePositive().And.Should().NotBeNull();
        }
        
        [Fact]
        private void ShouldNotEncryptData()
        {
            var randomKeyFirst = cryptoHelper.GenerateRandomKey();
            var randomKeySecond = cryptoHelper.GenerateRandomKey();
            var senderKeyPair = cryptoHelper.GenerateKeyPair(curve, algorithm);
            var receiverPublicKey = "";
            
            var result = cryptoHelper.EncryptData(receiverPublicKey, senderKeyPair, new Faker().Random.Word(), randomKeyFirst,
                randomKeySecond, curve, algorithm);

            result.Length.Should().Be(0).And.Should().NotBeNull();
        }

        private static IEnumerable<byte> GetByteFromBase64(string value)
        {
            return Encoder.Base64.Decode(value);
        }
    }
}