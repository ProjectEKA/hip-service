using System;
using System.Linq;
using System.Threading.Tasks;
using hip_service.Database;
using hip_service.Link.Patient.Models;
using Microsoft.EntityFrameworkCore;
using Optional;
using static Optional.Option;

namespace hip_service.Link.Patient
{
    public class LinkPatientRepository: ILinkPatientRepository
    {
        private readonly DatabaseContext databaseContext;
        public LinkPatientRepository(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        
        public async Task<Tuple<LinkRequest, Exception>> SaveRequestWith(string linkReferenceNumber, string consentManagerId, string consentManagerUserId,
            string patientReferenceNumber, string[] careContextReferenceNumbers)
        {
            var dateTimeStamp = DateTime.Now.ToUniversalTime().ToString(Constants.DateTimeFormat);
            var linkedCareContexts = careContextReferenceNumbers.Select(referenceNumber => new LinkedCareContext(referenceNumber)).ToList();
            var linkRequest = new LinkRequest(patientReferenceNumber, linkReferenceNumber, consentManagerId,
                consentManagerUserId, dateTimeStamp, linkedCareContexts);
            try
            {
                databaseContext.LinkRequest.Add(linkRequest);
                await databaseContext.SaveChangesAsync();
                return new Tuple<LinkRequest, Exception>(linkRequest,null);
            }
            catch (Exception exception)
            {
                return new Tuple<LinkRequest, Exception>(null, exception);
            }
        }

        public async Task<Tuple<LinkRequest, Exception>>GetPatientFor(string linkReferenceNumber)
        {
            try
            {
                var linkRequest = await databaseContext.LinkRequest.Include("CareContexts")
                    .FirstAsync(request => request.LinkReferenceNumber == linkReferenceNumber);
                return new Tuple<LinkRequest, Exception>(linkRequest, null);

            }
            catch (Exception exception)
            {
                return new Tuple<LinkRequest, Exception>(null, exception);
            }
        }
    }
}