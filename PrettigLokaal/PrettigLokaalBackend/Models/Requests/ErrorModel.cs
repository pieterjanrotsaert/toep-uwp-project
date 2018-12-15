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
                default: return "Ongekende fout";
            }
        }
    }
}
