namespace In.ProjectEKA.HipService.DataFlow.Encryptor
{
    using Optional;
    using Org.BouncyCastle.Crypto;

    public interface IEncryptor
    {
        Option<string> EncryptData(HipLibrary.Patient.Model.KeyMaterial receivedKeyMaterial,
            AsymmetricCipherKeyPair senderKeyPair,
            string content,
            string randomKeySender);
    }
}