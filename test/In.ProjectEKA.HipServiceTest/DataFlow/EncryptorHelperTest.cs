namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System.Linq;
    using FluentAssertions;
    using HipService.DataFlow.Encryptor;
    using Xunit;

    public class EncryptorHelperTest
    {
        [Fact]
        private void ShouldReturn34ByteRandomKey()
        {
            var randomKey = EncryptorHelper.GetByteFromBase64(EncryptorHelper.GenerateRandomKey());

            randomKey.Count().Should().Be(32);
        }

        [Fact]
        private void ShouldGenerate32ByteXor()
        {
            var randomKey1 = EncryptorHelper.GenerateRandomKey();
            var randomKey2 = EncryptorHelper.GenerateRandomKey();
            var xorRandom = EncryptorHelper.XorOfRandom(randomKey1,
                randomKey2);

            xorRandom.Count().Should().Be(32);
        }

        [Fact]
        private void ShouldGenerateGenerateKeyPair()
        {
            var keyPair = EncryptorHelper.GenerateKeyPair("curve25519", "ECDH");

            keyPair.Should().NotBeNull();
        }

        [Fact]
        private void ShouldGenerateBase64String()
        {
            const string testString = "dGhpcyBpcyBhIHN0cmluZwo=";
            var testStringByte = EncryptorHelper.GetByteFromBase64(testString);

            EncryptorHelper.GetBase64FromByte(testStringByte).Should().Be(testString);
        }
    }
}