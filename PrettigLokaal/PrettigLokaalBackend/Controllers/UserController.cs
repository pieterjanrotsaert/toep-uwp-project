using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PrettigLokaalBackend.Controllers.Extensions;
using PrettigLokaalBackend.Data;
using PrettigLokaalBackend.Models.Domain;
using PrettigLokaalBackend.Models.Requests;

namespace PrettigLokaalBackend.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserController : APIControllerBase
    {
        public UserController(IConfiguration config, PrettigLokaalContext context) : base(context, config)
        {

        }

        [HttpGet("checksubscription/{id}")]
        public async Task<IActionResult> CheckSubscription(int id)
        {
            var accId = GetAccountId();
            var subscription = await context.Subscriptions.Where(sub => sub.Account.Id == accId && sub.Merchant.Id == id).FirstOrDefaultAsync();
           

            if (subscription == null)
                return Ok(new SimpleBoolModel(false));

            return Ok(new SimpleBoolModel(true));
        }

        [HttpPost("subscribe/{id}")]
        public async Task<IActionResult> Subscribe(int id)
        {
            var account = await GetAccount();
            var subscription = await context.Subscriptions
                .Where(sub => sub.Account.Id == account.Id && sub.Merchant.Id == id).FirstOrDefaultAsync();
            if (subscription != null)
                return Error(ErrorModel.ALREADY_SUBSCRIBED);

            Merchant merchant = await context.Merchants.Where(m => m.Id == id)
                .Include(m => m.Subscriptions).FirstOrDefaultAsync();
            if (merchant == null)
                return Error(ErrorModel.NOT_FOUND);

            subscription = new MerchantSubscription()
            {
                Account = account,
                Merchant = merchant
            };

            if (account.Subscriptions == null)
                account.Subscriptions = new List<MerchantSubscription>();
            if (merchant.Subscriptions == null)
                merchant.Subscriptions = new List<MerchantSubscription>();

            account.Subscriptions.Add(subscription);
            merchant.Subscriptions.Add(subscription);
            context.Subscriptions.Add(subscription);
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("unsubscribe/{id}")]
        public async Task<IActionResult> Unsubscribe(int id)
        {
            var account = await GetAccount();
            var subscription = await context.Subscriptions.Where(sub => sub.Account.Id == account.Id && sub.Merchant.Id == id).FirstOrDefaultAsync();
            if (subscription == null)
                return Error(ErrorModel.NOT_FOUND);

            context.Subscriptions.Remove(subscription);
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}