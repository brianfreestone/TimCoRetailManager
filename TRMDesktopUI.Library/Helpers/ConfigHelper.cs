using System;
using System.Configuration;

namespace TRMDesktopUI.Library.Helpers
{
    public class ConfigHelper : IConfigHelper
    {
        public decimal GetTaxRate()
        {
            decimal tax;
            string rateText = ConfigurationManager.AppSettings["taxRate"];

            bool isValidTax = decimal.TryParse(rateText, out tax);

            if (!isValidTax)
            {
                throw new ConfigurationErrorsException("The tax rate is not properly set up");
            }

            return tax;
        }
    }
}
