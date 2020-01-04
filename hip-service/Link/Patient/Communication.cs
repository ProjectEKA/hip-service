using hip_library.Patient.models.dto;

namespace hip_service.OTP
{
    public class Communication
    {
        public LinkReferenceMode Mode { get; set; }
        public string Value { get; set; }

        public Communication(LinkReferenceMode mode, string value)
        {
            Mode = mode;
            Value = value;
        }
    }
}