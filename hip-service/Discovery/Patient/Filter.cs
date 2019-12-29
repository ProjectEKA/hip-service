namespace hip_service.Discovery.Patient
{
    using System.Linq;
    using hip_library.Patient.models;
    using System.Threading.Tasks;

    public class Filter
    {
        readonly IMatchingRepository matchingRepository;

        public Filter(IMatchingRepository matchingRepository)
        {
            this.matchingRepository = matchingRepository;
        }

        public async Task<IQueryable<hip_library.Patient.models.Patient>> doFilter(DiscoveryRequest request)
        {
            var expression = StrongMatcherFactory.GetExpression(request.VerifiedIdentifiers);
            return (await matchingRepository.Where(expression))
                    .AsEnumerable()
                    .Select((patient, index) =>
                    {
                        var careContexts = patient.Programs
                                .Select(program => new CareContextRepresentation(
                                    program.ReferenceNumber,
                                    program.Description))
                                .ToList();

                        return new hip_library.Patient.models.Patient(
                            patient.Identifier,
                            patient.FirstName + " " + patient.LastName,
                            careContexts, new[] { "" });
                    }).AsQueryable();
        }
    }
}