using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyR
{
    public class PasswordGenerator
    {
        public static string GeneratePassword(int length)
        {
            if (length <= 0)
                return string.Empty;

            string characters = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-_!#$%^&*()";
            Random r = new Random();
            string result = string.Empty;
            for (int i = 0; i < length; i++)
            {
                int rndNumber = r.Next(73);
                result += characters[rndNumber];
            }
            return result;
        }
    }
}