using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Data.Entities
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int WorkshopId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Scheduled";

        [MaxLength(500)]
        public string Notes { get; set; }

        public User User { get; set; }
        public Workshop Workshop { get; set; }
        public ICollection<AppointmentBicycle> AppointmentBicycles { get; set; } = new List<AppointmentBicycle>();
    }
}
