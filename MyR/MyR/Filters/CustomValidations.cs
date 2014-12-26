using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyR.Filters
{
    public class EmailAttribute:  RegularExpressionAttribute
    {
        public EmailAttribute() : 
            base(@"^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$") { }
    }

    public class PhoneAttribute : RegularExpressionAttribute
    {
        public PhoneAttribute() :
            base(@"((\(\d{3}\))|(\d{3}-))\d{3}-\d{4}") { }
 
    }

    public class PasswordAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string val = (string)value;
            bool validLength = val.Length >= Constants.SystemConstants.PasswordLength;
            bool hasNumber = false;
            bool hasLetter = false;
            for (int i = 0; i < val.Length; i++)
            {
                if (hasNumber && hasLetter)
                    break;

                int ascii = (int)val[i];
                if (!hasNumber && ascii >= 48 && ascii <= 57)
                    hasNumber = true;

                if (!hasLetter && ((ascii >= 65 && ascii <= 90) || (ascii >= 97 && ascii <= 122)))
                    hasLetter = true;
            }
            return validLength && hasLetter && hasNumber;
        }
    }

    public class MaxValueAttribute : ValidationAttribute
    {
        private readonly int _maxValue;
        public MaxValueAttribute(int maxValue)
        {
            _maxValue = maxValue;
        }

        public override bool IsValid(object value)
        {
            return (int)value <= _maxValue;
        }
    }

    public class MinValueAttribute : ValidationAttribute
    {
        private readonly int _minValue;
        public MinValueAttribute(int minValue)
        {
            _minValue = minValue;
        }

        public override bool IsValid(object value)
        {
            return (int)value >= _minValue;
        }
    }

    public class NotEqualValueAttribute : ValidationAttribute
    {
        private readonly int _value;
        public NotEqualValueAttribute(int value)
        {
            _value = value;
        }

        public override bool IsValid(object value)
        {
            return (int)value != _value;
        }
    }
}