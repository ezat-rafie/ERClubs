using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ERClassLibrary;

namespace ERClubs.Models
{
    [ModelMetadataType(typeof(NameAddressMetadata))]
    public partial class NameAddress : IValidatableObject
    {
        [Display(Name = "Name")]
        public string FullName { get { return LastName + " " + FirstName; } }
        private ERClubsContext _context;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //Use your XXStringManipulation.Capitalize method to capitalise FirstName, LastName, CompanyName, StreetAddress, and City.
            if (String.IsNullOrEmpty(FirstName))
                FirstName = "";
            else
                FirstName = ERStringManipulation.ERCapitalize(FirstName);

            if (String.IsNullOrEmpty(LastName))
                LastName = "";
            else
                LastName = ERStringManipulation.ERCapitalize(LastName);

            if (String.IsNullOrEmpty(CompanyName))
                CompanyName = "";
            else
                CompanyName = ERStringManipulation.ERCapitalize(CompanyName);

            if (String.IsNullOrEmpty(StreetAddress))
                StreetAddress = "";
            else
                StreetAddress = ERStringManipulation.ERCapitalize(StreetAddress);

            if (String.IsNullOrEmpty(City))
                City = "";
            else
                City = ERStringManipulation.ERCapitalize(City);

            //Use your XXStringManipulation.XXExtractDigits to reduce phone to just digits
            Phone = ERStringManipulation.ERExtractDigits(Phone);
            if(String.IsNullOrEmpty(Phone) || Phone.Length != 10)
            {
                yield return new ValidationResult("Phone is required", new[] { nameof(Phone) });
            }
            else
            {
                string newPhone = Phone.Substring(0, 3) + "-" + Phone.Substring(3, 3) + "-" + Phone.Substring(6, 4);
                Phone = newPhone;
            }
            

            //At least one of FirstName, LastName or CompanyName must be specified.  All can be specified, but is not mandatory.
            if (FirstName == "" && LastName == "" && CompanyName == "")
            {
                yield return new ValidationResult("Either first name, last name or company is reuired", new[] { nameof(FirstName), nameof(LastName), nameof(CompanyName) });
            }

            //Email Validations
            if (String.IsNullOrEmpty(Email) && String.IsNullOrEmpty(PostalCode) && String.IsNullOrEmpty(ProvinceCode)
                && String.IsNullOrEmpty(StreetAddress) && String.IsNullOrEmpty(City))
            {
                yield return new ValidationResult("Either Email or Postal addressing is required", new[] { nameof(Email), nameof(PostalCode), nameof(ProvinceCode),
                nameof(StreetAddress), nameof(City)});
            }

            //Province code validation
            _context = new ERClubsContext();
            string foundProvince = "";
            string provinceError = "";
            if (!String.IsNullOrEmpty(ProvinceCode))
            {
                try
                {
                    var provinceCode = _context.Province.ToList();
                    foundProvince = provinceCode.FirstOrDefault(x => x.ProvinceCode == ProvinceCode).ToString();
                    if (String.IsNullOrEmpty(foundProvince))
                    {
                        //Provinc code wasnt found
                        provinceError = new ValidationResult("Province was not found", new[] { nameof(ProvinceCode) }).ToString();
                    }
                }
                catch (Exception ex)
                {
                    //Error fetching province code
                    provinceError = new ValidationResult("Error finding the province", new[] { ex.Message.ToString() }).ToString();
                }
                if (provinceError != "")
                {
                    yield return new ValidationResult(provinceError);
                }
            }

            string postalError = "";
            //Postal code validation
            if (!String.IsNullOrEmpty(PostalCode))
            {
                //If postal provided check for province
                if (String.IsNullOrEmpty(ProvinceCode))
                {
                    yield return new ValidationResult("Province Code is is required", new[] { nameof(ProvinceCode) });
                }
                //Fetches the country and its postal pattern
                try
                {
                    // var foundProvinceCode = _context.Province.ToList();
                    var foundProvinceCode = _context.Province.FirstOrDefault(x => x.ProvinceCode == ProvinceCode);
                    var countryCode = _context.Country.FirstOrDefault(x => x.CountryCode == foundProvinceCode.CountryCode.ToString());

                    string postalRegex = countryCode.PostalPattern;

                    //Changes everything to upp case
                    PostalCode = PostalCode.ToUpper();
                    //Checks if the postal is from canada
                    if (countryCode.CountryCode == "CA")
                    {
                        string firstPostal = foundProvinceCode.FirstPostalLetter;
                        if (!firstPostal.Contains(PostalCode[0]))
                        {
                            postalError = new ValidationResult("First letter of postal doesnt match the province", new[] { nameof(PostalCode) }).ToString();
                        }
                        else
                        {
                            //If no space was there adds one
                            if (PostalCode.Length == 6)
                            {
                                PostalCode = PostalCode.Insert(3, " ");
                            }
                        }
                    }

                    //Validates the postal pattern
                    if (!ERStringManipulation.ERPostalCodeIsValid(PostalCode, postalRegex))
                    {
                        postalError = new ValidationResult("Postal Code is not in correct format", new[] { nameof(PostalCode) }).ToString();
                    }
                }
                catch (Exception ex)
                {
                    postalError = new ValidationResult("Error finding the country", new[] { ex.Message.ToString() }).ToString();
                }
                if (postalError != "")
                {
                    yield return new ValidationResult(postalError); 
                }
            }
            yield return ValidationResult.Success;
        }
    }


    public class NameAddressMetadata
    {
        [Display(Name = "ID")]
        public int NameAddressId { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }
        public string City { get; set; }
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [Display(Name = "Province Code")]
        public string ProvinceCode { get; set; }

        [EREmailAnnotation]
        public string Email { get; set; }
        public string Phone { get; set; }

        public virtual Province ProvinceCodeNavigation { get; set; }
        public virtual Club Club { get; set; }
        public virtual ICollection<Artist> Artist { get; set; }
    }
}