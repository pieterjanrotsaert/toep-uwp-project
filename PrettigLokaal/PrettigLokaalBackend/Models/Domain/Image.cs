using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Models.Domain
{
    public class Image
    {
        public int Id { get; set; }
        public string Data { get; set; } // Base64-encoded image data
        [JsonIgnore] public Merchant Merchant { get; set; } // If this is a merchant cover image, this is the merchant, otherwise it's null.
    }
}
