namespace In.ProjectEKA.HipLibrary.Patient
{
    public interface IMaskUtility
    {
        string MaskReference(string referenceNumber);
        string MaskCareContextDisplay(string referenceNumber);
        string UnmaskData(string maskedData);
        string MaskMobileNumber(string mobileNumber);

    }
}