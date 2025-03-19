using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Data.Entities
{
    public class AppointmentBicycle
    {
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        public int BicycleId { get; set; }
        public Bicycle Bicycle { get; set; }
    }
}
