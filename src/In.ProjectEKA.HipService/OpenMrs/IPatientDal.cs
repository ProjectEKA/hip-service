using System.Collections.Generic;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    public interface IPatientDal
    {
        System.Threading.Tasks.Task<List<Hl7.Fhir.Model.Patient>> LoadPatientsAsync(string name, Gender? gender, string yearOfBirth);
    }
}