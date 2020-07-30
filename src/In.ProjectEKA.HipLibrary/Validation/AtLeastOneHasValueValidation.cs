using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace In.ProjectEKA.HipLibrary.Validation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AtLeastOneHasValueValidationAttribute : ValidationAttribute
    {
        private string[] PropertyList { get; set; }

        public AtLeastOneHasValueValidationAttribute(params string[] propertyList)
        {
            this.PropertyList = propertyList;
        }

        //See http://stackoverflow.com/a/1365669
        public override object TypeId
        {
            get
            {
                return this;
            }
        }

        public override bool IsValid(object value)
        {
            if(value != null)
            {  
                PropertyInfo propertyInfo;
                foreach (string propertyName in PropertyList)
                {
                    propertyInfo = value.GetType().GetProperty(propertyName);

                    if (propertyInfo != null && propertyInfo.GetValue(value, null) != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
