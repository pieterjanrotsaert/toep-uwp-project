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

        [HttpGet("feed")]
        public async Task<IActionResult> GetFeed()
        {
            var accId = GetAccountId();
            FeedModel model = new FeedModel();

            model.FollowedMerchants = await context.Merchants
                .Where(m => m.Subscriptions.Any(sub => sub.Account.Id == accId))
                .Include(m => m.Images)
                .Include(m => m.Tags)
                .Include(m => m.Events)
                .Include(m => m.Promotions)
                .Include(m => m.OpeningHours)
                .ToListAsync();

            model.Events = await context.Events
                .Where(ev => ev.StartDate.CompareTo(DateTime.Now) <= 0 && 
                        ev.EndDate.CompareTo(DateTime.Now) >= 0 && model.FollowedMerchants.Contains(ev.Organizer))
                .Include(ev => ev.Image)
                .ToListAsync();

            model.Promotions = await context.Promotions
                .Where(ev => ev.StartDate.CompareTo(DateTime.Now) <= 0 &&
                        ev.EndDate.CompareTo(DateTime.Now) >= 0 && model.FollowedMerchants.Contains(ev.Organizer))
                .Include(ev => ev.Image)
                .ToListAsync();

            model.FeaturedMerchants = new List<Merchant>();
            foreach(var merchant in model.FollowedMerchants)
            {
                foreach(var ev in merchant.Events)
                {
                    if(model.Events.Contains(ev))
                    {
                        model.FeaturedMerchants.Add(merchant);
                        break;
                    }
                }

                if (!model.FeaturedMerchants.Contains(merchant))
                    foreach (var prom in merchant.Promotions)
                    {
                        if (model.Promotions.Contains(prom))
                        {
                            model.FeaturedMerchants.Add(merchant);
                            break;
                        }
                    }
            }

            return Ok(model);
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