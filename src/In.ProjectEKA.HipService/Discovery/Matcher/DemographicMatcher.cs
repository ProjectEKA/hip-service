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

        public static Func<Patient, bool> ExpressionFor(string name, ushort? yearOfBirth, Gender? gender)
        {
            if (yearOfBirth == null && gender == null)
            {
                return _ => false;
            }
            return GenderExpression(gender).And(NameExpression(name)).And(AgeExpression(yearOfBirth)).Compile();
        }

        private static Expression<Func<Patient, bool>> GenderExpression(Gender? gender)
        {
            Expression<Func<Patient, bool>> genderExpresion = patient => !gender.HasValue || patient.Gender == gender;
            return genderExpresion;
        }

        private static Expression<Func<Patient, bool>> NameExpression(string name)
        {
            Expression<Func<Patient, bool>> nameExpression = patient =>
                ExactNameMatcher.IsMatch(patient.Name, name);
            return nameExpression;
        }

        private static Expression<Func<Patient, bool>> AgeExpression(ushort? yearOfBirth)
        {
            return patient => !yearOfBirth.HasValue
                              || (patient.YearOfBirth.HasValue
                                    && AgeGroupMatcher.IsMatching(AgeCalculator.From(yearOfBirth.Value),
                                    AgeCalculator.From(patient.YearOfBirth.Value)));
        }
    }
}