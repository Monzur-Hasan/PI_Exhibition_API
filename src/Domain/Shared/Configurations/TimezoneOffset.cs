namespace Domain.Shared.Configurations
{
    public static class TimezoneOffset
    {
        public static string GetCurrencySymbol(string utcOffset)
        {
            if (string.IsNullOrEmpty(utcOffset))
                return "৳";

            string offset = utcOffset.ToUpper().Trim();

            // Add :00 if only hour is provided
            if (!offset.Contains(":") && (offset.StartsWith("+") || offset.StartsWith("-")))
            {
                offset += ":00";
            }

            return offset switch
            {
                "+00:00" => "£",    // UK Pound
                "+01:00" => "€",    // Euro (Western Europe)
                "+02:00" => "€",    // Euro (Eastern Europe)
                "+03:00" => "ر.س", // Saudi Riyal
                "+03:30" => "﷼",   // Iranian Rial
                "+04:00" => "₽",    // Russian Ruble
                "+04:30" => "؋",    // Afghan Afghani
                "+05:00" => "₨",    // Pakistani Rupee
                "+05:30" => "₹",    // Indian Rupee
                "+05:45" => "₨",    // Nepalese Rupee
                "+06:00" => "৳",    // Bangladeshi Taka
                "+06:30" => "K",    // Myanmar Kyat
                "+07:00" => "฿",    // Thai Baht
                "+08:00" => "¥",    // Chinese Yuan
                "+09:00" => "¥",    // Japanese Yen
                "+09:30" => "$",    // Australian Dollar (Central)
                "+10:00" => "$",    // Australian Dollar (Eastern)
                "+12:00" => "$",    // New Zealand Dollar
                "-01:00" => "€",    // Azores Euro
                "-03:00" => "R$",   // Brazilian Real
                "-04:00" => "$",    // US Dollar (Atlantic)
                "-05:00" => "$",    // US Dollar (Eastern Time)
                "-06:00" => "$",    // US Dollar (Central Time)
                "-07:00" => "$",    // US Dollar (Mountain Time)
                "-08:00" => "$",    // US Dollar (Pacific Time)
                "-09:00" => "$",    // US Dollar (Alaska)
                "-10:00" => "$",    // US Dollar (Hawaii)
                "-11:00" => "$",    // US Dollar (Samoa)
                "-12:00" => "$",    // Baker/Howland (uninhabited)

                _ => "৳"            // Default BDT
            };
        }

        public static CountryCurrencyInfo GetCountryInfo(string utcOffset)
        {
            if (string.IsNullOrEmpty(utcOffset))
                return new CountryCurrencyInfo { CountryName = "United States", CountryCode = "US", CurrencyType = "USD", CapitalName = "Washington, D.C." };

            string offset = utcOffset.ToUpper().Trim();

            if (!offset.Contains(":") && (offset.StartsWith("+") || offset.StartsWith("-")))
            {
                offset += ":00";
            }

            return offset switch
            {
                "+00:00" => new CountryCurrencyInfo { CountryName = "United Kingdom", CountryCode = "GB", CurrencyType = "GBP", CapitalName = "London" },
                "+01:00" => new CountryCurrencyInfo { CountryName = "Germany", CountryCode = "DE", CurrencyType = "EUR", CapitalName = "Berlin" },
                "+02:00" => new CountryCurrencyInfo { CountryName = "Greece", CountryCode = "GR", CurrencyType = "EUR", CapitalName = "Athens" },
                "+03:00" => new CountryCurrencyInfo { CountryName = "Saudi Arabia", CountryCode = "SA", CurrencyType = "SAR", CapitalName = "Riyadh" },
                "+03:30" => new CountryCurrencyInfo { CountryName = "Iran", CountryCode = "IR", CurrencyType = "IRR", CapitalName = "Tehran" },
                "+04:00" => new CountryCurrencyInfo { CountryName = "Russia", CountryCode = "RU", CurrencyType = "RUB", CapitalName = "Moscow" },
                "+04:30" => new CountryCurrencyInfo { CountryName = "Afghanistan", CountryCode = "AF", CurrencyType = "AFN", CapitalName = "Kabul" },
                "+05:00" => new CountryCurrencyInfo { CountryName = "Pakistan", CountryCode = "PK", CurrencyType = "PKR", CapitalName = "Islamabad" },
                "+05:30" => new CountryCurrencyInfo { CountryName = "India", CountryCode = "IN", CurrencyType = "INR", CapitalName = "New Delhi" },
                "+05:45" => new CountryCurrencyInfo { CountryName = "Nepal", CountryCode = "NP", CurrencyType = "NPR", CapitalName = "Kathmandu" },
                "+06:00" => new CountryCurrencyInfo { CountryName = "Bangladesh", CountryCode = "BD", CurrencyType = "BDT", CapitalName = "Dhaka" },
                "+06:30" => new CountryCurrencyInfo { CountryName = "Myanmar", CountryCode = "MM", CurrencyType = "MMK", CapitalName = "Naypyidaw" },
                "+07:00" => new CountryCurrencyInfo { CountryName = "Thailand", CountryCode = "TH", CurrencyType = "THB", CapitalName = "Bangkok" },
                "+08:00" => new CountryCurrencyInfo { CountryName = "China", CountryCode = "CN", CurrencyType = "CNY", CapitalName = "Beijing" },
                "+09:00" => new CountryCurrencyInfo { CountryName = "Japan", CountryCode = "JP", CurrencyType = "JPY", CapitalName = "Tokyo" },
                "+09:30" => new CountryCurrencyInfo { CountryName = "Australia (Central)", CountryCode = "AU", CurrencyType = "AUD", CapitalName = "Canberra" },
                "+10:00" => new CountryCurrencyInfo { CountryName = "Australia (Eastern)", CountryCode = "AU", CurrencyType = "AUD", CapitalName = "Canberra" },
                "+12:00" => new CountryCurrencyInfo { CountryName = "New Zealand", CountryCode = "NZ", CurrencyType = "NZD", CapitalName = "Wellington" },

                "-01:00" => new CountryCurrencyInfo { CountryName = "Azores", CountryCode = "PT", CurrencyType = "EUR", CapitalName = "Ponta Delgada" },
                "-03:00" => new CountryCurrencyInfo { CountryName = "Brazil", CountryCode = "BR", CurrencyType = "BRL", CapitalName = "Brasília" },
                "-04:00" => new CountryCurrencyInfo { CountryName = "Canada (Atlantic)", CountryCode = "CA", CurrencyType = "CAD", CapitalName = "Ottawa" },
                "-05:00" => new CountryCurrencyInfo { CountryName = "United States (Eastern)", CountryCode = "US", CurrencyType = "USD", CapitalName = "Washington, D.C." },
                "-06:00" => new CountryCurrencyInfo { CountryName = "United States (Central)", CountryCode = "US", CurrencyType = "USD", CapitalName = "Washington, D.C." },
                "-07:00" => new CountryCurrencyInfo { CountryName = "United States (Mountain)", CountryCode = "US", CurrencyType = "USD", CapitalName = "Washington, D.C." },
                "-08:00" => new CountryCurrencyInfo { CountryName = "United States (Pacific)", CountryCode = "US", CurrencyType = "USD", CapitalName = "Washington, D.C." },
                "-09:00" => new CountryCurrencyInfo { CountryName = "United States (Alaska)", CountryCode = "US", CurrencyType = "USD", CapitalName = "Washington, D.C." },
                "-10:00" => new CountryCurrencyInfo { CountryName = "United States (Hawaii)", CountryCode = "US", CurrencyType = "USD", CapitalName = "Washington, D.C." },
                "-11:00" => new CountryCurrencyInfo { CountryName = "Samoa", CountryCode = "WS", CurrencyType = "WST", CapitalName = "Apia" },
                "-12:00" => new CountryCurrencyInfo { CountryName = "Baker Island", CountryCode = "UM", CurrencyType = "USD", CapitalName = "None" },

                _ => new CountryCurrencyInfo { CountryName = "Unknown", CountryCode = "XX", CurrencyType = "USD", CapitalName = "Unknown" }
            };
        }


        public class CountryCurrencyInfo
        {
            public string CountryName { get; set; }
            public string CountryCode { get; set; }
            public string CurrencyType { get; set; }
            public string CapitalName { get; set; }
        }


    }
}
