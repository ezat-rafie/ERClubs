using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace ERClassLibrary
{
    public class EREmailAnnotation : ValidationAttribute
    {
        public EREmailAnnotation()
        {
            ErrorMessage = "Email is not in correct format";
        }
        /// <summary>
        /// Validates the entered email address
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            else
            {
                try
                {
                    //new System.Net.Mail.MailAddress(value.ToString());
                    MailAddress mailAddress = new MailAddress(value.ToString());
                    return ValidationResult.Success;
                }
                catch (Exception)
                {
                    return new ValidationResult(string.Format(ErrorMessage, validationContext.DisplayName));
                }
            }
        }
    }
}
