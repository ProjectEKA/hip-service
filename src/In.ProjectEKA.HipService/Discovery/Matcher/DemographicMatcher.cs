namespace In.ProjectEKA.HipService.Discovery.Matcher
{
    using System;
    using System.Linq.Expressions;
    using HipLibrary.Matcher;
    using HipLibrary.Patient.Model;

    public class DemographicMatcher
    {
        private static readonly AgeGroupMatcher AgeGroupMatcher = new AgeGroupMatcher(2);

        private DemographicMatcher()
        {
        }

        public static Func<Patient, bool> ExpressionFor(string name, ushort? age, Gender gender)
        {
            return GenderExpresion(gender).And(NameExpression(name)).And(AgeExpression(age)).Compile();
        }

        private static Expression<Func<Patient, bool>> GenderExpresion(Gender gender)
        {
            Expression<Func<Patient, bool>> genderExpresion = patient => patient.Gender == gender;
            return genderExpresion;
        }

        private static Expression<Func<Patient, bool>> NameExpression(string name)
        {
            Expression<Func<Patient, bool>> nameExpression = patient =>
                ExactNameMatcher.IsMatch(patient.Name, name);
            return nameExpression;
        }

        private static Expression<Func<Patient, bool>> AgeExpression(ushort? age)
        {
            return patient => !age.HasValue
                              || AgeGroupMatcher.IsMatching(AgeCalculator.From(age.Value),
                                  AgeCalculator.From(patient.YearOfBirth));
        }
    }
}