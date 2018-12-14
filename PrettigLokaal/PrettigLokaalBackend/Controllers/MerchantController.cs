using System;
using System.Collections.Generic;
using System.Linq;
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
    public class MerchantController : APIControllerBase
    {
        public MerchantController(IConfiguration config, PrettigLokaalContext context) : base(context, config)
        {

        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            Merchant merchant = await context.Merchants.Where(m => m.Id == id)
                .Include(m => m.Events).ThenInclude(e => e.Image)
                .Include(m => m.Promotions).ThenInclude(e => e.Image)
                .Include(m => m.Tags)
                .Include(m => m.OpeningHours)
                .Include(m => m.Images)
                .FirstOrDefaultAsync();

            if (merchant == null)
                return Error(ErrorModel.NOT_FOUND);

            return Ok(merchant);
        }

        [HttpGet("MyAccount")]
        public async Task<IActionResult> GetMyMerchantData()
        {
            Account acc = await GetAccount();
            Merchant merchant = await context.Merchants
                .Include(m => m.Events).ThenInclude(e => e.Image)
                .Include(m => m.Promotions).ThenInclude(e => e.Image)
                .Include(m => m.Tags)
                .Include(m => m.OpeningHours)
                .Include(m => m.Images)
                .Where(m => m.Account.Id == acc.Id)
                .FirstOrDefaultAsync();

            if (merchant == null)
                return Error(ErrorModel.NOT_FOUND);

            return Ok(merchant);
        }

        [HttpGet("Find/{query}")]
        [AllowAnonymous]
        public async Task<IActionResult> Find(string query)
        {
            string[] keywords = query.Split(new char[] { ' ', '\t', '\n', '\r' });

            List<Merchant> merchants = await context.Merchants
                .Include(m => m.Tags)
                .Include(m => m.OpeningHours)
                .Include(m => m.Images)
                .Include(m => m.Events)
                    .ThenInclude(e => e.Image)
                .Include(m => m.Promotions)
                    .ThenInclude(p => p.Image)
                .Where(m => keywords.Count(kw => m.Name.Contains(kw) || m.Address.Contains(kw) || 
                                           m.Description.Contains(kw) || (m.Tags.Count(tag => tag.Text.Contains(kw)) > 0)) > 0)
                .ToListAsync();

            return Ok(merchants);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]MerchantRegisterModel model)
        {
            Account acc = await GetAccount();
            if (acc.Merchant != null)
                return Error(ErrorModel.ALREADY_A_MERCHANT);

            acc.Merchant = new Merchant()
            {
                Name = model.Name,
                ContactEmail = model.ContactEmail,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                Description = model.Description,
                FacebookPage = model.FacebookPage,
                Account = acc,
                Tags = new List<Tag>(),
                OpeningHours = new List<OpeningHourSpan>()
            };

            foreach (var str in model.Tags)
                acc.Merchant.Tags.Add(new Tag() { Text = str });

            var modelOpeningHours = model.GetOpeningHours();
            foreach (var timeSpan in modelOpeningHours)
                acc.Merchant.OpeningHours.Add(timeSpan);

            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("UpdateDetails")]
        public async Task<IActionResult> UpdateDetails([FromBody]MerchantRegisterModel model)
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            acc.Merchant.Name = model.Name;
            acc.Merchant.ContactEmail = model.ContactEmail;
            acc.Merchant.Address = model.Address;
            acc.Merchant.PhoneNumber = model.PhoneNumber;
            acc.Merchant.Description = model.Description;
            acc.Merchant.FacebookPage = model.FacebookPage;

            List<Tag> toRemove = new List<Tag>();
            foreach(var curTag in acc.Merchant.Tags) // Check which tags to remove.
            {
                if(!model.Tags.Contains(curTag.Text))
                    toRemove.Add(curTag);
            }
            foreach(var tag in toRemove) // Remove tags
            {
                acc.Merchant.Tags.Remove(tag);
                context.Remove(tag);
            }
            foreach(var tag in model.Tags) // Add new tags
            {
                if (acc.Merchant.Tags.Count(t => t.Text == tag) <= 0)
                    acc.Merchant.Tags.Add(new Tag() { Text = tag });
            }

            var modelOpeningHours = model.GetOpeningHours();
            for (int i = 0;i < modelOpeningHours.Count;i++)
            {
                acc.Merchant.OpeningHours[i].OpenTime = modelOpeningHours[i].OpenTime;
                acc.Merchant.OpeningHours[i].CloseTime = modelOpeningHours[i].CloseTime;
            }

            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("Terminate")]
        public async Task<IActionResult> Terminate() // Terminates the Merchant's page, so the account becomes a regular user account.
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            context.Merchants.Remove(acc.Merchant);
            acc.Merchant = null;

            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("AddImages")]
        public async Task<IActionResult> AddImages([FromBody]List<string> images)
        {
            Account acc = await GetAccount();

            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            Merchant merchant = await context.Merchants
                .Where(m => m.Id == acc.Merchant.Id)
                .Include(m => m.Images)
                .FirstOrDefaultAsync();

            if (merchant == null)
                return Error(ErrorModel.NOT_FOUND);

            if (merchant.Images == null)
                merchant.Images = new List<Image>();
            foreach (string imgData in images)
                merchant.Images.Add(new Image() { Data = new ImageData { Data = imgData } });

            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("RemoveImage/{id}")]
        public async Task<IActionResult> RemoveImage(int id)
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            Image image = await context.Images
                .Include(e => e.Merchant)
                .Where(e => e.Id == id && e.Merchant.Id == acc.Merchant.Id)
                .FirstOrDefaultAsync();

            if (image == null)
                return Error(ErrorModel.NOT_FOUND);

            acc.Merchant.Images.Remove(image);
            context.Remove(image);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("AddPromotion")]
        public async Task<IActionResult> AddPromotion([FromBody]MerchantAddPromotionModel model)
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            if (acc.Merchant.Promotions == null)
                acc.Merchant.Promotions = new List<Promotion>();

            acc.Merchant.Promotions.Add(new Promotion()
            {
                 StartDate = model.StartDate,
                 EndDate = model.EndDate,
                 Name = model.Name,
                 Description = model.Description,
                 HasCouponCode = model.HasCouponCode,
                 Image = model.Image,
                 Organizer = acc.Merchant
            });
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("AddEvent")]
        public async Task<IActionResult> AddEvent([FromBody]MerchantAddEventModel model)
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            if (acc.Merchant.Events == null)
                acc.Merchant.Events = new List<Event>();

            acc.Merchant.Events.Add(new Event()
            {
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Name = model.Name,
                Description = model.Description,
                PlaceDescription = model.PlaceDescription,
                Image = model.Image,
                Organizer = acc.Merchant
            });
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("UpdatePromotion")]
        public async Task<IActionResult> UpdatePromotion([FromBody]MerchantAddPromotionModel model)
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            Promotion promo = await context.Promotions
                .Include(e => e.Organizer)
                .Where(e => e.Id == model.Id && e.Organizer.Id == acc.Merchant.Id)
                .FirstOrDefaultAsync();

            if (promo == null)
                return Error(ErrorModel.NOT_FOUND);

            promo.StartDate = model.StartDate;
            promo.EndDate = model.EndDate;
            promo.Name = model.Name;
            promo.Description = model.Description;
            promo.HasCouponCode = model.HasCouponCode;

            if (model.Image != null)
                promo.Image = model.Image;

            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("UpdateEvent")]
        public async Task<IActionResult> UpdateEvent([FromBody]MerchantAddEventModel model)
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            Event ev = await context.Events
                .Include(e => e.Organizer)
                .Where(e => e.Id == model.Id && e.Organizer.Id == acc.Merchant.Id)
                .FirstOrDefaultAsync();

            if (ev == null)
                return Error(ErrorModel.NOT_FOUND);

            ev.StartDate = model.StartDate;
            ev.EndDate = model.EndDate;
            ev.Name = model.Name;
            ev.Description = model.Description;
            ev.PlaceDescription = model.PlaceDescription;
            
            if (model.Image != null)
                ev.Image = model.Image;

            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("RemovePromotion/{id}")]
        public async Task<IActionResult> RemovePromotion(int id)
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            Promotion promo = await context.Promotions
                .Include(e => e.Organizer)
                .Where(e => e.Id == id && e.Organizer.Id == acc.Merchant.Id)
                .FirstOrDefaultAsync();

            if (promo == null)
                return Error(ErrorModel.NOT_FOUND);

            acc.Merchant.Promotions.Remove(promo);
            context.Remove(promo);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("RemoveEvent/{id}")]
        public async Task<IActionResult> RemoveEvent(int id)
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            Event ev = await context.Events
                .Include(e => e.Organizer)
                .Where(e => e.Id == id && e.Organizer.Id == acc.Merchant.Id)
                .FirstOrDefaultAsync();

            if (ev == null)
                return Error(ErrorModel.NOT_FOUND);

            acc.Merchant.Events.Remove(ev);
            context.Remove(ev);
            await context.SaveChangesAsync();
            return Ok();
        }
       
    }
}