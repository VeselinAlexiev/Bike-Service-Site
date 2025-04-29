using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BikeService.Data.Entities
{
    public class User : IdentityUser
    {
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();

        public int? WorkshopId { get; set; }
        public Workshop Workshop { get; set; }
    }

}
