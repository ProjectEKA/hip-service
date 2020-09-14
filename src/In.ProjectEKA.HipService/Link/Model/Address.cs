namespace In.ProjectEKA.HipService.Link.Model
{
    public class Address
    {
        public Address(string line, string district,string state,string pinCode)
        {
            Line = line;
            District = district;
            State = state;
            PinCode = pinCode;
        }
        public string Line { get; }
        public string District { get; }
        public string State { get; }
        public string PinCode { get; }
    }
}