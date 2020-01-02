using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hip_library.Patient.models;
using hip_library.Patient.models.dto;
using hip_service.Discovery.Patient.Helpers;
using hip_service.Discovery.Patient.models;
using hip_service.Link.Patient.Models;

namespace hip_service.Link.Patient
{
    public class LinkPatientRepository: ILinkPatientRepository
    {
        private readonly string patientFilePath;
        private readonly LinkPatientContext _linkPatientContext;
        public LinkPatientRepository(LinkPatientContext linkPatientContext)
        {
            _linkPatientContext = linkPatientContext;
            this.patientFilePath = "Resources/patients.json";
        }
        public Task<Tuple<PatientLinkReferenceResponse, Error>> LinkPatient(string consentManagerId, 
            string consentManagerUserId, string patientReferenceNumber, string[] careContextReferenceNumbers)
        {
            var patientsInfo = FileReader.ReadJson(patientFilePath);
            var patients = patientsInfo
                .Where(patient => patient.Identifier == patientReferenceNumber).ToList();
            if (!patients.Any())
            {
                return Task.FromResult(new Tuple<PatientLinkReferenceResponse, Error>
                    (null, new Error(ErrorCode.NoPatientFound, "No patient found")) );
            } else {
                var patient = patients.First();
                IEnumerable<ProgramInfo> programs = new ProgramInfo[]{};
                foreach (var refNumber in careContextReferenceNumbers)
                {
                    programs = patient.Programs
                        .Where(program => program.ReferenceNumber == refNumber);
                }
                bool isEqual = programs.Count().Equals(careContextReferenceNumbers.Length) ? true : false;
                if (!isEqual)
                {
                    return Task.FromResult(new Tuple<PatientLinkReferenceResponse, Error>
                        (null, new Error(ErrorCode.CareContextNotFound, "No Care context found for given patient")));
                }

                // send OTP to mobile
                string otp = "123456";

                String linkRefNumber = Guid.NewGuid().ToString();

                var expiry = new DateTime (DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 
                    DateTime.Now.Hour, DateTime.Now.Minute+1, DateTime.Now.Second).ToUniversalTime().
                    ToString("yyyy-MM-ddTHH:mm:ssZ"); 

                var meta = new LinkReferenceMeta(nameof(LinkReferenceMode.Mobile).ToUpper(), patient.PhoneNumber, expiry);
                var patientLinkReferenceResponse = new PatientLinkReferenceResponse(linkRefNumber, 
                    nameof(AuthenticationType.Mediated).ToUpper(), meta);
                
                
                // Saving the data 
                var dateTimeStamp = new DateTime (DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 
                        DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second).ToUniversalTime().
                    ToString("yyyy-MM-ddTHH:mm:ssZ");
                
                var linkedCareContexts = new List<LinkedCareContext>();

                foreach (var referenceNumber in careContextReferenceNumbers)
                {
                    LinkedCareContext linkedCareContext = new LinkedCareContext(referenceNumber);
                    linkedCareContexts.Add(linkedCareContext);
                }
                
                LinkRequest linkRequest = new LinkRequest(patientReferenceNumber, linkRefNumber, consentManagerId,
                    consentManagerUserId, dateTimeStamp, linkedCareContexts);
                _linkPatientContext.LinkRequest.Add(linkRequest);
                _linkPatientContext.SaveChanges();

                return Task.FromResult(
                    new Tuple<PatientLinkReferenceResponse, Error>(patientLinkReferenceResponse, null));

            }
            
        }
    }
}