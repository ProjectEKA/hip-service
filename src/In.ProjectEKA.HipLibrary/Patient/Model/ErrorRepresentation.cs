namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class ErrorRepresentation
    {
        public ErrorRepresentation(Error error)
        {
            Error = error;
        }

        public Error Error { get; }
    }
}