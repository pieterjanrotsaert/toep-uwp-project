using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Models.Requests
{
    public class LoginResponse
    {
        public string Token { get; set; }

        public LoginResponse() { }
        public LoginResponse(string token)
        {
            Token = token; 
        }
    }
}
