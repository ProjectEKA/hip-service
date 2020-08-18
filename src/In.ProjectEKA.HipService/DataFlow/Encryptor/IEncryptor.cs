namespace In.ProjectEKA.HipService.DataFlow.Encryptor
{
    using HipLibrary.Patient.Model;
    using Optional;
    using Org.BouncyCastle.Crypto;

    public interface IEncryptor
    {
        Option<string> EncryptData(KeyMaterial receivedKeyMaterial,
            AsymmetricCipherKeyPair senderKeyPair,
            string content,
            string randomKeySender);
    }
}