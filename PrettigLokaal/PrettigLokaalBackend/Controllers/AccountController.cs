using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
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
    public class AccountController : APIControllerBase
    {

        public AccountController(PrettigLokaalContext context, IConfiguration config) : base(context, config)
        {
        }

        private string GenerateJwtToken(int userId, string userEmail)
        {
            var jwtConfig = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile(_config["Data:JwtConfig"]).Build();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userEmail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["JWT_SIGN"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(30);

            var token = new JwtSecurityToken(
                jwtConfig["ISSUER"],
                jwtConfig["AUDIENCE"],
                claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("Create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateUserModel model)
        {
            if(await GetUserByEmail(model.Email) != null)
                return Error(ErrorModel.EMAIL_ALREADY_IN_USE);
            
            var account = new Account()
            {
                Email = model.Email,
                Fullname = model.Fullname,
                BirthDate = model.BirthDate,
            };
            account.SetPassword(model.Password);

            _context.Accounts.Add(account);
            SaveDB();

            return Ok(new LoginResponse(GenerateJwtToken(account.Id, account.Email)));
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Account account = await GetUserByEmail(model.Email);
            if (account == null)
                return Error(ErrorModel.INVALID_USERNAME);

            if (!account.ComparePassword(model.Password))
                return Error(ErrorModel.INVALID_PASSWORD);

            return Ok(new LoginResponse(GenerateJwtToken(account.Id, account.Email)));
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody]Dictionary<string, string> fields)
        {
            Account acc = await GetAccount();
            foreach(string key in fields.Keys)
            {
                switch(key.ToLower())
                {
                    case "birthdate":
                        acc.BirthDate = DateTime.Parse(fields[key]);
                        break;
                    case "fullname":
                        acc.Fullname = fields[key];
                        break;
                }
            }
            SaveDB();
            return Ok();
        }

        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody]UpdatePasswordModel model)
        {
            Account acc = await GetAccount();
            if (!acc.ComparePassword(model.OldPassword))
                return Error(ErrorModel.INVALID_PASSWORD);
            acc.SetPassword(model.NewPassword);
            SaveDB();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await GetAccount());
        }

    }
}
