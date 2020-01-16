using hip_service.Link.Patient;
using HipLibrary.Patient.Model;

namespace hip_service.OTP
{
    public class Communication
    {
        private readonly CommunicationMode Mode;
        private readonly string Value;

        public Communication(CommunicationMode mode, string value)
        {
            Mode = mode;
            Value = value;
        }
    }
}