namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class PatientLinkConfirmationRepresentation
    {
        public LinkConfirmationRepresentation Patient { get; }

        public PatientLinkConfirmationRepresentation(LinkConfirmationRepresentation patient)
        {
            Patient = patient;
        }
    }
}