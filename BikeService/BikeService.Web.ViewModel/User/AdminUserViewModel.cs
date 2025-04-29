using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Web.ViewModel.User
{
    public class AdminUserViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new List<string>();

        public string SelectedRole { get; set; }
        public string SelectedWorkshopName { get; set; }

        public List<string> AllRoles { get; set; } = new List<string>();
        public List<string> AllWorkshops { get; set; } = new List<string>();
    }
}
