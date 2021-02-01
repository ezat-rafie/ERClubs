using System;
using System.Text.RegularExpressions;

namespace ERClassLibrary
{
    public static class ERStringManipulation
    {
        /// <summary>
        /// Takes a string and extracts the digits
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string ERExtractDigits(string inputString)
        {
            string extractedDigit = "";
            if (String.IsNullOrEmpty(inputString))
            {
                return inputString = "";
            }
            else
            {
                foreach (char item in inputString)
                {
                    if (Char.IsDigit(item))
                    {
                        extractedDigit += item;
                    }
                }
                return extractedDigit;
            }
        }
        /// <summary>
        /// Takes postal code and checks it against its regex
        /// </summary>
        /// <param name="postalCode"></param>
        /// <param name="postalCodeRegex"></param>
        /// <returns></returns>
        public static Boolean ERPostalCodeIsValid(string postalCode, string postalCodeRegex)
        {
            Regex postal = new Regex($@"^{postalCodeRegex}$");
            if (String.IsNullOrEmpty(postalCode))
            {
                return true;
            }
            else
            {
                if (postal.IsMatch(postalCode))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Takes a string and capitalizes it
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string ERCapitalize(string inputString)
        {
            if (String.IsNullOrEmpty(inputString))
            {
                return inputString = "";
            }
            else
            {
                inputString = inputString.Trim().ToLower();
                string newString = char.ToUpper(inputString[0]) + inputString.Substring(1);
                return newString;
            }
        }
    }
}
