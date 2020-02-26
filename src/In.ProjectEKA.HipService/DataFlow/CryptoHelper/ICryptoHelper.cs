namespace In.ProjectEKA.HipService.DataFlow.CryptoHelper
{
    using Org.BouncyCastle.Crypto;

    public interface ICryptoHelper
    {
        AsymmetricCipherKeyPair GenerateKeyPair(string curveName, string algorithm);
        string GetPublicKey(AsymmetricCipherKeyPair senderKeyPair);
        string EncryptData(string receiverPublicKey, AsymmetricCipherKeyPair senderKeyPair, string content,
            string randomKeySender, string randomKeyReceiver, string curveName, string algorithm);
        string GenerateRandomKey();
    }
}