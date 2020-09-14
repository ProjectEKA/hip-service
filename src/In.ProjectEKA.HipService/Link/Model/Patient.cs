namespace In.ProjectEKA.HipService.Link.Model
{
    public class Patient
    {
        public Patient(string id, string name,Gender gender,int yearOfBirth,Address address)
        {
            Id = id;
            Name = name;
            Gender = gender;
            YearOfBirth = yearOfBirth;
            Address = address;
        }

        public string Id { get; }
        public string Name { get; }
        public Gender Gender { get; }
        public int YearOfBirth { get; }
        public Address Address { get; }
    }
}