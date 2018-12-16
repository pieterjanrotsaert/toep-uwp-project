using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Models.Requests
{
    public class ErrorModel
    {
        public int ErrorCode { get; set; }
        public string Detail { get; set; } = "";

        public ErrorModel(){}
        public ErrorModel(int code, string detail = "")
        {
            ErrorCode = code;
            Detail = detail;
        }

        public string GetDescription()
        {
            string str = GetString() + " (code " + ErrorCode + ")";
            if (!string.IsNullOrEmpty(Detail))
                str += ": " + Detail;
            return str;
        }

        public string GetString()
        {
            return GetErrorString(ErrorCode);
        }


        public const int NOERROR                = 0;
        public const int EMAIL_ALREADY_IN_USE   = 1;
        public const int INVALID_PASSWORD       = 2;
        public const int INVALID_USERNAME       = 3;

        public const int ALREADY_A_MERCHANT     = 100;
        public const int NOT_A_MERCHANT         = 101;
        public const int ALREADY_SUBSCRIBED = 102;

        public const int NOT_FOUND              = 404;

        public const int HTTP_ERROR             = 500;
        public const int NOT_LOGGED_IN          = 501;
        public const int NETWORK_ERROR = 502;

        public static string GetErrorString(int code)
        {
            switch (code)
            {
                case EMAIL_ALREADY_IN_USE: return "E-Mailadres is reeds in gebruik.";
                case INVALID_PASSWORD: return "Het opgegeven wachtwoord is onjuist.";
                case INVALID_USERNAME: return "De opgegeven gebruikersnaam is onjuist.";
                case ALREADY_A_MERCHANT: return "U bent reeds een handelaar.";
                case NOT_A_MERCHANT: return "U bent geen handelaar.";
                case ALREADY_SUBSCRIBED: return "U volgt deze handelaar reeds.";
                case NOT_FOUND: return "De opgrevraagde gegevens konden niet gevonden worden";
                case HTTP_ERROR: return "HTTP Fout";
                case NOT_LOGGED_IN: return "U bent niet ingelogd.";
                case NETWORK_ERROR: return "Netwerkfout";

                default: return "Ongekende fout";
            }
        }
    }
}
