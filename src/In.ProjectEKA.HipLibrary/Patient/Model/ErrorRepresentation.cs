namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class ErrorRepresentation
    {
        public Error Error { get; }

        public ErrorRepresentation(Error error)
        {
            Error = error;
        }
    }
}