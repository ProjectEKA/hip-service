using System.Collections.Generic;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Patient = Hl7.Fhir.Model.Patient;

namespace In.ProjectEKA.HipService.OpenMrs
{
    public interface IPatientDal
    {
        Task<List<Patient>> LoadPatientsAsync(string name, AdministrativeGender? gender, string yearOfBirth);
    }
}