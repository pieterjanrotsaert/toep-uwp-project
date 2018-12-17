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
                                padding-top: 4.5em;
                                padding-bottom: 7em;
                            }}
                #details{{
                            padding-left: 5em;
                        }}
                #details *{{
                        padding-bottom: 2em;
                		}}
                p{{
                font-size: 1.5em;
                }}
                #img{{
                    display: inline;
                		}}
                	</style>
                  <title>Promotion</title>
                </head>
                <body>
                <img src='{8}' height='200' style='position: absolute; top:50px; left:700px;'>
                  
                	<div id='details'>
                    <h1>{0}</h1>
                    <p></p>
                    <p> Zaak: {1}</p>
                	<p>	Adres: {2}</p>
                	<p>	Beschrijving: {3}</p>
                	<p>	Geldig vanaf: {4}</p>
                	<p>	Geldig tot: {5}</p>
                	<div id='img' style='padding-top: 5em;'>
                        <img src='{6}' height='250'>
                		<img src='{7}' height='250' style='float: right; margin-right: 2em;'>
                	</div>
                  </div>
                </body>
                </html>", pr.Name, pr.Organizer.Name, pr.Organizer.Address, pr.Description, pr.StartDate.ToShortDateString(), pr.EndDate.ToShortDateString(), "data:image/png;base64," + pr.Image.Data.Data, qr, logo);

            return html;
        }

        private string logo = "data:image/png;base64,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";
        private string qr = "data:image/png;base64,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7ty5c+fOnTt37ty5cz/izp07d+7cuXPnzp07d+7cuXM/4s6dO3fu3Llz586dO3fu3LlzP+LOnTt37ty5c+fOnTt37ty5cz/izp079x/26BgFQCCGoqDgwTyaV/UIlhYiVh+2CMRisZpXLsHghDt37ty5c+fOnTt37twTd+7cuXPnzp07d+7cuXPnzp174s6dO3fu3Llz586dO3fu3LlzT9y5c+fOnTt37ty5c+fOnTt37ok7d+5T3Puepa3/yFK2Tj76lrfvZXkdd+7cuXPnzp07d+7cuXPnzp174s6dO3fu3Llz586dO3fu3LlzT9y5c+fOnTt37ty5c+fOnTt37ok7d+7cuXPnzp07d+7cuXPnzj1x586dO3fu3Llz586dO3fu3Lkn7ty5c+fOnTt37ty5c+fOnTv3xJ07d+7cuXPnzp07d+7cuXPnnrhz586dO3fu3Llz586dO3fu3BN37tx/cu9/rJ642svcw8Rej4zu/dHb5cfwXC8/h2fu3Llz586dO3fu3Llz586dO/fEnTt37ty5c+fOnTt37ty5c+eeuHPnzp07d+4ve3Rsg0AMBFF0iQgpgVKhNEojGumCkXwgBxe8H1or2X7LnTt37ty5c+fOPXHnzp07d+7cuXPnzp07d+7cuSfu3Llz586dO3fu3Llz586dO/fEnTt37ty5c+fOnTt37ty5c+eeuHPnzp07d+7cuXPnzp07d+7cE3fu3Llz586dO3fu3Llz586de+LOnfum1revVTf0nkmPsvTSzudx5859ety5c+fOnTt37ty5c+fOPfW4c+fOnTt37ty5c+fOnXvqcefOnTt37ty5c+fOnTv31OPOnTt37ty5c+fOnTt37qnHnTt37ty5c+fOnTt37txTjzt37ty5c+fOnTt37ty5px537ty5c+fOnTt37ty5c0897ty5c+fOnTt37ty5c+eeety5c+fOnTt37ty5c+fOPfW4c+fOnTt37ty5c7+C+5mP1X6aeK7cj90OE5+ZdM9ZysSfvbhz536ZuJ+LO3fu3Llz/7JDxzYQAkAMBJG+gO+/UzKILrOECZAumG3A8nDnzp37hrh3cefOnTt37ty5c+e+Ie5d3Llz586dO3fu3LlviHsXd+7cuXPnzp07d+4b4t7FnTt37ty5c+fOnfuGuHdx586dO3fu3Llz574h7l3cuXPnzp07d+7cuW+Iexd37ty5c+fOnTt37hvi3sWdO3fu3Llz586d+4a4d3Hnzp07d+7cuXPnviHuXdy5c+fOnTt37ty5f9Pv5fXYM87xpv/11HnkivHieYo7d+7cuXPnzp07d+7cuU857ty5c+fOnTt37ty5c+c+5bhz586dO3fu3Llz586d+5Tjzp07d+7cuXPnzp07d+5Tjjt37ty5c+fOnTt37ty5Tznu3Llz586dO3fu3Llz5z7luHPnzp07d+7cuXPnzp37lOPOnTt37ty5c+fOnTt37lOOO3fu3Llz586dO/ebHTo2QSiIgiiqWIAlWIql/ZYNDYQ1GlAYeGyiybkNDHO4c+eeety5c+fOnTt37ty5c+fO/X8dqzaqpv369evW+OC+22Nk4s6dO3fu3Llz586dO3fuqcedO3fu3Llz586dO3fu3FOPO3fu3Llz586dO3fu3LmnHnfu3Llz586dO3fu3LlzTz3u3Llz586dO3fu3Llz55563Llz586dO3fu3Llz58499bhz586dO3fu3Llz586de+px586dO3fu3Llz586dO/fU486dO3fu3Llz586dO3fuqcedO3fu3Llz586dO3fu3H/Q5et667xqz1NvxLmtlV6noc3x+6rN4wd37tw/486dO3fu3Llz586dO3fu3LlzT9y5c+fOnTt37ty5c+fOnTt37ok7d+7cuXPnzp07d+7cuXPnzj1x586dO3fu3Llz586dO3fu3Lkn7ty5c+fOnTt37ty5c+fOnTv3xJ079zc7dGzCQBQDUTBw4NAluf8uXInvooUfCKToQDAvVCDY4c6dO3fu3Llz586dO3fuiTt37ty5c+fOnTt37ty5c+fOPXHnzp07d+7cuXPnzp07d+7cuadn3D/Xlf79sB7nOP+OczM97uPm7inLu7hz586dO3fu3Llz586de6rjzp07d+7cuXPnzp07d+6pjjt37ty5c+fOnTt37ty5pzru3Llz586dO3fu3Llz557quHPnzp07d+7cuXPnzp17quPOnTt37ty5c+fOnTt37qmOO3fu3Llz586dO3fu3LmnOu7cuXPnzp07d+7cuXPnnuq4c+fOnTt37ty5c+fOnXuq486dO3fu3Llz586dO3fuu/s27k2te153vU/3shd37tyXxn0Ud+7cua+L+yju3LlzXxf3Udy5c+e+Lu6juHPnzn1d3Edx586d+7q4j+LOnTv3dXEfxZ07d+7r4j6KO3fu3NfFfRR37ty5r4v7zR69nCAQBFEURQzEUDU0QzEQQVcFvWjwMejiybnL+dXU6Sju3Llzr4t7FHfu3LnXxT2KO3fu3OviHsWdO3fudXGP4s6dO/e6uEdx586de13co7hz5869Lu5R3Llz514X9yju3Llzr4t7FHfu3LnXxT2KO3fu3OviHsWdO3fudXGP4s6dO/ffdHl9t6PDn8vl6+J+9Ejvq/u2x/JiMJw7d+7cuXPnzp07d+7cuXPnzn3izp07d+7cuXPnzp07d+7cuXOfuHPnzp07d+7cuXPnzp07d+7cJ+7cuXPnzp07d+7cuXPnzp0794k7d+7cuXPnzp07d+7cuXPnzn3izp07d+7cuXPnzp07d+7cuXOfuHPnzp07d+7cuXPnzp07d+7cJ+7cuXPnzp07d+7cuXPnzp0794n7v7nP6mH7xY5+47ySbFc/bW6Pe/x7n4ff9o9w586dO3fu3Llz586dO3fu3LlP3Llz586dO3fu3Llz586d+5s9OjYBEAgCIFiapdmapViGkRotmMiLmAiz6T8cN8ede3Hnzp07d+7cuXPnzp07d+7cuRd37ty5c+fOnTt37ty5c+fOnXtx586dO3fu3Llz586dO3fu3LkXd+7cuXPnzp07d+7cuXPnzp17cefOnTt37ty5c+fOnTt37ty5F3fu3Llz586dO3fu3Llz586de3Hnzv1z9/kYtQwXm46ql6vn/m3ro6MXd+7cuXPnzp07d+7cuXPnzp17cefOnTt37ty5c+fOnTt37ty5F3fu3Llz586dO3fu3Llz586de3Hnzp07d+7cuXPnzp07d+7cuRd37ty5c+fOnTt37ty5c+fOnXtx586dO3fu3Llz586dO3fu3LkXd+7cuXPnzp07d+7cuXPnzp17cefOnTt37ty5c+fOnTt37ty5F3fu3P/vXvfD956r4aO2y2/u3E/26NgEABAIgiD237SBLGhkYiSzDTw3z507d+7cuXPnzp07d+7cuXPnzp07d+7cuXPnzp07d+7cuXPnzp07d+7cuXMv7ty5c+fOnTt37ty5c+fOnTv34s6dO3fu3Llz586dO3fu3LlzL+7cuXPnzp07d+7cuXPnzp079+LOnTt37ty5c+fOnTt37ty5cy/u3Llz586dO3fu3Llz586dO/fizp07d+7cuXPnzv1v970H08dx/NLl6S0v7ty5c+fOnTt37ty5c+fOnTt37ty5c+fOnTt37ty5c+fOnTt37ty5c+fOfcWdO3fu3Llz586dO3fu3Llz517cuXPnzp07d+7cuXPnzp07d+7FnTt37ty5c+fOnTt37ty5c+de3Llz586dO3fu3Llz586dO3fuxZ07d+6TPTpGARCIgShaeACP7LWtxGrAqcKChcL75S4E8sKdO3fu3Llz586dO3fuiTt37ty5c+fOnTt37ty5f8J9oXafVy/3qX3h6B137ty5c+fOnTt37ty5c+fOnTt37ty5c+fOnTt37ty5c+fOnTt37ty5c+fOnTt37ty5J+7cuXPnzp07d+7cuXPnzp0798SdO3fu3Llz586dO3fu3Llz5564c+fOnTt37ty5c+fOnTt37twTd+7cuXPnzp07d+7cuXPnzp174s6dO3fu3Llz586dO3fu3LlzT9y5c+fOnTv3P7i/0uye724bJ189Op2P57pMquajH9y5c+fOnTt37ty5c+fOnTt37ty5c0/cuXPnzp07d+7cuXPnzp07d+6JO3fu3Llz586dO3fu3Llz5849cefOnTt37ty5c+fOnTt37ty5J+7cuXPnzp07d+7cb3boWAAAAABgkL/1rlkUQt69e/fu3fu8e/fu3bt37969e/fu3bt3797n3bt37969e/fu3bt37969e/c+7969e/fu3bt37969e/fu3bt3AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIPbgQAAAAAAAyP+1EVRVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVpT04IAEAAAAQ9P91OwIVAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHgI3ZAv1mh1K4gAAAABJRU5ErkJggg==";
    }
}