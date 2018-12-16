using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
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
        private readonly IConverter _converter;

        public FileController(IConfiguration config, PrettigLokaalContext context, IConverter converter) : base(context, config)
        {
            _converter = converter;
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

        [HttpGet("Pdf/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPdf(int id)
        {
            var promotion = await context.Promotions
                .Include(pr => pr.Image)
                .ThenInclude(item => item.Data)
                .Include(pr => pr.Organizer)
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (promotion == null) return File(new byte[0], "application/pdf", "NotFound.pdf");

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = { Right = 0, Bottom = 0, Left = 0, Top = 0 }
                },
                Objects = {
                    new ObjectSettings() {
                        HtmlContent = BuildPromotionCouponCodeHtml(promotion),
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };



            byte[] pdf = _converter.Convert(doc);

            return File(pdf, "application/pdf", promotion.Name + ".pdf");
        }

        private string BuildPromotionCouponCodeHtml(Promotion pr)
        {
            string html = string.Format(@"<!doctype html>
                <html lang='en'>
                 <head>
                   <meta charset='utf-8'>
                      <style type='text/css'>
                          *{{
                                font-family: Verdana, Geneva, sans-serif;
                            }}
                            h1{{
                                padding-top: 3em;
                                text-align: center;
                            }}
                #details{{
                            padding-top: 10em;
                            padding-left: 5em;
                        }}
                #details *{{
                        padding-bottom: 2em;
                			font-size: 1.5em;
                		}}
                #img{{
                    display: inline;
                		}}
                	</style>
                  <title>Promotion</title>
                </head>
                <body>
                <img src='https://localhost:3001/Assets/Square150x150Logo.scale-200.png' height='200' style='position: absolute; top:50px; left:700px;'>
                  <h1>{0}</h1>
                	<div id='details'>
                    <p></p>
                    <p> Zaak: {1}</p>
                	<p>	Adres: {2}</p>
                	<p>	Beschrijving: {3}</p>
                	<p>	Geldig vanaf: {4}</p>
                	<p>	Geldig tot: {5}</p>
                	<div id='img' style='padding-top: 5em;'>
                        <img src='{6}' height='250'>
                		<img src='https://localhost:3001/Assets/sample.png' height='250' style='float: right; margin-right: 2em;'>
                	</div>
                  </div>
                </body>
                </html>", pr.Name, pr.Organizer.Name, pr.Organizer.Address, pr.Description, pr.StartDate.ToShortDateString(), pr.EndDate.ToShortDateString(), "data:image/png;base64," + pr.Image.Data.Data);

            return html;
        }
    }
}