
namespace In.ProjectEKA.HipService.Patient.Model
{
    public class PatientDetails
    {
        public string Code { get; }
        public UserDemographics UserDemographics { get; }

        public PatientDetails(string code, UserDemographics userDemo)
        {
            Code = code;
            UserDemographics = userDemo;
        }
    }
}