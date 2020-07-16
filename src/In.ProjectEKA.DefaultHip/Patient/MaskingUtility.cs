using System.Collections.Generic;
using System.Linq;
using In.ProjectEKA.HipLibrary.Patient;

namespace In.ProjectEKA.DefaultHip.Patient
{
    public class MaskingUtility : IMaskUtility
    {
        private static Dictionary<string,string>  maskedDataMap = new Dictionary<string, string>();
        public string MaskReference(string referenceNumber)
        {
            var maskedPatientReferenceNumber = "";
            if (referenceNumber.Length > 2)
            {
                maskedPatientReferenceNumber = string.Concat(
                    "".PadLeft(referenceNumber.Length - 2, 'X'),
                    referenceNumber.Substring(referenceNumber.Length - 2));
            }
            else {
                maskedPatientReferenceNumber = "XXX" + referenceNumber;
            }

            if (!maskedDataMap.ContainsKey(referenceNumber))
            {
                maskedDataMap.Add(referenceNumber,maskedPatientReferenceNumber);
            }
            return maskedPatientReferenceNumber;
        }
        public string MaskMobileNumber(string mobileNumber)
        {
            var number = mobileNumber;
            var paddingBit = "".PadLeft(mobileNumber.Length - 8, 'X');
            return string.Concat(
                number.Substring(0,6)+
                paddingBit,
                number.Substring(mobileNumber.Length - 2));
        }
        public string MaskCareContextDisplay(string referenceNumber)
        {
            return string.Concat(
                "".PadLeft(referenceNumber.Length - 4, 'X'),
                referenceNumber.Substring(referenceNumber.Length - 2));
        }
        
        public string UnmaskData(string maskedData)
        {
            var keysWithMatchingValues = maskedDataMap.Where(p => p.Value == maskedData).Select(p => p.Key);
            return keysWithMatchingValues.First();
        }

        public string MaskPatientName(string patientName)
        {
            if (patientName.Length >= 5)
            {
                return string.Concat(
                    patientName.Substring(0,2)+
                    "".PadLeft(patientName.Length - 4, 'X'),
                    patientName.Substring(patientName.Length - 2));
            }
            return "XXX"+patientName;
        }
    }
}