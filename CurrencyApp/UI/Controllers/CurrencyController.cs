using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UI.Models.CurrencyViewModel;
using System.Xml;
using System.Text.RegularExpressions;
using UI.Controllers.Helper_methods;

namespace UI.Controllers
{
    public class CurrencyController : Controller
    {
        public ActionResult Exchange()
        {
            return View();
        }

        public ActionResult CurrencyCharts()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetCurrencies()
        {
            List<string> Currencies = new List<string>
            {
                //XML provides us values compared to EUR, therefore we need to add EUR
                "EUR"
            };
            using (XmlReader reader = XmlReader.Create("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist-90d.xml"))
            {
                while (reader.GetAttribute("currency") == null)
                {
                    reader.ReadToFollowing("Cube");
                }

                //While we find siblings at the current level, we find extra currency to add to the counter
                do
                {
                    Currencies.Add(reader.GetAttribute(0));
                } while (reader.ReadToNextSibling("Cube"));
            }
            return Json(Currencies, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public double GetLatestValue(string currency)
        {
            //EUR is not on the list in the XML
            if (String.Compare(currency, "EUR") == 0)
            {
                return 1.0;
            }
            using (XmlReader reader = XmlReader.Create("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist-90d.xml"))
            {
                while (reader.GetAttribute("currency") == null)
                {
                    reader.ReadToFollowing("Cube");
                }
                while (String.Compare(reader.GetAttribute("currency"), currency) != 0)
                {
                    reader.ReadToNextSibling("Cube");
                }
                return Double.Parse(reader.GetAttribute("rate"));
            }
        }

        [HttpGet]
        public int GetOldestDate()
        {
            string oldestDate = DateTime.MaxValue.ToString("yyyy-MM-dd");
            using (XmlReader reader = XmlReader.Create("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist-90d.xml"))
            {
                while (!reader.EOF)
                {
                    if (reader.GetAttribute("time") != null)
                    {
                        if (String.Compare(reader.GetAttribute("time"), oldestDate) == -1)
                        {
                            oldestDate = reader.GetAttribute("time");
                        }
                    }
                }
            }
            return Int32.Parse(oldestDate.Replace("-", ""));
        }

        [HttpGet]
        public JsonResult GetDataOfCurrencies(string currencies)
        {
            Regex splitter = new Regex("(.{3})");
            string[] currency = splitter.Split(currencies).Where(str => str != "").ToArray();
            long[] dates = new long[getDateCount()];
            double[,] rates = new double[currency.Length, dates.Length];
            int i = dates.Length;
            int temp = -1;
            double minRate = 0;
            double maxRate = double.MaxValue;

            for (int k = 0; k < currency.Length; k++)
            {
                if (String.Compare(currency[k], "EUR") == 0)
                {
                    for (int l = 0; l < rates.GetLength(1); l++)
                    {
                        rates[k, l] = 1.0;
                    }
                }
                minRate = 1.0;
                maxRate = 1.0;
            }

            using (XmlReader reader = XmlReader.Create("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist-90d.xml"))
            {
                while (!reader.EOF)
                {
                    if (reader.GetAttribute("time") != null)
                    {
                        --i;
                        dates[i] = reader.GetAttribute("time").ConvertToUnixTime();
                    }
                    if (reader.GetAttribute("currency") != null)
                    {
                        temp = reader.GetAttribute("currency").IncludesAt(currency);
                        if (temp > -1)
                        {
                            rates[temp, i] = 1.0 / Double.Parse(reader.GetAttribute("rate"));
                            if (minRate > rates[temp, i])
                            {
                                minRate = rates[temp, i];
                            }
                            else if (maxRate < rates[temp, i])
                            {
                                maxRate = rates[temp, i];
                            }
                        }
                    }
                    reader.ReadToFollowing("Cube");
                }
            }

            string[] palette =
            {
            "#000000", "#FFFF00", "#1CE6FF", "#FF34FF", "#FF4A46", "#008941", "#006FA6", "#A30059",
            "#FFDBE5", "#7A4900", "#0000A6", "#63FFAC", "#B79762", "#004D43", "#8FB0FF", "#997D87",
            "#5A0007", "#809693", "#FEFFE6", "#1B4400", "#4FC601", "#3B5DFF", "#4A3B53", "#FF2F80",
            "#61615A", "#BA0900", "#6B7900", "#00C2A0", "#FFAA92", "#FF90C9", "#B903AA", "#D16100",
            "#DDEFFF", "#000035", "#7B4F4B", "#A1C299", "#300018", "#0AA6D8", "#013349", "#00846F",
            "#372101", "#FFB500", "#C2FFED", "#A079BF", "#CC0744", "#C0B9B2", "#C2FF99", "#001E09",
            "#00489C", "#6F0062", "#0CBD66", "#EEC3FF", "#456D75", "#B77B68", "#7A87A1", "#788D66",
            "#885578", "#FAD09F", "#FF8A9A", "#D157A0", "#BEC459", "#456648", "#0086ED", "#886F4C",
            "#34362D", "#B4A8BD", "#00A6AA", "#452C2C", "#636375", "#A3C8C9", "#FF913F", "#938A81",
            "#575329", "#00FECF", "#B05B6F", "#8CD0FF", "#3B9700", "#04F757", "#C8A1A1", "#1E6E00",
            "#7900D7", "#A77500", "#6367A9", "#A05837", "#6B002C", "#772600", "#D790FF", "#9B9700",
            "#549E79", "#FFF69F", "#201625", "#72418F", "#BC23FF", "#99ADC0", "#3A2465", "#922329",
            "#5B4534", "#FDE8DC", "#404E55", "#0089A3", "#CB7E98", "#A4E804", "#324E72", "#6A3A4C"
            };

            string[] colours = new string[currency.Length];

            temp = new Random().Next(palette.Length - currency.Length);

            for (int j = 0; j < colours.Length; j++)
            {
                colours[j] = palette[temp + j];
            }

            var data = new
            {
                names = currency,
                x = dates,
                y = rates,
                colour = colours,
                min = minRate * 0.8,
                max = maxRate * 1.2
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }



        private int getDateCount()
        {
            int counter = 0;
            using (XmlReader reader = XmlReader.Create("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist-90d.xml"))
            {
                while (!reader.EOF)
                {
                    if (reader.GetAttribute("time") != null)
                    {
                        ++counter;
                    }
                    reader.ReadToFollowing("Cube");
                }
                return counter;
            }
        }
    }
}