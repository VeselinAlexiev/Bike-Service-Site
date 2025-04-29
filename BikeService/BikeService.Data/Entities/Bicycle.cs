using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Data.Entities
{
    public class Bicycle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Model { get; set; }

        [Required]
        [MaxLength(50)]
        public string Brand { get; set; }

        public int Year { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string Description { get; set; }

        [MaxLength(250)]
        public string ImageUrl { get; set; }

        public bool EcoFriendly { get; set; } = false;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? BatteryCapacity { get; set; }

        [MaxLength(50)]
        public string EnergySource { get; set; }

        [MaxLength(50)]
        public string Material { get; set; }

        public ICollection<AppointmentBicycle> AppointmentBicycles { get; set; }
        public ICollection<BicycleSparePart> BicycleSpareParts { get; set; } = new List<BicycleSparePart>();
    }
}
