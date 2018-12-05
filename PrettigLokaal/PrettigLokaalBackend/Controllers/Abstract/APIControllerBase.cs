using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PrettigLokaalBackend.Data;
using PrettigLokaalBackend.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Controllers.Extensions
{
    public class APIControllerBase : ControllerBase
    {
        protected readonly PrettigLokaalDataContext _context;
        protected readonly IConfiguration _config;

        public APIControllerBase(PrettigLokaalDataContext context, IConfiguration configuration) : base()
        {
            _context = context;
            _config = configuration;
        }

        protected async Task<Account> GetUserByEmail(string email)
        {
            return await _context.Accounts.Where(a => a.Email.Equals(email)).Include(a => a.Merchant).FirstOrDefaultAsync();
        }

        protected async Task<Account> GetAccountById(int id)
        {
            return await _context.Accounts.Where(a => a.Id == id).Include(a => a.Merchant).FirstOrDefaultAsync();
        }

        protected int GetAccountId()
        {
            return int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
        }

        protected async Task<Account> GetAccount()
        {
            return await GetAccountById(GetAccountId());
        }
    }
}
