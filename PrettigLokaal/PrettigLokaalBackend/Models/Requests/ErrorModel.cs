using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Models.Requests
{
    public class ErrorModel
    {
        public int ErrorCode { get; set; }

        public ErrorModel()
        {
        }
        public ErrorModel(int code)
        {
            ErrorCode = code;
        }

        public string GetString()
        {
            switch(ErrorCode)
            {
                case EMAIL_ALREADY_IN_USE: return "E-Mailadres is reeds in gebruik.";
                default: return "Ongekende fout.";
            }
        }

        public const int NOERROR = 0;
        public const int EMAIL_ALREADY_IN_USE = 1;

    }
}
