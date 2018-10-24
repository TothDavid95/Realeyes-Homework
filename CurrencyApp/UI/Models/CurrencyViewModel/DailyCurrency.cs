using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Models.CurrencyViewModel
{
    public class DailyCurrency
    {
        DateTime date;
        Dictionary<string, double> currency;

        public DailyCurrency(DateTime date, Dictionary<string, double> currency)
        {
            this.date = date;
            this.currency = currency;
            currency.Add("EUR", 1.0);
        }
    }
}