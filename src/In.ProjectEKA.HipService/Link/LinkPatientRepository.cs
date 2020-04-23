namespace In.ProjectEKA.HipService.Link
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Database;
    using Database.Migrations;
    using Logger;
    using Microsoft.EntityFrameworkCore;
    using Model;
    using Optional;

    public class LinkPatientRepository : ILinkPatientRepository
    {
        private readonly LinkPatientContext linkPatientContext;

        public LinkPatientRepository(LinkPatientContext linkPatientContext)
        {
            this.linkPatientContext = linkPatientContext;
        }


        public async Task<Tuple<LinkEnquires, Exception>> SaveRequestWith(
            string linkReferenceNumber,
            string consentManagerId,
            string consentManagerUserId,
            string patientReferenceNumber,
            IEnumerable<string> careContextReferenceNumbers)
        {
            var dateTimeStamp = DateTime.Now.ToUniversalTime().ToString(Constants.DateTimeFormat);
            var linkedCareContexts = careContextReferenceNumbers
                .Select(referenceNumber => new CareContext(referenceNumber)).ToList();
            var linkRequest = new LinkEnquires(
                patientReferenceNumber,
                linkReferenceNumber,
                consentManagerId,
                consentManagerUserId,
                dateTimeStamp,
                linkedCareContexts);
            try
            {
                linkPatientContext.LinkEnquires.Add(linkRequest);
                await linkPatientContext.SaveChangesAsync();
                return new Tuple<LinkEnquires, Exception>(linkRequest, null);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, exception.StackTrace);
                return new Tuple<LinkEnquires, Exception>(null, exception);
            }
        }

        public async Task<Tuple<LinkEnquires, Exception>> GetPatientFor(string linkReferenceNumber)
        {
            try
            {
                var linkRequest = await linkPatientContext.LinkEnquires.Include("CareContexts")
                    .FirstAsync(request => request.LinkReferenceNumber == linkReferenceNumber);
                return new Tuple<LinkEnquires, Exception>(linkRequest, null);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, exception.StackTrace);
                return new Tuple<LinkEnquires, Exception>(null, exception);
            }
        }

        public async Task<Option<LinkedAccounts>> Save(string consentManagerUserId,
            string patientReferenceNumber,
            string linkReferenceNumber,
            IEnumerable<string> careContextReferenceNumbers)
        {
            var linkedAccounts = new LinkedAccounts(patientReferenceNumber,
                linkReferenceNumber,
                consentManagerUserId,
                DateTime.Now.ToUniversalTime().ToString(Constants.DateTimeFormat),
                careContextReferenceNumbers.ToList());
            try
            {
                linkPatientContext.LinkedAccounts.Add(linkedAccounts);
                await linkPatientContext.SaveChangesAsync();
                return Option.Some(linkedAccounts);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, exception.StackTrace);
                return Option.None<LinkedAccounts>();
            }
        }

        public async Task<Tuple<IEnumerable<LinkedAccounts>, Exception>> GetLinkedCareContexts(
            string consentManagerUserId)
        {
            try
            {
                var linkRequest = await linkPatientContext.LinkedAccounts
                    .Where(request => request.ConsentManagerUserId.Equals(consentManagerUserId)).ToListAsync();
                return new Tuple<IEnumerable<LinkedAccounts>, Exception>(linkRequest, null);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, exception.StackTrace);
                return new Tuple<IEnumerable<LinkedAccounts>, Exception>(null, exception);
            }
        }

        public async Task<Option<InitiatedLinkRequest>> Save(string requestId, string transactionId,
            string linkReferenceNumber)
        {
            try
            {
                var initiatedLinkRequest = new InitiatedLinkRequest(requestId,
                    transactionId,
                    linkReferenceNumber,
                    false,
                    DateTime.Now.ToUniversalTime().ToString(Constants.DateTimeFormat));
                linkPatientContext.InitiatedLinkRequest.Add(initiatedLinkRequest);
                await linkPatientContext.SaveChangesAsync();
                return Option.Some(initiatedLinkRequest);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, exception.StackTrace);
                return Option.None<InitiatedLinkRequest>();
            }
        }

        public async Task<Option<IEnumerable<InitiatedLinkRequest>>> Get(string linkReferenceNumber)
        {
            try
            {
                var initiatedLinkRequest = await linkPatientContext.InitiatedLinkRequest
                    .Where(request => request.LinkReferenceNumber.Equals(linkReferenceNumber)).ToListAsync();
                return Option.Some(initiatedLinkRequest.AsEnumerable());
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, exception.StackTrace);
                return Option.None<IEnumerable<InitiatedLinkRequest>>();
            }
        }

        public async Task<bool> Update(InitiatedLinkRequest linkRequest)
        {
            try
            {
                linkPatientContext.InitiatedLinkRequest.Update(linkRequest);
                return true;
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, exception.StackTrace);
                return false;
            }
        }
    }
}