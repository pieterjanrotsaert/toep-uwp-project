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
    public class FileController : APIControllerBase
    {
        public FileController(IConfiguration config, PrettigLokaalContext context) : base(context, config)
        {

        }

        [HttpGet("Image/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetImage(int id)
        {
            var img = await context.Images
                .Where(i => i.Id == id)
                .Include(i => i.Data)
                .FirstOrDefaultAsync();

            if (img == null)
                return Error(ErrorModel.NOT_FOUND);

            return Ok(img);
        }
    }
}