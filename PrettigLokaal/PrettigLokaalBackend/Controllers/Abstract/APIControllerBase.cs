using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PrettigLokaalBackend.Data;
using PrettigLokaalBackend.Models.Domain;
using PrettigLokaalBackend.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Controllers.Extensions
{
    // This class contains helper functions that are helpful for every controller.
    public class APIControllerBase : ControllerBase
    {
        protected readonly PrettigLokaalContext context;
        protected readonly IConfiguration config;

        public APIControllerBase(PrettigLokaalContext context, IConfiguration configuration) : base()
        {
            this.context = context;
            config = configuration;
        }

        protected async Task<Account> GetUserByEmail(string email)
        {
            return await context.Accounts.Where(a => a.Email.Equals(email))
                .Include(a => a.Merchant)
                    .ThenInclude(m => m.Tags)
                .Include(a => a.Merchant)
                    .ThenInclude(m => m.OpeningHours)
                .Include(a => a.Merchant)
                    .ThenInclude(m => m.Promotions)
                .Include(a => a.Merchant)
                    .ThenInclude(m => m.Events)
                .FirstOrDefaultAsync();
        }

        protected async Task<Account> GetAccountById(int id)
        {
            return await context.Accounts.Where(a => a.Id == id)
                .Include(a => a.Merchant)
                    .ThenInclude(m => m.Tags)
                .Include(a => a.Merchant)
                    .ThenInclude(m => m.OpeningHours)
                .Include(a => a.Merchant)
                    .ThenInclude(m => m.Promotions)
                .Include(a => a.Merchant)
                    .ThenInclude(m => m.Events)
                .FirstOrDefaultAsync();
        }

        protected int GetAccountId()
        {
            return int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
        }

        protected async Task<Account> GetAccount()
        {
            return await GetAccountById(GetAccountId());
        }

        protected IActionResult Error(int errCode)
        {
            return UnprocessableEntity(new ErrorModel(errCode));
        }
    }
}
