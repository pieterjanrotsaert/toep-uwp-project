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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            Merchant merchant = await _context.Merchants.Where(m => m.Id == id)
                .Include(m => m.Events).ThenInclude(ev => ev.Image)
                .Include(m => m.Promotions).ThenInclude(p => p.Image)
                .Include(m => m.Images)
                .Include(m => m.Tags)
                .Include(m => m.OpeningHours)
                .FirstOrDefaultAsync();

            if (merchant == null)
                return Error(ErrorModel.NOT_FOUND);

            return Ok(merchant);
        }

        [HttpGet("Find")]
        [AllowAnonymous]
        public async Task<IActionResult> Find(string query)
        {
            string[] keywords = query.Split(new char[] { ' ', '\t', '\n', '\r' });

            List<Merchant> merchants = await _context.Merchants
                .Include(m => m.Tags)
                .Include(m => m.OpeningHours)
                .Where(m => keywords.Count(kw => m.Name.Contains(kw) || m.Address.Contains(kw) || 
                                           m.Description.Contains(kw) || (m.Tags.Count(tag => tag.Text.Contains(kw)) > 0)) > 0)
                .ToListAsync();

            return Ok(merchants);
        }

        [HttpGet("MyDetails")]
        public async Task<IActionResult> MyDetails()
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            return await Get(acc.Merchant.Id);
        }

        // Returns events for the currently logged in merchant
        [HttpGet("MyEvents")]
        public async Task<IActionResult> MyEvents()
        {
            int accId = GetAccountId();
            Merchant merchant = await _context.Merchants.Where(m => m.Account.Id == accId)
                .Include(m => m.Events).ThenInclude(ev => ev.Image)
                .FirstOrDefaultAsync();

            if (merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            return Ok(merchant.Events);
        }

        // Returns promotions for the currently logged in merchant
        [HttpGet("MyPromotions")]
        public async Task<IActionResult> MyPromotions()
        {
            int accId = GetAccountId();
            Merchant merchant = await _context.Merchants.Where(m => m.Account.Id == accId)
                .Include(m => m.Promotions).ThenInclude(p => p.Image)
                .FirstOrDefaultAsync();

            if (merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            return Ok(merchant.Promotions);
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
                Account = acc
            };

            foreach (var str in model.Tags)
                acc.Merchant.Tags.Add(new Tag() { Text = str });

            var modelOpeningHours = model.GetOpeningHours();
            foreach (var timeSpan in modelOpeningHours)
                acc.Merchant.OpeningHours.Add(timeSpan);

            await _context.SaveChangesAsync();
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

            List<Tag> toRemove = new List<Tag>();
            foreach(var curTag in acc.Merchant.Tags) // Check which tags to remove.
            {
                if(!model.Tags.Contains(curTag.Text))
                    toRemove.Add(curTag);
            }
            foreach(var tag in toRemove) // Remove tags
            {
                acc.Merchant.Tags.Remove(tag);
                _context.Remove(tag);
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

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("Terminate")]
        public async Task<IActionResult> Terminate() // Terminates the Merchant's page, so the account becomes a regular user account.
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            _context.Merchants.Remove(acc.Merchant);
            acc.Merchant = null;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("AddImages")]
        public async Task<IActionResult> AddImages([FromBody]List<string> images)
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            foreach (string imgData in images)
                acc.Merchant.Images.Add(new Image() { Data = imgData });

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("RemoveImage")]
        public async Task<IActionResult> RemoveImage(int id)
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            Image image = await _context.Images
                .Include(e => e.Merchant)
                .Where(e => e.Id == id && e.Merchant.Id == acc.Merchant.Id)
                .FirstOrDefaultAsync();

            if (image == null)
                return Error(ErrorModel.NOT_FOUND);

            acc.Merchant.Images.Remove(image);
            _context.Remove(image);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("AddPromotion")]
        public async Task<IActionResult> AddPromotion([FromBody]MerchantAddPromotionModel model)
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

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
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("AddEvent")]
        public async Task<IActionResult> AddEvent([FromBody]MerchantAddEventModel model)
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

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
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("UpdatePromotion")]
        public async Task<IActionResult> UpdatePromotion([FromBody]MerchantAddPromotionModel model)
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            Promotion promo = await _context.Promotions
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

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("UpdateEvent")]
        public async Task<IActionResult> UpdateEvent([FromBody]MerchantAddEventModel model)
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            Event ev = await _context.Events
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

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("RemovePromotion")]
        public async Task<IActionResult> RemovePromotion(int id)
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            Promotion promo = await _context.Promotions
                .Include(e => e.Organizer)
                .Where(e => e.Id == id && e.Organizer.Id == acc.Merchant.Id)
                .FirstOrDefaultAsync();

            if (promo == null)
                return Error(ErrorModel.NOT_FOUND);

            acc.Merchant.Promotions.Remove(promo);
            _context.Remove(promo);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("RemoveEvent")]
        public async Task<IActionResult> RemoveEvent(int id)
        {
            Account acc = await GetAccount();
            if (acc.Merchant == null)
                return Error(ErrorModel.NOT_A_MERCHANT);

            Event ev = await _context.Events
                .Include(e => e.Organizer)
                .Where(e => e.Id == id && e.Organizer.Id == acc.Merchant.Id)
                .FirstOrDefaultAsync();

            if (ev == null)
                return Error(ErrorModel.NOT_FOUND);

            acc.Merchant.Events.Remove(ev);
            _context.Remove(ev);
            await _context.SaveChangesAsync();
            return Ok();
        }
       
    }
}