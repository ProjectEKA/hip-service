using HipLibrary.Patient.Models;

namespace hip_service.OTP
{
    public class Communication
    {
        public IdentifierType Mode { get; set; }
        public string Value { get; set; }

        public Communication(IdentifierType mode, string value)
        {
            Mode = mode;
            Value = value;
        }
    }
}