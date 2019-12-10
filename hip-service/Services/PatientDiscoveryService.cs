using System.Collections.Generic;
using System.IO;
using hip_library.Patient;
using hip_library.Patient.models.domain;
using hip_library.Patient.models.dto;
using hip_service.Models.dto;
using Newtonsoft.Json;

namespace hip_service.Services
{
    public class PatientDiscoveryService: IDiscovery
    {
        private readonly string _patientsFilePath;

        public PatientDiscoveryService(string patientsFilePath)
        {
            _patientsFilePath = patientsFilePath;
        }

        public List<Patient> GetPatients(PatientRequest request)
        {
            var jsonData = File.ReadAllText(_patientsFilePath);
            var patients = JsonConvert.DeserializeObject<List<PatientInfo>>(jsonData);

            var filteredPatients = new List<Patient>();
            foreach (var patient in patients)
            {
                if (patient.PhoneNumber == request.PhoneNumber || patient.FirstName == request.FirstName || patient.LastName == request.LastName || patient.CaseId == request.CaseId)
                {
                    filteredPatients.Add(new Patient(
                        "patient", 
                        patient.FirstName + " " + patient.LastName, 
                        patient.Gender, 
                        patient.BirthDate,
                        new Address("home", patient.Address.Line, patient.Address.City, patient.Address.District, patient.Address.State, patient.Address.PostalCode),
                        new Contact(patient.Contact.Name, new ContactPoint("home", "https://tmc.gov.in/ncg/telecom", patient.Contact.PhoneNumber))));
                }
            }
            return filteredPatients;
        }
    }
}