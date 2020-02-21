namespace In.ProjectEKA.HipService.DataFlow
{
    public class KeyMaterial
    {
        public string CryptoAlg { get; }
        public string Curve { get; }
        public KeyStructure DhPublicKey { get; }
        public KeyStructure RandomKey { get; }

        public KeyMaterial(string cryptoAlg, string curve, KeyStructure dhPublicKey, KeyStructure randomKey)
        {
            CryptoAlg = cryptoAlg;
            Curve = curve;
            DhPublicKey = dhPublicKey;
            RandomKey = randomKey;
        }
    }
}