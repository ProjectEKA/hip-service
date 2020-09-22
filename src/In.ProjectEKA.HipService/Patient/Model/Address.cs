namespace In.ProjectEKA.HipService.Patient.Model
{
    public class Address
    {
        public string Line { get; }
        public string District { get; }
        public string State { get; }
        public string Pincode { get; }

        public Address(string line, string district, string state, string pincode)
        {
            Line = line;
            District = district;
            State = state;
            Pincode = pincode;
        }
    }
}