using hip_service.Link.Patient;
using HipLibrary.Patient.Model;

namespace hip_service.OTP
{
    public class Communication
    {
        public CommunicationMode Mode { get; set; }
        public string Value { get; set; }

        public Communication(CommunicationMode mode, string value)
        {
            Mode = mode;
            Value = value;
        }
    }
}