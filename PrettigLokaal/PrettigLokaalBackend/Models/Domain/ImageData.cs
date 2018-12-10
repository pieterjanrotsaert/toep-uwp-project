﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Models.Domain
{
    public class ImageData
    {
        [JsonIgnore] public int Id { get; set; }
        public string Data { get; set; }
    }
}
