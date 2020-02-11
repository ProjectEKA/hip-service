using System;

namespace In.ProjectEKA.TMHHip.Discovery
{
    public class Patient
    {
        public string Identifier { get; set; }
        
        public string Gender { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        // ReSharper disable once UnusedMember.Global
        public DateTime DateOfBirth { get; set; }
    }
}