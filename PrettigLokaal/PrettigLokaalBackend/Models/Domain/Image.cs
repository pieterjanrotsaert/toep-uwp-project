using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Models.Domain
{
    public class Image
    {
        public int Id { get; set; }
        public ImageData Data { get; set; }

        [NotMapped] [JsonIgnore] public bool IsLoading { get; set; } = true;

        [JsonIgnore] public Merchant Merchant { get; set; } // If this is a merchant cover image, this is the merchant, otherwise it's null.

        public Image() { }
        public Image(int id, string data, bool isLoading = false)
        {
            Id = id;
            Data = new ImageData { Data = data };
            IsLoading = isLoading;
        }

        public Image Clone()
        {
            return new Image()
            {
                Id = Id,
                Data = new ImageData() { Data = Data?.Data, Id = Data != null ? Data.Id : 0 },
                IsLoading = IsLoading
            };
        }


    }
}
