using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.DomainModels
{
    public class Image
    {
        public int Id { get; set; }
        public string Data { get; set; } // Base64 image data
    }
}
