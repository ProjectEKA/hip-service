namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using Common.Model;
    using Optional;

    public interface IConsentArtefactRepository
    {
        Tuple<ConsentArtefact, Exception> GetFor(string consentId);
    }
}