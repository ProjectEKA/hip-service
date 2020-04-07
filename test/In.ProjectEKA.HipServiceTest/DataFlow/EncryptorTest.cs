namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System.Collections.Generic;
    using System.Linq;
    using Bogus;
    using FluentAssertions;
    using HipLibrary.Patient.Model;
    using HipService.DataFlow.Encryptor;
    using Moq;
    using Xunit;
    using Encoder = Org.BouncyCastle.Utilities.Encoders;

    public class EncryptorTest
    {
        private readonly string curve = "curve25519";
        private readonly string algorithm = "ECDH";
        private readonly Encryptor encryptor = new Encryptor();

        [Fact]
        private void ShouldGenerateKeyPair()
        {
            var keyPair = EncryptorHelper.GenerateKeyPair(curve, algorithm);
            keyPair.Should().NotBeNull();
        }

        [Fact]
        private void ShouldReturnPublicKey()
        {
            var keyPair = EncryptorHelper.GenerateKeyPair(curve, algorithm);
            var publicKey = EncryptorHelper.GetPublicKey(keyPair);
            publicKey.Should().NotBeNull();
        }

        [Fact]
        private void ShouldGenerate32ByteRandomKey()
        {
            var randomKey = EncryptorHelper.GenerateRandomKey();
            var randomKeyByte = GetByteFromBase64(randomKey);

            randomKeyByte.Count().Should().Be(32);
        }

        [Fact]
        private void ShouldEncryptData()
        {
            var randomKeyFirst = EncryptorHelper.GenerateRandomKey();
            var randomKeySecond = EncryptorHelper.GenerateRandomKey();
            var senderKeyPair = EncryptorHelper.GenerateKeyPair(curve, algorithm);
            var receiverPublicKey =
                "BE7rHCAB0xdU/wTh4PeZhnmLfPSS2CQQo3Loi2D3sWiGBwm9lwsxudMgTyjZ0KUzEqUniuW7zmDr3Vy9JeYI7gA=";
            var keyMaterial = new KeyMaterial(algorithm, curve,
                new KeyStructure(It.IsAny<string>(), It.IsAny<string>(), receiverPublicKey), randomKeyFirst);

            var result = encryptor.EncryptData(keyMaterial, senderKeyPair, new Faker().Random.Word(),
                randomKeySecond);

            result.HasValue.Should().BeTrue();
            result.MatchSome(data => data.Length.Should().BePositive());
        }

        [Fact]
        private void ShouldNotEncryptData()
        {
            var randomKeyFirst = EncryptorHelper.GenerateRandomKey();
            var randomKeySecond = EncryptorHelper.GenerateRandomKey();
            var senderKeyPair = EncryptorHelper.GenerateKeyPair(curve, algorithm);
            const string receiverPublicKey =
                "BE7rHCAB0xdU/wTh4PeZhnmLfPSS2CQQo3Loi2D3sWiGBwm9lwsxudMgTyjZ0KUzEqUniuW7zmDr3Vy9JeYI7gA=";
            var keyMaterial = new KeyMaterial(algorithm, curve, new KeyStructure("", "",
                receiverPublicKey), randomKeyFirst);
            var result = encryptor.EncryptData(keyMaterial, senderKeyPair, null,
                randomKeySecond);
            result.HasValue.Should().BeFalse();
            result.MatchSome(data => data.Length.Should().Be(0));
        }

        private static IEnumerable<byte> GetByteFromBase64(string value)
        {
            return Encoder.Base64.Decode(value);
        }
    }
}