using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Models.Domain
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Image Image { get; set; }
        [JsonIgnore] public Merchant Organizer { get; set; }
        public bool HasCouponCode { get; set; } = false;
        public string Description { get; set; }

        [NotMapped] [JsonIgnore] public ImageData ImageData { get; set; } // Used by the frontend only
        [NotMapped] [JsonIgnore] public bool ImageDataLoading { get; set; } = true;


        public Promotion() { }
        public Promotion(Promotion other)
        {
            Id = other.Id;
            Name = other.Name;
            StartDate = other.StartDate;
            EndDate = other.EndDate;
            Image = other.Image;
            Organizer = other.Organizer;
            Description = other.Description;
            HasCouponCode = other.HasCouponCode;
            ImageData = other.ImageData;
            ImageDataLoading = other.ImageDataLoading;
        }

        public Promotion Clone()
        {
            return new Promotion(this);
        }
    }
}
