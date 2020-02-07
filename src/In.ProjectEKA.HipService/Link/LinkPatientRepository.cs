namespace In.ProjectEKA.HipService.Link
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Database;
    using Microsoft.EntityFrameworkCore;
    using Model;
    using Logger;

    public class LinkPatientRepository : ILinkPatientRepository
    {
        private readonly LinkPatientContext linkPatientContext;

        public LinkPatientRepository(LinkPatientContext linkPatientContext)
        {
            this.linkPatientContext = linkPatientContext;
        }

        public async Task<Tuple<LinkRequest, Exception>> SaveRequestWith(
            string linkReferenceNumber,
            string consentManagerId,
            string consentManagerUserId,
            string patientReferenceNumber,
            IEnumerable<string> careContextReferenceNumbers)
        {
            var dateTimeStamp = DateTime.Now.ToUniversalTime().ToString(Constants.DateTimeFormat);
            var linkedCareContexts = careContextReferenceNumbers
                .Select(referenceNumber => new LinkedCareContext(referenceNumber)).ToList();
            var linkRequest = new LinkRequest(
                patientReferenceNumber,
                linkReferenceNumber,
                consentManagerId,
                consentManagerUserId,
                dateTimeStamp, 
                linkedCareContexts);
            try
            {
                linkPatientContext.LinkRequest.Add(linkRequest);
                await linkPatientContext.SaveChangesAsync();
                return new Tuple<LinkRequest, Exception>(linkRequest, null);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, exception.StackTrace);
                return new Tuple<LinkRequest, Exception>(null, exception);
            }
        }

        public async Task<Tuple<LinkRequest, Exception>> GetPatientFor(string linkReferenceNumber)
        {
            try
            {
                var linkRequest = await linkPatientContext.LinkRequest.Include("CareContexts")
                    .FirstAsync(request => request.LinkReferenceNumber == linkReferenceNumber);
                return new Tuple<LinkRequest, Exception>(linkRequest, null);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, exception.StackTrace);
                return new Tuple<LinkRequest, Exception>(null, exception);
            }
        }
    }
}