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
        private readonly LinkPatientContext _linkPatientContext;
        public LinkPatientRepository(LinkPatientContext linkPatientContext)
        {
            _linkPatientContext = linkPatientContext;
        }
        
        public async Task<Tuple<LinkRequest, Exception>> SaveLinkPatientDetails(string linkReferenceNumber, string consentManagerId, string consentManagerUserId,
            string patientReferenceNumber, string[] careContextReferenceNumbers)
        {
            var dateTimeStamp = new DateTime (DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 
                    DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second).ToUniversalTime().
                ToString("yyyy-MM-ddTHH:mm:ssZ");
            var linkedCareContexts = careContextReferenceNumbers.Select(referenceNumber => new LinkedCareContext(referenceNumber)).ToList();
            var linkRequest = new LinkRequest(patientReferenceNumber, linkReferenceNumber, consentManagerId,
                consentManagerUserId, dateTimeStamp, linkedCareContexts);

            _linkPatientContext.LinkRequest.Add(linkRequest);
            try
            {
                await _linkPatientContext.SaveChangesAsync();
                return new Tuple<LinkRequest, Exception>(linkRequest, null);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new Tuple<LinkRequest, Exception>(null, exception);
                
            }
        }
    }
}