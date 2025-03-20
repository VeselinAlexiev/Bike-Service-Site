using BikeService.Data.Entities;
using BikeService.Web.ViewModel.Home;

namespace BikeService.Web.ViewModel
{
    public class HomeViewModel
    {
        public List<ProductViewModel> Bicycles { get; set; }
        public List<ProductViewModel> SpareParts { get; set; }
    }
}