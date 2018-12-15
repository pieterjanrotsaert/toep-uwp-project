using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Models.Domain
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Image Image { get; set; }

        [JsonIgnore] public Merchant Organizer { get; set; }

        public int OrganizerId { get; set; }

        public string Description { get; set; }
        public string PlaceDescription { get; set; }

        [NotMapped] [JsonIgnore] public ImageData ImageData { get; set; } // Used by the frontend only
        [NotMapped] [JsonIgnore] public bool ImageDataLoading { get; set; } = true;

        public Event() { }
        public Event(Event other)
        {
            Id = other.Id;
            Name = other.Name; 
            StartDate = other.StartDate;
            EndDate = other.EndDate;
            Image = other.Image; 
            Organizer = other.Organizer;
            Description = other.Description;
            PlaceDescription = other.PlaceDescription;
            ImageData = other.ImageData;
            ImageDataLoading = other.ImageDataLoading;
            OrganizerId = other.OrganizerId;
        }

        public Event Clone()
        {
            return new Event(this);
        }


    }
}
