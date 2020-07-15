namespace In.ProjectEKA.HipServiceTest.Discovery
{
    using System.Linq;
    using System.Threading.Tasks;
    using Hangfire.MemoryStorage.Database;
    using In.ProjectEKA.DefaultHip.Patient;
    using In.ProjectEKA.HipLibrary.Matcher;
    using In.ProjectEKA.HipLibrary.Patient.Model;
    using In.ProjectEKA.HipServiceTest.OpenMrs;
    using static HipLibrary.Matcher.StrongMatcherFactory;
    using Moq;
    using Xunit;
    using System.Collections;
    using System.Collections.Generic;

    [Collection("Patient Matching Repository Tests")]
    public class PatientMatchingRepositoryTest
    {
        [Fact]
        public void ShouldQueryTheRightDataSource()
        {
            var dataSource = new Mock<OpenMrsClient>(MockBehavior.Strict);
            var repository = new PatientMatchingRepository(dataSource.Object);
        }
    }

    public class PatientMatchingRepository: IMatchingRepository
    {
        private readonly OpenMrsClient dataSource;

        public PatientMatchingRepository(OpenMrsClient dataSource)
        {
            this.dataSource = dataSource;
        }

        public async Task<IQueryable<Patient>> Where(DiscoveryRequest request)
        {
            var expression = GetVerifiedExpression(request.Patient.VerifiedIdentifiers);
            Task<IEnumerable<Patient>> patientsInfo = await FileReader.ReadJsonAsync("");
            return patientsInfo.Where(expression.Compile()).AsQueryable();
        }
    }
}