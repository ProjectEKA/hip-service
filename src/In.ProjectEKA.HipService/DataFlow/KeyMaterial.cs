namespace In.ProjectEKA.HipService.DataFlow
{
    public class KeyMaterial
    {
        public KeyMaterial(string cryptoAlg, string curve, KeyStructure dhPublicKey, string nonce)
        {
            CryptoAlg = cryptoAlg;
            Curve = curve;
            DhPublicKey = dhPublicKey;
            Nonce = nonce;
        }

        public string CryptoAlg { get; }
        public string Curve { get; }
        public KeyStructure DhPublicKey { get; }
        public string Nonce { get; }
    }
}