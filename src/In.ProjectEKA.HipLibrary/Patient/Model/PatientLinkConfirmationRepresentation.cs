namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class PatientLinkConfirmationRepresentation
    {
        public PatientLinkConfirmationRepresentation(LinkConfirmationRepresentation patient)
        {
            Patient = patient;
        }

        public LinkConfirmationRepresentation Patient { get; }
    }
}