using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.DomainModels
{
    public class Account
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }

        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        public Merchant Merchant { get; set; } // null if the user is not a merchant

        // This sets a new password for the user.
        // The password is not stored directly, rather a hash is computed and stored along with a salt.
        public void SetPassword(string password)
        {
            PasswordSalt = BCrypt.Net.BCrypt.GenerateSalt();
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password + PasswordSalt);
        }

        // Returns true if password matches this account's password, false if it does not.
        public bool ComparePassword(string password)
        {
            if (BCrypt.Net.BCrypt.HashPassword(password + PasswordSalt) == PasswordHash)
                return true;
            return false;
        }
    }
}
