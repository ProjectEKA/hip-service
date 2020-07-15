using System.Collections.Generic;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    public interface IPatientDal
    {
        List<Hl7.Fhir.Model.Patient> LoadPatients(string name, Gender? gender, string yearOfBirth);
    }
}