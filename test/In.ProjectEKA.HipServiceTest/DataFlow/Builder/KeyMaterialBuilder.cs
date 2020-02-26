namespace In.ProjectEKA.HipServiceTest.DataFlow.Builder
{
    using HipService.DataFlow;

    public class KeyMaterialBuilder
    {
        private string CryptoAlg;
        private string Curve;
        private KeyStructure DhPublicKey;
        private string RandomKey;
        
        public KeyMaterial Build()
        {
            return new KeyMaterial(CryptoAlg, Curve, DhPublicKey, RandomKey);
        }
    }
}