using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
