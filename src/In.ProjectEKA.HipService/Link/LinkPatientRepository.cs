namespace In.ProjectEKA.HipService.Link
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Database;
    using Logger;
    using Microsoft.EntityFrameworkCore;
    using Model;

    public class LinkPatientRepository : ILinkPatientRepository
    {
        private readonly LinkPatientContext linkPatientContext;

        public LinkPatientRepository(LinkPatientContext linkPatientContext)
        {
            this.linkPatientContext = linkPatientContext;
        }

        public async Task<ValueTuple<LinkRequest, Exception>> SaveRequestWith(
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
                return (linkRequest, null);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, exception.StackTrace);
                return (null, exception);
            }
        }

        public async Task<ValueTuple<LinkRequest, Exception>> GetPatientFor(string linkReferenceNumber)
        {
            try
            {
                var linkRequest = await linkPatientContext.LinkRequest.Include("CareContexts")
                    .FirstAsync(request => request.LinkReferenceNumber == linkReferenceNumber);
                return (linkRequest, null);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, exception.StackTrace);
                return (null, exception);
            }
        }

        public async Task<ValueTuple<IEnumerable<LinkRequest>, Exception>> GetLinkedCareContexts(
            string consentManagerUserId)
        {
            try
            {
                var linkRequest = await linkPatientContext.LinkRequest.Include("CareContexts")
                    .Where(request => request.ConsentManagerUserId.Equals(consentManagerUserId)).ToListAsync();
                return (linkRequest, null);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, exception.StackTrace);
                return (null, exception);
            }
        }
    }
}