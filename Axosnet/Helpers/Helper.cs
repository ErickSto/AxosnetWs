using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Axosnet.Helpers
{
    public class Helper
    {

    }

    public static class PropertyExtension
    {
        public static void SetPropertyValue(this object obj, string proName, object value) {
            if (obj.GetType().GetProperty(proName) != null)
            {
                obj.GetType().GetProperty(proName).SetValue(obj, value, null);
            }
        }

        public static bool IsValidEmail(this string emailAddress) {
            try
            {
                MailAddress m = new MailAddress(emailAddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}