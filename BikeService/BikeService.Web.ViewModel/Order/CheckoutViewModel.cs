using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Web.ViewModel.Order
{
    public class CheckoutViewModel
    {
        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string PaymentMethod { get; set; } = "Cash on Delivery";

        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
    }
}
